

# Console Application Setup

A major benefit in ElectronNET.Core is the ability to build Electron applications using simple console applications instead of requiring ASP.NET Core. This removes a significant barrier and enables many more use cases.

## 🎯 What You Can Build

Console applications with ElectronNET.Core support multiple content scenarios:

- **File System HTML/JS** - Serve static web content directly from the file system
- **Remote Server Integration** - Connect to existing web servers or APIs
- **Lightweight Architecture** - Avoid ASP.NET overhead when not needed
- **Simplified Deployment** - Package and distribute with minimal dependencies

## 📋 Prerequisites

See [System Requirements](../GettingStarted/System-Requirements.md).


## 🚀 Quick Start

### 1. Create Console Application

#### Visual Studio

Create a new console application in Visual Studio by selecting **New Project** and choosing one of the project templates for console apps.

#### From the command line

```bash
dotnet new console -n MyElectronApp
cd MyElectronApp
```

### 2. Install NuGet Packages

```powershell
dotnet add package ElectronNET.Core
```

> [!Note]  
> The API package is automatically included as a dependency of `ElectronNET.Core`.

### 3. Configure Project File

Add the Electron.NET configuration to your `.csproj` file:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0</TargetFramework>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ElectronNET.Core" Version="0.3.0" />
</ItemGroup>
```

> [!WARNING]  
> Specifying `OutputType` property is crucial in order to get the ability of WSL debugging. Especially it is not included in ASP.NET projects.  
> When you migrate from ASP.NET to a console application, be sure to add this to the project file.


### 4. Implement Basic Structure

Here's a complete console application example:

```csharp
using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace MyElectronApp

public class Program
{
    public static async Task Main(string[] args)
    {
        var runtimeController = ElectronNetRuntime.RuntimeController;

        try
        {
            // Start Electron runtime
            await runtimeController.Start();
            await runtimeController.WaitReadyTask;

            // Initialize your Electron app
            await InitializeApp();

            // Wait for shutdown
            await runtimeController.WaitStoppedTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await runtimeController.Stop().ConfigureAwait(false);
            await runtimeController.WaitStoppedTask.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        }
    }

    private static async Task InitializeApp()
    {
        // Create main window
        var browserWindow = await Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions
            {
                Show = false,
                WebPreferences = new WebPreferences
                {
                    // Add these two when using file:// URLs
                    WebSecurity = false,
                    AllowRunningInsecureContent = true,

                    NodeIntegration = false,
                    ContextIsolation = true
                }
            });

        // Load your content (file system, remote URL, etc.)
        await browserWindow.WebContents.LoadURLAsync("https://example.com");

        // Show window when ready
        browserWindow.OnReadyToShow += () => browserWindow.Show();
    }
}
```

## 📁 Content Sources

### File System Content

Serve HTML/JS files from your project:

```csharp
// In your project root, create wwwroot/index.html
var fileInfo = new FileInfo(Environment.ProcessPath);
var exeFolder = fileInfo.DirectoryName;
var htmlPath = Path.Combine(exeFolder, "wwwroot/index.html");
var url = new Uri(htmlPath, UriKind.Absolute);
await browserWindow.WebContents.LoadFileAsync(url.ToString());
```

### Remote Content

Load content from any web server:

```csharp
await browserWindow.WebContents.LoadURLAsync("https://your-server.com/app");
```


## 🚀 Next Steps

- **[Debugging](../Using/Debugging.md)** - Learn about debugging console applications
- **[Package Building](../Using/Package-Building.md)** - Create distributable packages
- **[Migration Guide](../Core/Migration-Guide.md)** - Moving from ASP.NET projects

## 💡 Benefits of Console Apps

✅ **Simpler Architecture** - No ASP.NET complexity when not needed  
✅ **Flexible Content** - Use any HTML/JS source  
✅ **Faster Development** - Less overhead for simple applications  
✅ **Easy Deployment** - Minimal dependencies  
✅ **Better Performance** - Lighter weight than full web applications  
