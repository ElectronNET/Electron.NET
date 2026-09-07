
# ASP.NET Core Setup

ASP.NET Core remains the recommended approach for complex web applications with ElectronNET.Core, providing all the benefits of the ASP.NET ecosystem along with enhanced Electron integration.

## 🛠 System Requirements

See [System Requirements](../GettingStarted/System-Requirements.md).


## 🚀 Quick Start

> [!Tip]
> To skip the manual setup below, scaffold a ready-to-run app with the
> [project templates](Templates.md): `dotnet new install ElectronNET.Core.Templates` followed by
> `dotnet new electron-blazor -n MyDesktopApp`.

### 1. Create ASP.NET Core Project

#### Visual Studio

Create a new ASP.NET Core Web App in Visual Studio by selecting **New Project** and choosing one of the ASP.NET Core project templates.

#### From the command line

```bash
dotnet new webapp -n MyElectronWebApp
cd MyElectronWebApp
```

### 2. Install NuGet Packages

#### Visual Studio

Right-click the solution and select **Manage Nuget Packages**. 
Finr and install `ElectronNET.Core` as well as `ElectronNET.Core.AspNet`.

#### From the command line

```powershell
dotnet add package ElectronNET.Core
dotnet add package ElectronNET.Core.AspNet
```

> [!Note]  
> The API package is automatically included as a dependency of `ElectronNET.Core`.


### 3. Configure Program.cs

Update your `Program.cs` to enable Electron.NET:

```csharp
using ElectronNET.API;
using ElectronNET.API.Entities;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();

        // Add this line to enable Electron.NET:
        builder.UseElectron(args, ElectronAppReady);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    public static async Task ElectronAppReady()
    {
        var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false });

        browserWindow.OnReadyToShow += () => browserWindow.Show();
    }
}
```

#### ASP.NET Port

If you want to launch a specific URL, you can retrieve the actual ASP.NET port from the new `ElectronNetRuntime` static class, for example:

```csharp
    await browserWindow.WebContents
        .LoadURLAsync($"http://localhost:{ElectronNetRuntime.AspNetWebPort}/mypage.html");
```

### 4. Alternative: IWebHostBuilder Setup

For projects using the traditional `Startup.cs` pattern, please see "Traditional ASP.NET Core" in the [Migration Guide](../Core/Migration-Guide.md).


### 5. Dependency Injection

ElectronNET.API can be added to your DI container within the `Startup` class. All of the modules available in Electron will be added as Singletons.

```csharp
using ElectronNET.API;

public void ConfigureServices(IServiceCollection services)
{
    services.AddElectron();
}
```


## 🚀 Next Steps

- **[Debugging](../Using/Debugging.md)** - Learn about ASP.NET debugging features
- **[Package Building](../Using/Package-Building.md)** - Create distributable packages
- **[Startup Methods](../Using/Startup-Methods.md)** - Understanding launch scenarios

## 💡 Benefits of ASP.NET + Electron

✅ **Full Web Stack** - Use MVC, Razor Pages, Blazor, and all ASP.NET features  
✅ **Hot Reload** - Edit ASP.NET code and see changes instantly  
✅ **Rich Ecosystem** - Access to thousands of ASP.NET packages  
✅ **Modern Development** - Latest C# features and ASP.NET patterns  
✅ **Scalable Architecture** - Build complex, maintainable applications  
