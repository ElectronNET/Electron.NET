# Electron.NET Blazor SignalR Sample

This sample demonstrates how to use Electron.NET with **SignalR-based startup mode** in a Blazor Server application.

## Features

✅ **SignalR Communication** - Uses SignalR instead of socket.io for .NET ↔ Electron communication  
✅ **Dynamic Port Assignment** - Kestrel binds to port 0, avoiding conflicts  
✅ **Blazor Server** - Full Blazor Server support with interactive components  
✅ **Electron Integration** - Native desktop window with auto-hide menu bar  

## Prerequisites

- .NET 10.0 SDK or later
- Node.js 22.x or later

## How to Run

### Method 1: Visual Studio

1. Open the solution in Visual Studio
2. Set `ElectronNET.Samples.BlazorSignalR` as the startup project
3. Press **F5** to run

The environment variable `ELECTRON_USE_SIGNALR=true` is already configured in `launchSettings.json`.

### Method 2: Command Line

```bash
cd src/ElectronNET.Samples.BlazorSignalR
set ELECTRON_USE_SIGNALR=true    # Windows
# or
export ELECTRON_USE_SIGNALR=true # Linux/Mac
dotnet run
```

## What Happens

1. ASP.NET Core starts with Kestrel on port 0 (random available port)
2. Electron.NET detects `ELECTRON_USE_SIGNALR=true` environment variable
3. Runtime controller captures the actual port (e.g., `http://localhost:54321`)
4. Electron process is launched with `--electronUrl=http://localhost:54321`
5. Electron connects to SignalR hub at `/electron-hub`
6. Once connected, the `ElectronAppReady` callback fires
7. A browser window is created showing the Blazor Server application
8. Both Blazor's SignalR hub (`/_blazor`) and Electron's hub (`/electron-hub`) run side-by-side

## Key Files

- **Program.cs** - Application startup with Electron and SignalR configuration
- **launchSettings.json** - Sets `ELECTRON_USE_SIGNALR=true` environment variable
- **ElectronNET.Samples.BlazorSignalR.csproj** - Project configuration with Electron.NET references

## Code Highlights

### Program.cs

```csharp
// Configure Electron.NET with SignalR mode
builder.WebHost.UseElectron(args, async () =>
{
    var options = new BrowserWindowOptions
    {
        Show = false,
        Width = 1200,
        Height = 800,
        IsRunningBlazor = true, // Crucial for Blazor support
    };

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

// ... configure services ...

// Map the Electron SignalR hub (required for SignalR mode)
app.MapElectronHub();
```

### launchSettings.json

```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ELECTRON_USE_SIGNALR": "true"
  },
  "applicationUrl": "http://localhost:0"
}
```

## Differences from Socket.io Mode

| Aspect | Socket.io Mode | SignalR Mode |
|--------|---------------|--------------|
| Communication | socket.io | SignalR |
| Port Selection | Fixed port found by portscanner | Dynamic (port 0) |
| Startup Order | Electron → .NET or .NET → Electron | .NET → Electron |
| Hub Endpoint | N/A (socket.io on separate port) | `/electron-hub` |
| Environment Variable | None | `ELECTRON_USE_SIGNALR=true` |

## Console Output

When running successfully, you should see:

```
[SignalR Sample] Application configured and starting...
[RuntimeControllerAspNetDotnetFirstSignalR] StartCore
[RuntimeControllerAspNetDotnetFirstSignalR] URL: http://localhost:54321
[RuntimeControllerAspNetDotnetFirstSignalR] Launching: --dotnetpackedsignalr --electronUrl=http://localhost:54321
[RuntimeControllerAspNetDotnetFirstSignalR] Electron ready
[SignalRBridge] Connecting to http://localhost:54321/electron-hub
[ElectronHub] Client connected: abc123xyz
[SignalRBridge] Connected successfully
[RuntimeControllerAspNetDotnetFirstSignalR] SignalR connected!
[SignalR Sample] Electron app ready callback executed!
[SignalR Sample] Window ready and visible!
```

## Troubleshooting

### Window doesn't appear

- Check that `ELECTRON_USE_SIGNALR` environment variable is set
- Look for errors in the console output
- Verify `app.MapElectronHub()` is called in Program.cs

### Connection fails

- Ensure SignalR hub is properly mapped
- Check firewall settings
- Verify Node.js and npm packages are installed (`npm install` in ElectronNET.Host)

### Hot Reload issues

- SignalR supports automatic reconnection
- If issues persist, restart the application

## Next Steps

- Modify the Blazor components in `Components/Pages/`
- Add more Electron API calls in the `ElectronAppReady` callback
- Explore bidirectional communication between Blazor and Electron

## Learn More

- [SignalR Startup Mode Documentation](../../docs/SignalR-Startup-Mode.md)
- [Electron.NET Documentation](https://github.com/ElectronNET/Electron.NET/wiki)
- [ASP.NET Core SignalR](https://docs.microsoft.com/aspnet/core/signalr)

## License

MIT - Same as Electron.NET
