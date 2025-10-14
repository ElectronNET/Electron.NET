

# Console Application Setup

One of the most significant breakthroughs in ElectronNET.Core is the ability to build Electron applications using simple console applications instead of requiring ASP.NET Core. This removes a major barrier and enables many more use cases.

## 🎯 What You Can Build

Console applications with ElectronNET.Core support multiple content scenarios:

- **File System HTML/JS** - Serve static web content directly from the file system
- **Remote Server Integration** - Connect to existing web servers or APIs
- **Lightweight Architecture** - Avoid ASP.NET overhead when not needed
- **Simplified Deployment** - Package and distribute with minimal dependencies

## 📋 Prerequisites

Before starting, ensure you have:

- **.NET 8.0** or later
- **Node.js 22.x** or later
- **Visual Studio 2022** (recommended) or Visual Studio Code

## 🚀 Quick Start

### 1. Create Console Application

Create a new Console Application project in Visual Studio:

```bash
dotnet new console -n MyElectronApp
cd MyElectronApp
```

### 2. Install NuGet Packages

```powershell
PM> Install-Package ElectronNET.Core
```

> **Note**: The API package is automatically included as a dependency of `ElectronNET.Core`.

### 3. Configure Project File

Add the Electron.NET configuration to your `.csproj` file:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net8.0</TargetFramework>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ElectronNET.Core" Version="1.0.0" />
</ItemGroup>
```

### 4. Implement Basic Structure

Here's a complete console application example:

```csharp
using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace MyElectronApp
{
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
                await runtimeController.WaitStoppedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await runtimeController.Stop();
                await runtimeController.WaitStoppedTask.WaitAsync(TimeSpan.FromSeconds(2));
            }
        }

        private static async Task InitializeApp()
        {
            // Create main window
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions
                {
                    Width = 1200,
                    Height = 800,
                    Show = false,
                    WebPreferences = new WebPreferences
                    {
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
}
```

## 📁 Content Sources

### File System Content

Serve HTML/JS files from your project:

```csharp
// In your project root, create wwwroot/index.html
await browserWindow.WebContents.LoadFileAsync("wwwroot/index.html");
```

### Remote Content

Load content from any web server:

```csharp
await browserWindow.WebContents.LoadURLAsync("https://your-server.com/app");
```

### Development Server

For development, you can run a simple HTTP server:

```csharp
// Add this for development
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    await browserWindow.WebContents.LoadURLAsync("http://localhost:3000");
}
```

## 🔧 Configuration Options

### Project Configuration

Configure Electron settings through MSBuild properties in your `.csproj`:

```xml
<PropertyGroup>
  <ElectronNETCoreDescription>My Electron App</ElectronNETCoreDescription>
  <ElectronNETCoreDisplayName>MyApp</ElectronNETCoreDisplayName>
  <ElectronNETCoreAuthorName>Your Name</ElectronNETCoreAuthorName>
</PropertyGroup>
```

### Runtime Configuration

Access configuration at runtime:

```csharp
var app = await Electron.App.GetAppAsync();
Console.WriteLine($"App Name: {app.Name}");
```

## 🎨 Customization

### Window Options

Customize your main window:

```csharp
var options = new BrowserWindowOptions
{
    Width = 1400,
    Height = 900,
    MinWidth = 800,
    MinHeight = 600,
    Frame = true,
    Title = "My Custom App",
    Icon = "assets/app-icon.png"
};
```

### Multiple Windows

Create additional windows as needed:

```csharp
var settingsWindow = await Electron.WindowManager.CreateWindowAsync(
    new BrowserWindowOptions { Width = 600, Height = 400, Modal = true },
    "app://settings.html");
```

## 🚀 Next Steps

- **[Debugging](Debugging.md)** - Learn about debugging console applications
- **[Package Building](Package-Building.md)** - Create distributable packages
- **[Migration Guide](../Core/Migration-Guide.md)** - Moving from ASP.NET projects

## 💡 Benefits of Console Apps

✅ **Simpler Architecture** - No ASP.NET complexity when not needed
✅ **Flexible Content** - Use any HTML/JS source
✅ **Faster Development** - Less overhead for simple applications
✅ **Easy Deployment** - Minimal dependencies
✅ **Better Performance** - Lighter weight than full web applications
