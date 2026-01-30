# SignalR Implementation Summary

This document summarizes the completed implementation of SignalR-based bidirectional communication in Electron.NET as an alternative to Socket.IO.

## Overview

The SignalR implementation provides a modern, .NET-native alternative to Socket.IO for communication between the ASP.NET Core host and the Electron process. This new startup mode was designed specifically for **Blazor Server applications** where ASP.NET Core and Electron need tighter integration and lifecycle control.

**Key Innovation**: .NET-first startup with dynamic port assignment - ASP.NET Core starts first, binds to port 0 (letting Kestrel choose an available port), then launches Electron with the actual URL.

## Primary Use Case

Blazor Server applications where:
- ASP.NET Core owns the application lifecycle
- Dynamic port binding is needed (no fixed port configuration)
- Modern SignalR infrastructure is preferred over Socket.IO
- Single process debugging is desired (.NET process controls Electron)

## Implementation Phases (All Complete)

### Phase 1: Core Infrastructure ✅
- Added new `StartupMethod` enum values:
  - `UnpackagedDotnetFirstSignalR` 
  - `PackagedDotnetFirstSignalR`
- Created `ElectronHub` SignalR hub for bidirectional communication
- Registered hub endpoint at `/electron-hub` (separate from Blazor's `/_blazor` hub)

### Phase 2: Runtime Controller ✅
- Created `RuntimeControllerAspNetDotnetFirstSignalR`
- Implemented logic to:
  - Bind Kestrel to port 0
  - Wait for Kestrel startup and capture actual port via `IServerAddressesFeature`
  - Launch Electron with `--electronurl` parameter
  - Wait for SignalR connection from Electron
  - Transition to Ready state when connected

### Phase 3: Electron/Node.js Side ✅
- Added `@microsoft/signalr` npm package dependency
- Created SignalR connection module (`signalr-bridge.js`)
- Updated `main.js` to detect SignalR modes and connect to `/electron-hub`
- Implemented Socket.IO-compatible interface for API compatibility

### Phase 4: API Bridge Adaptation ✅
- Created `SignalRFacade` implementing `IFacade` interface
- Ensured existing Electron API classes work with SignalR
- Implemented type conversion helper for SignalR's JSON deserialization
- Event routing from both directions (.NET ↔ Electron)

### Phase 5: Configuration & Extensions ✅
- Updated `WebHostBuilderExtensions` for automatic SignalR configuration
- Added startup mode detection via command-line flags
- Configured dynamic port binding (port 0) for SignalR modes
- Integrated with `UseElectron()` API for seamless usage

### Phase 6: Testing & Fixes ✅
- Created sample Blazor Server application
- Fixed multiple critical issues discovered during integration testing
- Cleaned up debug logging
- Added comprehensive code documentation

## Key Components

### 1. SignalRFacade (`src/ElectronNET.AspNet/Bridge/SignalRFacade.cs`)
- Implements `IFacade` interface to match Socket.IO facade API
- Handles bidirectional event routing using `IHubContext<ElectronHub>`
- Includes `ConvertToType<T>` helper for handling SignalR's JSON deserialization quirks
- Critical fix: Handles `JsonElement` and numeric type conversions (long → int)

### 2. ElectronHub (`src/ElectronNET.AspNet/Hubs/ElectronHub.cs`)
- SignalR hub for .NET ↔ Electron communication
- Key methods:
  - `RegisterElectronClient()` - Called by Electron on connection
  - `ElectronEvent(string, object[])` - Receives events from Electron
  - Connection/disconnection handlers notify runtime controller

### 3. RuntimeControllerAspNetDotnetFirstSignalR
- Manages SignalR mode lifecycle
- Critical flow:
  1. Wait for ASP.NET server to start
  2. Capture dynamic port from `IServerAddressesFeature`
  3. Update `ElectronNetRuntime.AspNetWebPort` with actual port
  4. Launch Electron with `--electronurl` parameter
  5. Wait for `electron-host-ready` signal before calling app ready callback

### 4. SignalRBridge (`src/ElectronNET.Host/api/signalr-bridge.js`)
- JavaScript SignalR client that mimics Socket.IO interface
- Provides `on()` and `emit()` methods for API compatibility
- Critical fix: Event args passed as arrays, spread when calling handlers
- Uses `@microsoft/signalr` npm package

### 5. Main.js Startup (`src/ElectronNET.Host/main.js`)
- Detects SignalR mode via `--unpackeddotnetsignalr` or `--dotnetpackedsignalr` flags
- Creates invisible keep-alive window (destroyed when first real window is created)
- Loads API modules then signals `electron-host-ready` to .NET

## Usage

Enable SignalR mode by passing the appropriate command-line flag:

```bash
# Unpacked mode (development)
dotnet run --unpackeddotnetsignalr

# Packed mode (production)
dotnet run --dotnetpackedsignalr
```

Or set environment variable (deprecated, flags preferred):
```bash
ELECTRON_USE_SIGNALR=true
```

In your ASP.NET Core Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Electron.NET services
builder.Services.AddElectron();

// Configure Electron with SignalR mode
builder.WebHost.UseElectron(args, async () =>
{
    var window = await Electron.WindowManager.CreateWindowAsync();
    window.OnReadyToShow += () => window.Show();
});

var app = builder.Build();

// Enable WebSockets (required for SignalR)
app.UseWebSockets();
app.UseRouting();

// Register SignalR hub endpoint
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ElectronHub>("/electron-hub");
});

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

