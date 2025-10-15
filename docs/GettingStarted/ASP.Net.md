

## 🛠 Requirements to Run

 Our API uses .NET 6/8, so our 

Also you should have installed:

* .NET 6/8 or later
* OS
  minimum base OS is the same as [.NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md) / [.NET 8](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md).
* NodeJS (at least [Version 22.x](https://nodejs.org))


## 👩‍🏫 Usage with ASP.Net

- Create a new ASP.Net Core project
- Install the following two nuget packages:

```ps1
PM> Install-Package ElectronNET.Core

PM> Install-Package ElectronNET.Core.AspNet
```

### Enable Electron.NET on Startup

To do so, use the `UseElectron` extension method on a `WebApplicationBuilder`, an `IWebHostBuilder` or any descendants.

> [!NOTE]  
> New in Electron.NET Core is that you provide a callback method as an argument to `UseElectron()`, which ensures that you get to know the right moment to set up your application UI.

### Program.cs

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


