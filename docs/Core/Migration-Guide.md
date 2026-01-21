# Migration Guide

Migrating from previous versions of Electron.NET to ElectronNET.Core is straightforward but requires several important changes. This guide walks you through the process step by step.

## 📋 Prerequisites

Before starting the migration:

- **Backup your project** - Ensure you have a working backup
- **Update development tools** - Install Node.js 22.x and .NET 8.0+
- **Review current setup** - Note your current Electron and ASP.NET versions

## 🚀 Migration Steps

### Step 1: Update NuGet Packages

**Uninstall old packages:**
```powershell
dotnet remove package  ElectronNET.API
```

**Install new packages:**
```powershell
dotnet add package ElectronNET.Core
dotnet add package ElectronNET.Core.AspNet  # For ASP.NET projects
```

> **Note**: The API package is automatically included as a dependency of `ElectronNET.Core`. See [Package Description](../RelInfo/Package-Description.md) for details about the package structure.


### Step 2: Configure Project Settings

**Auto-generated Configuration:**  
ElectronNET.Core automatically creates `electron-builder.json` in the `Properties` folder of your project during the first build or NuGet restore. No manual configuration is needed for basic setups.

**Migrate Existing Configuration:**  
If you have an existing `electron.manifest.json` file:

1. **Open the generated `electron-builder.json`** file in your project
2. **Locate the 'build' section** in your old `electron.manifest.json`
3. **Copy the contents** of the build section (not the "build" key itself) into the new `electron-builder.json`
4. **Use Visual Studio Project Designer** to configure Electron settings through the UI
5. **Delete the old `electron.manifest.json`** file

**Alternative: Manual Configuration**
You can also manually edit `electron-builder.json`:

```json
{
  "linux": {
    "target": [
      "tar.xz"
    ]
  },
  "win": {
    "target": [
      {
        "target": "portable",
        "arch": "x64"
      }
    ]
  }
}
```

**Modify Launch Settings:**
ElectronNET.Core no longer needs a separate CLI tool (electronize.exe) for launching. You should update your launch settings to use either the ASP.NET-first or Electron-first approach. See [Debugging](../Using/Debugging.md) for details.

## 🎯 Testing Migration

After completing the migration steps:

1. **Build your project** to ensure no compilation errors
2. **Test debugging** using the new ASP.NET-first approach
3. **Verify packaging** works with the new configuration
4. **Check cross-platform builds** if targeting multiple platforms

## 🚨 Common Migration Issues

### Build Errors
- **Node.js version**: Verify Node.js 22.x is installed and in PATH
- **Package conflicts**: Clean NuGet cache if needed

### Runtime Errors
- **Missing electron-builder.json**: Trigger rebuild or manual NuGet restore
- **Process termination**: Use .NET-first startup mode for better cleanup

## 🚀 Next Steps

- **[What's New?](What's-New.md)** - Complete overview of ElectronNET.Core features
- **[Advanced Migration Topics](Advanced-Migration-Topics.md)** - Handle complex scenarios
- **[Getting Started](../GettingStarted/ASP.Net.md)** - Learn about new development workflows

## 💡 Migration Benefits

✅ **Simplified Configuration** - No more CLI tools or JSON files  
✅ **Better Debugging** - Native Visual Studio experience with Hot Reload  
✅ **Modern Architecture** - .NET-first process lifecycle  
✅ **Cross-Platform Ready** - Build Linux apps from Windows  
✅ **Future-Proof** - Flexible Electron version selection  

### Step 3: Update Startup Code

**Update UseElectron() calls** to include the new callback parameter. This callback executes at the right moment to initialize your Electron UI.

#### Modern ASP.NET Core (WebApplication)

```csharp
using ElectronNET.API;
using ElectronNET.API.Entities;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.UseElectron(args, ElectronAppReady);

        var app = builder.Build();
        app.Run();
    }

    public static async Task ElectronAppReady()
    {
        var browserWindow = await Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions { Show = false });

        browserWindow.OnReadyToShow += () => browserWindow.Show();
    }
```

#### Traditional ASP.NET Core (IWebHostBuilder)

```csharp	
using ElectronNET.API;
using ElectronNET.API.Entities;

    public static void Main(string[] args)
    {
        WebHost.CreateDefaultBuilder(args)
            .UseElectron(args, ElectronAppReady)
            .UseStartup<Startup>()
            .Build()
            .Run();
    }

    public static async Task ElectronAppReady()
    {
        var browserWindow = await Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions { Show = false });

        browserWindow.OnReadyToShow += () => browserWindow.Show();
    }
```


### Step 4: Update Development Tools

See [System Requirements](../GettingStarted/System-Requirements.md).

### Step 5: Update Debugging Setup

**Watch Feature Removal:**

The old 'watch' feature is no longer supported. Instead, use the new ASP.NET-first debugging with Hot Reload:

- **Old approach**: Manual process attachment and slow refresh
- **New approach**: Native Visual Studio debugging with Hot Reload
- **Benefits**: Faster development cycle, better debugging experience

**Update Launch Settings:**

Replace old watch configurations with new debugging profiles. See [Debugging](../GettingStarted/Debugging.md) for detailed setup instructions.

