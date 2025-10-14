# Debugging

ElectronNET.Core transforms the debugging experience by providing native Visual Studio integration with multiple debugging modes. No more complex setup or manual process attachment—debugging now works as expected for .NET developers.

## 🎯 Debugging Modes

ElectronNET.Core supports three main debugging approaches, all configured through Visual Studio's launch profiles:

### 1. ASP.NET-First Debugging (Recommended)

Debug your .NET code directly with full Hot Reload support:

```json
{
  "profiles": {
    "ASP.Net (unpackaged)": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:7001/"
    }
  }
}
```

**Benefits:**
- ✅ Full C# debugging with breakpoints
- ✅ Hot Reload for ASP.NET code
- ✅ Edit-and-continue functionality
- ✅ Native Visual Studio debugging experience

### 2. Electron-First Debugging

Debug the Electron process when you need to inspect native Electron APIs:

```json
{
  "profiles": {
    "Electron (unpackaged)": {
      "commandName": "Executable",
      "executablePath": "node",
      "commandLineArgs": "node_modules/electron/cli.js main.js -unpackedelectron",
      "workingDirectory": "$(TargetDir).electron",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

**Benefits:**
- ✅ Debug Electron main process
- ✅ Inspect native Electron APIs
- ✅ Node.js debugging capabilities

### 3. Cross-Platform WSL Debugging

Debug Linux builds directly from Windows Visual Studio:

```json
{
  "profiles": {
    "WSL": {
      "commandName": "WSL2",
      "launchUrl": "http://localhost:7001/",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:7001/"
      },
      "distributionName": ""
    }
  }
}
```

**Benefits:**
- ✅ Debug Linux applications from Windows
- ✅ Test Linux-specific behavior
- ✅ Validate cross-platform compatibility

## 🔧 Setup Instructions

### 1. Configure Launch Settings

Add the debugging profiles to `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "ASP.Net (unpackaged)": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:7001/"
    },
    "Electron (unpackaged)": {
      "commandName": "Executable",
      "executablePath": "node",
      "commandLineArgs": "node_modules/electron/cli.js main.js -unpackedelectron",
      "workingDirectory": "$(TargetDir).electron",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "WSL": {
      "commandName": "WSL2",
      "launchUrl": "http://localhost:7001/",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:7001/"
      },
      "distributionName": ""
    }
  }
}
```

### 2. Switch Runtime Identifiers

When switching between Windows and WSL debugging:

1. **Right-click your project** in Solution Explorer
2. **Select "Edit Project File"**
3. **Update the RuntimeIdentifier**:

```xml
<!-- For Windows debugging -->
<RuntimeIdentifier>win-x64</RuntimeIdentifier>

<!-- For WSL/Linux debugging -->
<RuntimeIdentifier>linux-x64</RuntimeIdentifier>
```

### 3. Enable WSL Debugging

For WSL debugging, ensure:

- **WSL2 is installed** and configured
- **Linux distribution** is set in the launch profile
- **Project targets Linux RID** for WSL debugging

## 🚀 Debugging Workflow

### ASP.NET-First Debugging (Default)

1. **Select "ASP.Net (unpackaged)"** profile in Visual Studio
2. **Press F5** to start debugging
3. **Set breakpoints** in your C# code
4. **Use Hot Reload** to edit ASP.NET code during runtime
5. **Stop debugging** when finished

### Electron Process Debugging

1. **Select "Electron (unpackaged)"** profile
2. **Press F5** to start debugging
3. **Attach to Electron process** if needed
4. **Debug Node.js and Electron APIs**

### Cross-Platform Debugging

1. **Set RuntimeIdentifier** to `linux-x64`
2. **Select "WSL"** profile
3. **Press F5** to debug in WSL
4. **Test Linux-specific behavior**

## 🔍 Debugging Tips

### Hot Reload

- **Works with ASP.NET-first debugging**
- **Edit Razor views, controllers, and pages**
- **See changes instantly** without restart
- **Preserves application state**

### Breakpoint Debugging

```csharp
// Set breakpoints here
public async Task<IActionResult> Index()
{
    var data = await GetData(); // ← Breakpoint
    return View(data);
}
```

### Process Management

- **ASP.NET-first mode** automatically manages Electron process lifecycle
- **Proper cleanup** on debugging session end
- **No manual process killing** required

## 🛠 Troubleshooting

### Common Issues

**"Electron process not found"**
- Ensure Node.js 22.x is installed
- Check that packages are restored (`dotnet restore`)
- Verify RuntimeIdentifier matches your target platform

**"WSL debugging fails"**
- Install and configure WSL2
- Ensure Linux distribution is properly set up
- Check that project targets correct RID

**"Hot Reload not working"**
- Use ASP.NET-first debugging profile
- Ensure ASPNETCORE_ENVIRONMENT=Development
- Check for compilation errors

## 🎨 Visual Debugging

*Placeholder for image showing Visual Studio debugging interface with Electron.NET*

The debugging interface provides familiar Visual Studio tools:
- **Locals and Watch windows** for variable inspection
- **Call Stack** for method call tracing
- **Immediate Window** for runtime evaluation
- **Hot Reload** indicator for edit-and-continue

## 🚀 Next Steps

- **[Startup Methods](Startup-Methods.md)** - Understanding different launch scenarios
- **[Package Building](Package-Building.md)** - Debug packaged applications
- **[Migration Guide](../Core/Migration-Guide.md)** - Moving from old debugging workflows

## 💡 Benefits

✅ **Native Visual Studio Experience** - No complex setup or manual attachment
✅ **Hot Reload Support** - Edit ASP.NET code during debugging
✅ **Cross-Platform Debugging** - Debug Linux apps from Windows
✅ **Multiple Debugging Modes** - Choose the right approach for your needs
✅ **Process Lifecycle Management** - Automatic cleanup and proper termination
