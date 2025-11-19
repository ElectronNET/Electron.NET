# Startup Methods

ElectronNET.Core supports multiple startup methods to handle different development and deployment scenarios. The framework automatically detects the appropriate mode based on command-line flags and environment.

## 🎯 Startup Scenarios

The framework supports **8 different launch scenarios** covering every combination of:

- **Packaged vs Unpackaged** deployment
- **Console vs ASP.NET** application types
- **Dotnet-first vs Electron-first** initialization

## 🚀 Command-Line Flags

### Unpackaged Debugging Modes

#### **`-unpackedelectron`** - Electron-first debugging
```bash
# Launch Electron first, which then starts .NET
node node_modules/electron/cli.js main.js -unpackedelectron
```

#### **`-unpackeddotnet`** - .NET-first debugging
```bash
# Launch .NET first, which then starts Electron
dotnet run -unpackeddotnet
```

### Packaged Deployment Modes

#### **`-dotnetpacked`** - .NET-first packaged execution
```bash
# Run packaged app with .NET starting first
MyApp.exe -dotnetpacked
```

#### **No flags** - Electron-first packaged execution (default)
```bash
# Run packaged app with Electron starting first
MyApp.exe
```

## 📋 Startup Method Details

### 1. Unpackaged + Electron-First (Development)
- **Use Case**: Debug Electron main process and Node.js code
- **Command**: `-unpackedelectron` flag
- **Process Flow**:
  1. Electron starts first
  2. Electron launches .NET process
  3. .NET connects back to Electron
  4. Application runs with Electron in control

### 2. Unpackaged + .NET-First (Development)
- **Use Case**: Debug ASP.NET/C# code with Hot Reload
- **Command**: `-unpackeddotnet` flag
- **Process Flow**:
  1. .NET application starts first
  2. .NET launches Electron process
  3. Electron connects back to .NET
  4. Application runs with .NET in control

### 3. Packaged + .NET-First (Production)
- **Use Case**: Deployed application with .NET controlling lifecycle
- **Command**: `-dotnetpacked` flag
- **Process Flow**:
  1. .NET executable starts first
  2. .NET launches Electron from packaged files
  3. Electron loads from app.asar or extracted files
  4. .NET maintains process control

### 4. Packaged + Electron-First (Production)
- **Use Case**: Traditional Electron app behavior
- **Command**: No special flags
- **Process Flow**:
  1. Electron executable starts first
  2. Electron launches .NET from packaged files
  3. .NET runs from Electron's process context
  4. Electron maintains UI control

## 🔧 Configuration Examples

### ASP.NET Application Startup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure for different startup modes
builder.WebHost.UseElectron(args, async () =>
{
    var browserWindow = await Electron.WindowManager.CreateWindowAsync(
        new BrowserWindowOptions { Show = false });

    await browserWindow.WebContents.LoadURLAsync("http://localhost:8001");
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

var app = builder.Build();
app.Run();
```

### Console Application Startup

```csharp
// Program.cs
public static async Task Main(string[] args)
{
    var runtimeController = ElectronNET.Runtime.ElectronHostEnvironment.Current.RuntimeController;

    await runtimeController.Start();
    await runtimeController.WaitReadyTask;

    await InitializeApplication();

    await runtimeController.WaitStoppedTask;
}
```

## 🎨 Visual Process Flow


![Startup Modes](../images/startup_modes.png)

The image above illustrates how each combination of deployment type, application type, and initialization order affects the process lifecycle.

## 🚀 Development Workflows

### Debugging Workflow

**ASP.NET-First Debugging** (Recommended)
```json
// launchSettings.json
{
  "ASP.Net (unpackaged)": {
    "commandName": "Project",
    "commandLineArgs": "-unpackeddotnet"
  }
}
```

**Electron-First Debugging**
```json
// launchSettings.json
{
  "Electron (unpackaged)": {
    "commandName": "Executable",
    "executablePath": "node",
    "commandLineArgs": "node_modules/electron/cli.js main.js -unpackedelectron"
  }
}
```

### Production Deployment

**Dotnet-First Deployment**

```bash
# Build and package
dotnet publish -c Release -r win-x64
cd publish\Release\net8.0\win-x64
npm install
npx electron-builder

# Run with dotnet-first
MyApp.exe -dotnetpacked
```

**Electron-First Deployment** (Default)

```bash
# Run packaged application (no special flags needed)
MyApp.exe
```

## 🔍 Process Lifecycle Management

### Automatic Cleanup

ElectronNET.Core automatically manages process lifecycle:

- **Graceful shutdown** when main window is closed
- **Proper cleanup** of child processes
- **Error handling** for process failures
- **Cross-platform compatibility** for process management

### Manual Control

Access runtime controller for advanced scenarios:

```csharp
var runtime = ElectronNET.Runtime.ElectronHostEnvironment.Current.RuntimeController;

// Wait for Electron to be ready
await runtime.WaitReadyTask;

// Stop Electron runtime
await runtime.Stop();
await runtime.WaitStoppedTask;
```

## 🛠 Troubleshooting

### Common Startup Issues

**"Electron process not found"**
- Ensure Node.js 22.x is installed
- Check that .NET build succeeded
- Verify RuntimeIdentifier is set correctly

**"Port conflicts"**
- Use different ports for different startup modes
- Check that no other instances are running
- Verify firewall settings

**"Process won't terminate"**
- Use dotnet-first mode for better cleanup
- Check for unhandled exceptions
- Verify all windows are properly closed

## 💡 Best Practices

### Choose the Right Mode

- **Development**: Use .NET-first for C# debugging, Electron-first for Node.js debugging
- **Production**: Use .NET-first for better process control, Electron-first for traditional behavior
- **Cross-platform**: Use .NET-first for consistent behavior across platforms

### Environment Configuration

```xml
<!-- .csproj -->
<PropertyGroup>
  <ElectronNETCoreEnvironment>Production</ElectronNETCoreEnvironment>
</PropertyGroup>
```

## 🚀 Next Steps

- **[Debugging](Debugging.md)** - Debug different startup modes
- **[Package Building](Package-Building.md)** - Package for different deployment scenarios
- **[Migration Guide](../Core/Migration-Guide.md)** - Update existing apps for new startup methods

## 🎯 Summary

The flexible startup system ensures ElectronNET.Core works optimally in every scenario while providing the control and debugging experience .NET developers expect. Choose the appropriate mode based on your development workflow and deployment requirements.
