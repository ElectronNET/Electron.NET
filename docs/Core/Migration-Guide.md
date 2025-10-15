# Migration Guide

// Explain migration steps:

Uninstall existing package ElectronNET.API

// Install new packages:


```ps1
PM> Install-Package ElectronNET.Core

PM> Install-Package ElectronNET.Core.AspNet
```

// add link to package type description: [text](../Releases/Package-Description.md)
// the API package is a dependency of .Core, so it's auto-incldued


// Edit Properties\electron-builder.json
// it's auto-created: In VS after nuget restore, otherwise on first build - even when that fails

// Now look at the electron-manifest.json file
// 1. Manually merge everything under the 'build' property into the 
// electron-builder file (omitting the build node, only its content is to be merged)
// 2. Open the project designer in VS and enter the values from the manifest file into the fields
// 3. Delete the manifest file
// 

// Check ASP.Net core startup (program.cs or statup.cs, typically)
// Find the UseElectron() extension method call
// it will have an error. it needs a 3rd parameter now: the onAppReady callback. 
// set this to a callback function. this gets called just in the right moment for you  
// to start things going (like accessing ElectronNET classes)

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


// Also show an example for the other case with IWebHostBuilder and Startup class



// Next, explain that the 'watch' feature is no longer supported, now that proper debugging with hot reload is possible.


// Nodejs needs to be updated to 22.x
// Important. Explain how to (for win and linux)