**Note**: `UseElectron()` automatically detects SignalR mode and configures everything. The rest happens automatically:
1. Port 0 binding (dynamic port assignment)
2. Electron launch with actual URL
3. SignalR connection establishment
4. App ready callback execution

## Architecture Decisions

### Why .NET-First Startup?
SignalR mode uses .NET-first startup (vs. Electron-first in Socket.IO mode) because:
1. **No port scanning needed** - .NET can pass the actual URL to Electron
2. **SignalR hub must be registered** before Electron connects
3. **Simpler lifecycle** - ASP.NET controls when Electron launches
4. **Better for Blazor Server** - Blazor is already running when Electron starts
5. **Single process debugging** - Developer debugs .NET process which owns Electron

### Why IFacade Interface?
Introducing `IFacade` allows `BridgeConnector.Socket` to return either `SocketIOFacade` or `SignalRFacade` based on startup mode, ensuring existing API code works with both transport mechanisms without modification.

### Why Keep-Alive Window?
Electron quits immediately on macOS if no windows exist. The keep-alive window ensures Electron stays running during the connection and API initialization phase. It's automatically destroyed when the first real window is created.

### Why 'electron-host-ready' Signal?
Without this signal, .NET would call the app ready callback before Electron finished loading API modules, causing API calls to fail. The signal ensures proper initialization order:
1. Electron connects to SignalR
2. Electron loads all API modules (browserWindows, dialog, menu, etc.)
3. Electron signals `electron-host-ready`
4. .NET calls app ready callback
5. App code can safely use Electron APIs

## Blazor Server Considerations

### SignalR Hub Coexistence
Blazor Server already uses SignalR for component communication (`/_blazor` hub). Our implementation:
- Uses separate endpoint (`/electron-hub`) to avoid conflicts
- Both hubs coexist on the same Kestrel server without interference
- No impact on Blazor's reconnection logic
- Compatible with hot reload scenarios

### Lifecycle Integration
- Electron window creation happens **after** Blazor app is ready
- `UseElectron()` callback fires when SignalR hub is connected
- Blazor components can inject Electron services to control windows
- Proper disposal when Electron process exits

### Development Experience
- Hot reload works for both Blazor and Electron integration
- F5 debugging works seamlessly
- No need to manually coordinate ports
- Single process to debug (.NET process owns the lifecycle)

## Critical Fixes Applied

### 1. Race Condition: API Module Loading
**Problem**: .NET called app ready callback before Electron finished loading API modules.

**Solution**: Electron signals `electron-host-ready` after loading all API modules. .NET waits for this signal before calling the app ready callback.

### 2. Event Argument Mismatch
**Problem**: SignalR sent event data as nested arrays `[[data]]` instead of `[data]`.

**Solution**: 
- C#: Use explicit `object[] args` parameter (not `params`)
- JS: Always pass args as array: `invoke('ElectronEvent', eventName, args)`
- JS: Spread args when calling handlers: `handler(...argsArray)`

