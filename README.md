[![Electron.NET Logo](https://github.com/ElectronNET/Electron.NET/raw/main/assets/images/electron.net-logo.png)](https://github.com/ElectronNET/Electron.NET)

[![donate](https://img.shields.io/badge/Donate-Donorbox-green.svg)](https://donorbox.org/electron-net) [![Gitter](https://badges.gitter.im/ElectronNET/community.svg)](https://gitter.im/ElectronNET/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge) [![Build status](https://github.com/ElectronNET/Electron.NET/actions/workflows/ci.yml/badge.svg)](https://github.com/ElectronNET/Electron.NET/actions/workflows/ci.yml)

# Electron.Net Core is here!

## A Complete Transformation

ElectronNET.Core represents a fundamental modernization of Electron.NET, addressing years of accumulated pain points while preserving full API compatibility. This isn't just an update—it's a complete rethinking of how .NET developers build and debug cross-platform desktop applications with Electron.

Read more: [**What's New in ElectronNET.Core**](wiki/What's-New)


Build cross platform desktop applications with .NET 6/8 - from console apps to ASP.Net Core (Razor Pages, MVC) to Blazor


## Wait - how does that work exactly?

Well... there are lots of different approaches how to get a X-plat desktop app running. Electron.NET provides a range of  ways to build .NET based solutions using Electron at the side of presentation. While the classic Electron.Net setup,  using an ASP.Net host ran by the Electron side is still the primary way, there's more flexibility now: both, dotnet and Electron are now able to launch the other for better lifetime management, and when you don't need a local web server - like when running content from files or remote servers, you can  drop the ASP.Net stack altogether and got with a lightweight console app instead.

## 📦 NuGet

[![NuGet](https://img.shields.io/nuget/v/ElectronNET.Core.svg?style=flat-square) ElectronNET.Core ](https://www.nuget.org/packages/ElectronNET.Core.API/)  |  [![NuGet](https://img.shields.io/nuget/v/ElectronNET.Core.API.svg?style=flat-square) ElectronNET.Core.API ](https://www.nuget.org/packages/ElectronNET.Core.API/)  | [![NuGet](https://img.shields.io/nuget/v/ElectronNET.Core.AspNet.svg?style=flat-square) ElectronNET.Core.AspNet ](https://www.nuget.org/packages/ElectronNET.Core.AspNet/)


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
dotnet add package ElectronNET.Core

dotnet add package ElectronNET.Core.AspNet
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


## 🚀 Starting and Debugging the Application

Just press F5 in Visual Studio or use dotnet for debugging.


## 📔 Usage of the Electron API

A complete documentation is available on the Wiki.

In this YouTube video, we show you how you can create a new project, use the Electron.NET API, debug a application and build an executable desktop app for Windows: [Electron.NET - Getting Started](https://www.youtube.com/watch?v=nuM6AojRFHk)  
  
  > [!NOTE]  
  > The video hasn't been updated for the changes in ElectronNET.Core, so it is partially outdated.



## 👨‍💻 Authors

* **[Gregor Biswanger](https://github.com/GregorBiswanger)** - (Microsoft MVP, Intel Black Belt and Intel Software Innovator) is a freelance lecturer, consultant, trainer, author and speaker. He is a consultant for large and medium-sized companies, organizations and agencies for software architecture, web- and cross-platform development. You can find Gregor often on the road attending or speaking at international conferences. - [Cross-Platform-Blog](http://www.cross-platform-blog.com) - Twitter [@BFreakout](https://www.twitter.com/BFreakout)  
* **[Dr. Florian Rappl](https://github.com/FlorianRappl)** - Software Developer - from Munich, Germany. Microsoft MVP & Web Geek. - [The Art of Micro Frontends](https://microfrontends.art) - [Homepage](https://florian-rappl.de) - Twitter [@florianrappl](https://twitter.com/florianrappl)
* [**softworkz**](https://github.com/softworkz) - full range developer - likes to start where others gave up - MS MVP alumni and Munich citizen as well
* **[Robert Muehsig](https://github.com/robertmuehsig)** - Software Developer - from Dresden, Germany, now living & working in Switzerland. Microsoft MVP & Web Geek. - [codeinside Blog](https://blog.codeinside.eu) - Twitter [@robert0muehsig](https://twitter.com/robert0muehsig)  
  
See also the list of [contributors](https://github.com/ElectronNET/Electron.NET/graphs/contributors) who participated in this project.
  
## 🙋‍♀️🙋‍♂ Contributing

Feel free to submit a pull request if you find any bugs (to see a list of active issues, visit the [Issues section](https://github.com/ElectronNET/Electron.NET/issues).
Please make sure all commits are properly documented.


## 🙏 Donate

We do this open source work in our free time. If you'd like us to invest more time on it, please [donate](https://donorbox.org/electron-net). Donation can be used to increase some issue priority. Thank you!

[![donate](https://img.shields.io/badge/Donate-Donorbox-green.svg)](https://donorbox.org/electron-net)

Alternatively, consider using a GitHub sponsorship for the core maintainers:

- [Gregor Biswanger](https://github.com/sponsors/GregorBiswanger)
- [Florian Rappl](https://github.com/sponsors/FlorianRappl)

Any support appreciated! 🍻

## 🎉 License

MIT-licensed. See [LICENSE](./LICENSE) for details.

**Enjoy!**
    


