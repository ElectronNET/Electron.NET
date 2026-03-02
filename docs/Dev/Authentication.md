# Electron.NET SignalR Authentication Guide

This guide explains the token-based authentication system implemented for SignalR mode in Electron.NET, designed to protect applications in multi-user environments.

## Table of Contents

1. [Overview](#overview)
2. [Threat Model](#threat-model)
3. [Authentication Architecture](#authentication-architecture)
4. [Implementation Details](#implementation-details)
5. [Security Properties](#security-properties)
6. [Troubleshooting](#troubleshooting)
7. [FAQ](#faq)

## Overview

Electron.NET's SignalR mode includes built-in authentication to ensure that only the Electron process spawned by a specific .NET instance can connect to that instance's HTTP and SignalR endpoints.

**When is this important?**
- Multi-user Windows Server environments (Terminal Services, RDP)
- Shared development machines with multiple users
- Any scenario where multiple users run the same application simultaneously

**What does it protect against?**
- User A's Electron process connecting to User B's .NET backend
- Unauthorized port scanning and connection attempts from other users
- Accidental misconfigurations causing cross-user connections

## Threat Model

### The Problem

When multiple users run the same Electron.NET application on a shared server:

1. Each .NET process binds to a TCP port (e.g., `http://localhost:58971`)
2. TCP ports are visible to **all users** on the machine
3. Without authentication, User A's Electron could connect to User B's backend

```
┌─────────────────────────────────────────────────┐
│ Windows Server (Terminal Services)             │
├─────────────────────────────────────────────────┤
│ User A Session                                  │
│   ├─ .NET Process (localhost:58971)            │
│   └─ Electron Process                          │
│                                                 │
│ User B Session                                  │
│   ├─ .NET Process (localhost:61234)            │
│   └─ Electron Process (could connect to 58971) │ ❌ Prevent this!
└─────────────────────────────────────────────────┘
```

### The Solution

Each .NET process generates a unique authentication token when launching Electron. Only requests with the correct token are allowed to connect.

```
┌─────────────────────────────────────────────────┐
│ User A Session                                  │
│   ├─ .NET (token: abc123...)                   │
│   └─ Electron (has token abc123...)            │ ✅ Authenticated
│                                                 │
│ User B Session                                  │
│   ├─ .NET (token: xyz789...)                   │
│   └─ Electron (has token xyz789...)            │ ✅ Authenticated
│                                                 │
│   ❌ User B's Electron → User A's .NET          │
│      (lacks token abc123...)                    │ ❌ Rejected (401)
└─────────────────────────────────────────────────┘
```

## Authentication Architecture

### Flow Diagram

```
┌──────────────┐                                    ┌─────────────────┐
│ .NET Process │                                    │ Electron Process│
└──────┬───────┘                                    └────────┬────────┘
       │                                                     │
       │ 1. Generate Token (GUID)                           │
       │    Token: a3f8b2c1d4e5...                         │
       │                                                     │
       │ 2. Launch Electron                                 │
       │    --authtoken=a3f8b2c1d4e5...                    │
       │────────────────────────────────────────────────────>│
       │                                                     │
       │                                                     │ 3. Extract Token
       │                                                     │    global.authToken = ...
       │                                                     │
       │                                                     │ 4. Initial Page Load
       │<────────────────────────────────────────────────────│
       │    GET /?token=a3f8b2c1d4e5...                     │
       │                                                     │
       │ 5. Validate Token                                  │
       │    ✓ Valid → Set Cookie                            │
       │────────────────────────────────────────────────────>│
       │    HTTP 200 + Set-Cookie: ElectronAuth=...        │
       │                                                     │
       │                                                     │ 6. SignalR Connection
       │<────────────────────────────────────────────────────│
       │    GET /electron-hub?token=a3f8b2c1d4e5...         │
       │                                                     │
       │ 7. Validate Token, Set Cookie                      │
       │────────────────────────────────────────────────────>│
       │    HTTP 200 + Set-Cookie                           │
       │                                                     │
       │                                                     │ 8. Subsequent Requests
       │<────────────────────────────────────────────────────│
       │    GET /api/data                                   │
       │    Cookie: ElectronAuth=a3f8b2c1d4e5...            │
       │                                                     │
       │ 9. Validate Cookie                                 │
       │────────────────────────────────────────────────────>│
       │    HTTP 200 (authenticated)                        │
       │                                                     │
```

### Key Steps

1. **Token Generation**: .NET generates 128-bit cryptographic random GUID
2. **Command-Line Passing**: Token passed to Electron via `--authtoken` parameter
3. **Token Extraction**: Electron stores token in `global.authToken`
4. **URL Appending**: Token appended to initial page and SignalR connection URLs
5. **Middleware Validation**: Every HTTP request validated by middleware
6. **Cookie Setting**: Valid token results in secure HttpOnly cookie
7. **Cookie-Based Requests**: Subsequent requests use cookie (no token in URL)

## Implementation Details

### 1. Token Generation (.NET)

**File**: `src/ElectronNET.AspNet/Runtime/Controllers/RuntimeControllerAspNetDotnetFirstSignalR.cs`

```csharp
private void LaunchElectron()
{
    // Generate secure authentication token (128-bit cryptographic random GUID)
    this.authenticationToken = Guid.NewGuid().ToString("N"); // 32 hex chars
    
    // Register token with authentication service
    this.authenticationService.SetExpectedToken(this.authenticationToken);
    
    // Launch Electron with token
    var args = $"--unpackeddotnetsignalr --electronurl={this.actualUrl} --authtoken={this.authenticationToken}";
    this.electronProcess = new ElectronProcessActive(isUnPacked, ElectronNetRuntime.ElectronExecutable, args, this.port.Value);
    _ = this.electronProcess.Start();
}
```

**Token Format**: 32-character hexadecimal string (e.g., `a3f8b2c1d4e5f6a7b8c9d0e1f2a3b4c5`)

### 2. Authentication Service (.NET)

**File**: `src/ElectronNET.AspNet/Services/ElectronAuthenticationService.cs`

```csharp
public class ElectronAuthenticationService : IElectronAuthenticationService
{
    private string _expectedToken;
    private readonly object _lock = new object();

    public void SetExpectedToken(string token)
    {
        lock (_lock)
        {
            _expectedToken = token;
        }
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        lock (_lock)
        {
            if (string.IsNullOrEmpty(_expectedToken))
                return false;

            // Constant-time comparison prevents timing attacks
            return ConstantTimeEquals(token, _expectedToken);
        }
    }

    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a == null || b == null || a.Length != b.Length)
            return false;

        var result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}
```

**Key Features**:
- Thread-safe with lock
- Constant-time comparison (prevents timing attacks)
- Singleton lifetime (one per .NET instance)

### 3. Authentication Middleware (.NET)

**File**: `src/ElectronNET.AspNet/Middleware/ElectronAuthenticationMiddleware.cs`

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Check if authentication cookie exists
    var authCookie = context.Request.Cookies["ElectronAuth"];
    
    if (!string.IsNullOrEmpty(authCookie))
    {
        if (_authService.ValidateToken(authCookie))
        {
            await _next(context);
            return;
        }
        else
        {
            _logger.LogWarning("Invalid cookie for path {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid authentication");
            return;
        }
    }

    // No cookie - check for token in query string
    var token = context.Request.Query["token"].ToString();
    
    if (!string.IsNullOrEmpty(token) && _authService.ValidateToken(token))
    {
        // Valid token - set cookie for future requests
        context.Response.Cookies.Append("ElectronAuth", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Secure = false,  // localhost is HTTP
            IsEssential = true
        });
        
        await _next(context);
        return;
    }

    // Reject - no valid cookie or token
    context.Response.StatusCode = 401;
    await context.Response.WriteAsync("Unauthorized: Authentication required");
}
```

**Middleware Order** (IMPORTANT):
```csharp
app.UseMiddleware<ElectronAuthenticationMiddleware>();  // ← FIRST
app.UseRouting();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapHub<ElectronHub>("/electron-hub");
app.MapRazorComponents<App>();
```

### 4. Token Extraction (Electron)

**File**: `src/ElectronNET.Host/main.js`

```javascript
// Extract authentication token from command-line
if (app.commandLine.hasSwitch('authtoken')) {
    global.authToken = app.commandLine.getSwitchValue('authtoken');
}
```

### 5. Token in URLs (Electron)

**Initial Page Load** (`src/ElectronNET.Host/api/browserWindows.js`):
```javascript
// Append token to window URL
if (global.authToken) {
    const separator = electronUrl.includes('?') ? '&' : '?';
    electronUrl = `${electronUrl}${separator}token=${global.authToken}`;
}
window.loadURL(electronUrl);
```

**SignalR Connection** (`src/ElectronNET.Host/api/signalr-bridge.js`):
```javascript
async connect() {
    // Append token to SignalR hub URL
    const connectionUrl = this.authToken ? 
        `${this.hubUrl}?token=${this.authToken}` : 
        this.hubUrl;
    
    this.connection = new signalR.HubConnectionBuilder()
        .withUrl(connectionUrl)
        .build();
    
    await this.connection.start();
}
```

### 6. Service Registration (Application)

**File**: Your application's `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register authentication service as singleton
builder.Services.AddSingleton<IElectronAuthenticationService, ElectronAuthenticationService>();

builder.Services.AddElectron();

var app = builder.Build();

// Register middleware BEFORE UseRouting()
app.UseMiddleware<ElectronAuthenticationMiddleware>();

app.UseRouting();
app.MapHub<ElectronHub>("/electron-hub");
app.Run();
```

## Security Properties

### Cookie Configuration

| Property | Value | Purpose |
|----------|-------|---------|
| `HttpOnly` | `true` | Prevents JavaScript access (XSS protection) |
| `SameSite` | `Strict` | Prevents CSRF attacks |
| `Path` | `/` | Cookie sent with all requests |
| `Secure` | `false` | Cannot use on localhost HTTP |
| `IsEssential` | `true` | Required for app to function |
| **Lifetime** | Session | Expires when Electron closes |

### Token Properties

- **Entropy**: 128 bits (2^128 possible values)
- **Format**: 32 hexadecimal characters (GUID without hyphens)
- **Generation**: `Guid.NewGuid()` uses cryptographically secure RNG
- **Lifetime**: Entire application session
- **Uniqueness**: Each .NET instance generates unique token

### What's Protected

✅ **All HTTP endpoints**:
- Blazor Server pages (`/`, `/counter`, etc.)
- Static files (`/css/app.css`, `/js/script.js`)
- API endpoints (custom controllers)
- SignalR hub (`/electron-hub`)

✅ **Both transport modes**:
- Initial token-based authentication (query parameter)
- Cookie-based subsequent requests

✅ **Cross-user isolation**:
- Different users = different tokens
- Invalid token = 401 Unauthorized

### What's NOT Protected Against

❌ **Same-user attacks** (by design):
- Process memory inspection
- Debugger attachment
- Command-line parameter visibility

**Rationale**: A malicious process running as the same user already has full access to:
- Process memory (can read token from RAM)
- Cookies (stored in Electron's data directory)
- All files owned by the user

Token-based authentication focuses on **cross-user isolation**, not same-user security.

## Troubleshooting

### Problem: 401 Unauthorized on Initial Load

**Symptoms**:
```
GET / HTTP/1.1
Response: 401 Unauthorized
```

**Possible Causes**:
1. Token not passed to Electron
2. Token not appended to URL
3. Middleware rejecting valid token

**Debugging Steps**:

1. Check Electron command-line:
   ```javascript
   console.log('Auth Token:', global.authToken);
   ```
   Should print 32-character hex string.

2. Check URL in browser window:
   ```javascript
   console.log('Loading URL:', electronUrl);
   ```
   Should include `?token=<guid>` parameter.

3. Check .NET logs:
   ```
   [Warning] Authentication failed: Invalid token (prefix: a3f8b2c1...)
   ```

4. Verify middleware registration:
   ```csharp
   app.UseMiddleware<ElectronAuthenticationMiddleware>(); // Before UseRouting()
   ```

### Problem: SignalR Connection Fails

**Symptoms**:
```
[SignalRBridge] Authentication failed: The authentication token is invalid or missing.
```

**Possible Causes**:
1. Token not passed to `SignalRBridge` constructor
2. Token not appended to hub URL
3. Cookie not being sent

**Debugging Steps**:

1. Check token passed to SignalRBridge:
   ```javascript
   console.log('SignalRBridge token:', this.authToken);
   ```

2. Check connection URL:
   ```javascript
   console.log('Hub URL:', connectionUrl);
   ```
   Should be `http://localhost:PORT/electron-hub?token=<guid>`.

3. Enable verbose logging:
   ```javascript
   .configureLogging(signalR.LogLevel.Debug)
   ```

### Problem: Cookie Not Persisting

**Symptoms**: Every request includes token in URL, cookie never set.

**Possible Causes**:
1. Cookie settings incompatible with browser
2. Middleware not setting cookie
3. Response headers not sent

**Debugging Steps**:

1. Check response headers:
   ```
   Set-Cookie: ElectronAuth=a3f8b2c1...; Path=/; HttpOnly; SameSite=Strict
   ```

2. Check subsequent requests:
   ```
   Cookie: ElectronAuth=a3f8b2c1...
   ```

3. Enable middleware logging:
   ```csharp
   _logger.LogInformation("Setting cookie for path {Path}", path);
   ```

### Problem: Token Visible in Process List

**Observation**:
```powershell
wmic process where "name='electron.exe'" get commandline
...--authtoken=a3f8b2c1d4e5f6a7b8c9d0e1f2a3b4c5...
```

**Is this a problem?** No, by design.

**Explanation**:
- Command-line parameters are visible to same-user processes
- This is acceptable because:
  - Same user already has access to process memory
  - Same user can read cookies from Electron's data directory
  - Token-based auth protects against **other users**, not same-user processes

**If you need same-user protection**: Use OS-level access controls (file permissions, process isolation) or consider named pipes with ACLs.

## FAQ

### Q: Why use tokens instead of Process ID validation?

**A**: PIDs can be recycled and reused, making validation unreliable. Additionally:
- PIDs don't provide cryptographic security
- Parent-child validation is platform-specific
- Adds complexity without meaningful security benefit

Token-based authentication provides:
- Cryptographic randomness (128-bit entropy)
- Simple cross-platform implementation
- No race conditions or PID recycling issues

### Q: Why not use HTTPS with certificates?

**A**: Localhost doesn't support HTTPS certificates easily:
- Self-signed certificates trigger browser warnings
- Certificate management adds complexity
- Token-based auth provides equivalent security for localhost IPC

### Q: Can I disable authentication?

**A**: Not recommended, but possible by removing middleware registration:

```csharp
// Remove this line:
// app.UseMiddleware<ElectronAuthenticationMiddleware>();
```

**Warning**: Only do this if:
- Application runs on single-user machines only
- No Terminal Services / RDP access
- You understand the security implications

### Q: Does this work with hot reload?

**A**: Yes, cookie persists across hot reload as long as Electron process keeps running.

### Q: What about multiple Electron windows?

**A**: All windows in the same Electron process share cookies automatically. Authentication works seamlessly across multiple windows.

### Q: How do I test authentication?

**Test 1 - Happy Path**:
1. Run application normally
2. Check logs for "Authentication successful"
3. Verify cookie is set (DevTools → Application → Cookies)
4. Subsequent requests should not include token in URL

**Test 2 - Invalid Token**:
1. Modify token in browser URL: `?token=invalid`
2. Should receive 401 Unauthorized
3. Check logs for "Authentication failed: Invalid token"

**Test 3 - No Token**:
1. Open browser manually to `http://localhost:PORT/`
2. Should receive 401 Unauthorized
3. Check logs for "Authentication failed: No cookie or token"

**Test 4 - Multi-User** (Windows Server/Terminal Services):
1. Launch app as User A
2. In User B session, try to connect to User A's port
3. Should receive 401 Unauthorized

### Q: What about packaged applications?

**A**: Authentication works identically in packaged mode. The `--authtoken` parameter is included in the packaged Electron executable.

### Q: Can I customize the cookie name?

**A**: Yes, modify `AuthCookieName` constant in `ElectronAuthenticationMiddleware.cs`:

```csharp
private const string AuthCookieName = "MyCustomCookieName";
```

## Summary

Electron.NET's token-based authentication provides:

✅ **Security**: 128-bit entropy, constant-time comparison, secure cookies  
✅ **Simplicity**: Automatic token generation and validation  
✅ **Compatibility**: Works with Blazor, SignalR, and static files  
✅ **Monitoring**: Structured logging for security events  
✅ **Multi-User**: Cross-user isolation on shared servers  

The authentication system is **enabled by default** in SignalR mode and requires minimal configuration. For most applications, simply register the services and middleware - everything else happens automatically.

For additional help or questions, see the [SignalR Implementation Summary](SignalR-Implementation-Summary.md) or open an issue on GitHub.