### 3. Type Conversion Failures
**Problem**: SignalR deserializes JSON numbers as `JsonElement` or `long`, causing `Once<int>` handlers to fail silently.

**Solution**: `SignalRFacade.ConvertToType<T>` handles JsonElement deserialization and numeric conversions.

### 4. Window Shutdown Not Triggering Exit
**Problem**: Keep-alive window prevented `window-all-closed` event from firing.

**Solution**: Destroy keep-alive window when first real window is created using `app.once('browser-window-created')`.

### 5. Dynamic Port Not Propagated
**Problem**: When using port 0, Kestrel assigns a dynamic port, but `ElectronNetRuntime.AspNetWebPort` was not updated.

**Solution**: Update `AspNetWebPort` after capturing port from `IServerAddressesFeature` in `CapturePortAndLaunchElectron()`.

### 7. Blazor Static Files Not Loading
**Problem**: Blazor CSS and framework files returned 404 errors.

**Solution**: 
- Added `app.UseStaticFiles()` to serve wwwroot content
- Fixed middleware order: `UseAntiforgery()` must be between `UseRouting()` and `UseEndpoints()`
- Updated scoped CSS asset reference to use lowercase name matching .NET 9+ convention

## Backward Compatibility

This is a **new optional startup mode** - all existing modes continue to work unchanged:
- `PackagedElectronFirst` - unchanged
- `PackagedDotnetFirst` - unchanged  
- `UnpackedElectronFirst` - unchanged
- `UnpackedDotnetFirst` - unchanged

Existing applications do not need to change. SignalR mode is opt-in via command-line flags.

## File Changes Summary

**New Files**:
- `src/ElectronNET.AspNet/Bridge/SignalRFacade.cs` (225 lines)
- `src/ElectronNET.AspNet/Hubs/ElectronHub.cs` (108 lines)
- `src/ElectronNET.AspNet/Runtime/Controllers/RuntimeControllerAspNetDotnetFirstSignalR.cs` (163 lines)
- `src/ElectronNET.Host/api/signalr-bridge.js` (125 lines)

**Modified Files**:
- `src/ElectronNET.AspNet/API/WebHostBuilderExtensions.cs` - Added SignalR service registration
- `src/ElectronNET.Host/main.js` - Added SignalR startup flow
- `src/ElectronNET.Host/package.json` - Added `@microsoft/signalr` dependency
- `src/ElectronNET.Samples.BlazorSignalR/Program.cs` - Sample implementation

**Total Changes**: ~1,030 lines added

## Testing Recommendations

1. Test with dynamic port (port 0) to ensure URL propagation works
2. Verify window-all-closed triggers app exit
3. Test rapid window creation/destruction
4. Verify reconnection behavior if SignalR connection drops
5. Test with both packed and unpacked modes
6. Verify API calls work correctly (especially those returning data)

## Known Limitations

1. **Request-response pattern not yet implemented** - `InvokeElectronApi` is a placeholder. Current API calls use event-based pattern.
2. **TouchBar API not yet supported** on macOS SignalR mode
3. **SignalR automatic reconnection** may cause issues with pending API calls (needs circuit breaker pattern)

## Future Enhancements

1. **Request-response pattern** - Implement proper async/await pattern for API calls that return values
2. **Metrics/diagnostics** - Add SignalR connection health monitoring
3. **Circuit breaker** - Handle reconnection scenarios gracefully
4. **Integration tests** - Comprehensive test suite for SignalR mode
5. **Performance benchmarks** - Compare SignalR vs Socket.IO performance

## Success Metrics

The implementation is considered complete and functional:
- ✅ .NET starts first with dynamic port (port 0)
- ✅ Electron launches with actual URL
- ✅ SignalR connection establishes successfully
- ✅ API modules load before app ready callback
- ✅ Window creation works from .NET
- ✅ Window shutdown triggers app exit
- ✅ Blazor Server pages load with correct styling
- ✅ Both SignalR hubs coexist (Electron + Blazor)
- ✅ Clean codebase with minimal debug logging
- ✅ Comprehensive inline documentation
