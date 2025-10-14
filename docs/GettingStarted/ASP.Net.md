

# ASP.NET Core Setup

ASP.NET Core remains the recommended approach for complex web applications with ElectronNET.Core, providing all the benefits of the ASP.NET ecosystem along with enhanced Electron integration.

## 🛠 System Requirements

### Required Software
- **.NET 8.0** or later
- **Node.js 22.x** or later ([Download here](https://nodejs.org))
- **Visual Studio 2022** (recommended) or other .NET IDE

### Supported Operating Systems
- **Windows 10/11** (x64, ARM64)
- **macOS 11+** (Intel, Apple Silicon)
- **Linux** (most distributions with glibc 2.31+)

> **Note**: For Linux development on Windows, install [WSL2](https://docs.microsoft.com/windows/wsl/install) to build and debug Linux packages.

## 🚀 Quick Start

### 1. Create ASP.NET Core Project

Create a new ASP.NET Core Web App in Visual Studio:

```bash
dotnet new webapp -n MyElectronWebApp
cd MyElectronWebApp
```

### 2. Install NuGet Packages

```powershell
PM> Install-Package ElectronNET.Core
PM> Install-Package ElectronNET.Core.AspNet
```

> **Note**: `ElectronNET.Core.AspNet` provides ASP.NET-specific runtime components and should be used alongside `ElectronNET.Core`.

### 3. Configure Program.cs

Update your `Program.cs` to enable Electron.NET:

```csharp
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Enable Electron.NET with callback for UI setup
builder.WebHost.UseElectron(args, ElectronAppReady);

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Electron initialization callback
async Task ElectronAppReady()
{
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

    // Load your ASP.NET application
    await browserWindow.WebContents.LoadURLAsync("https://localhost:7001");

    browserWindow.OnReadyToShow += () => browserWindow.Show();
}
```

### 4. Alternative: IWebHostBuilder Setup

For projects using the traditional `Startup.cs` pattern:

```csharp
public static void Main(string[] args)
{
    CreateWebHostBuilder(args).Build().Run();
}

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseElectron(args, ElectronAppReady)
        .UseStartup<Startup>();

// Electron callback (same as above)
async Task ElectronAppReady()
{
    var browserWindow = await Electron.WindowManager.CreateWindowAsync(
        new BrowserWindowOptions { Show = false });

    await browserWindow.WebContents.LoadURLAsync("https://localhost:5001");
    browserWindow.OnReadyToShow += () => browserWindow.Show();
}
```

## 🔧 Configuration

### Project File Settings

Configure Electron.NET through MSBuild properties in your `.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <ElectronNETCoreDescription>My ASP.NET Electron App</ElectronNETCoreDescription>
  <ElectronNETCoreDisplayName>MyApp</ElectronNETCoreDisplayName>
</PropertyGroup>
```



## 🎨 Customization

### Window Configuration

Customize the main window appearance:

```csharp
var options = new BrowserWindowOptions
{
    Width = 1400,
    Height = 900,
    MinWidth = 800,
    MinHeight = 600,
    Frame = true,
    TitleBarStyle = TitleBarStyle.Default,
    Icon = "wwwroot/favicon.ico"
};
```

### Multiple Windows

Create additional windows for different parts of your application:

```csharp
var settingsWindow = await Electron.WindowManager.CreateWindowAsync(
    new BrowserWindowOptions
    {
        Width = 600,
        Height = 400,
        Parent = browserWindow,
        Modal = true
    },
    "https://localhost:7001/settings");
```

## 🚀 Next Steps

- **[Debugging](Debugging.md)** - Learn about ASP.NET debugging features
- **[Package Building](Package-Building.md)** - Create distributable packages
- **[Startup Methods](Startup-Methods.md)** - Understanding launch scenarios

## 💡 Benefits of ASP.NET + Electron

✅ **Full Web Stack** - Use MVC, Razor Pages, Blazor, and all ASP.NET features
✅ **Hot Reload** - Edit ASP.NET code and see changes instantly
✅ **Rich Ecosystem** - Access to thousands of ASP.NET packages
✅ **Modern Development** - Latest C# features and ASP.NET patterns
✅ **Scalable Architecture** - Build complex, maintainable applications
