# Secure Communication

By default, the IPC communication between .NET and Node.js is secured on startup. Consequently, multiple instances running on different user accounts (but shared on the same machine) can safely co-exist. However, this protection is not enough to secure the web application behind - or make any security statement w.r.t. a malicious root user.

## Securing the Web Application

You can opt-in to also guard your ASP.NET Core application using the same mechanism that is already used to protected the IPC broker that deals with the .NET to Node.js communication.

The key to opt-in is to provide another service *before* calling `AddElectron` on the service collection.

The following two namespaces are used in the next instructions:

```cs
using ElectronNET.AspNet.Middleware;
using ElectronNET.AspNet.Services;
```

You'll need the following line:

```cs
builder.Services.AddSingleton<IElectronAuthenticationService, ElectronAuthenticationService>();
```

This way, Electron.NET is notified that you want to store and re-use the authentication token that has been negotiated between the .NET and Node.js processes at startup.

With this being set up you can register a middleware to actually deny requests that have originated outside of your Electron.NET application:

```cs
app.UseMiddleware<ElectronAuthenticationMiddleware>();
```

This must be placed above any routing (e.g., before calling `UseRouting` on the web application) in order to properly take effect.
