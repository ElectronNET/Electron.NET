[![Electron.NET Logo](https://github.com/ElectronNET/Electron.NET/blob/master/assets/images/electron.net-logo.png)](https://github.com/ElectronNET/Electron.NET)


AppVeyor (Win/Linux): [![Build status](https://ci.appveyor.com/api/projects/status/q95h4xt14papwi05/branch/master?svg=true)](https://ci.appveyor.com/project/robertmuehsig/electron-net/branch/master)

* Checkout AppVeyor Aritfacts: Contains the WebApp sample builded for Windows & Linux!

Travis-CI (Win/macOS/Linux): [![Build Status](https://travis-ci.org/ElectronNET/Electron.NET.svg?branch=master)](https://travis-ci.org/ElectronNET/Electron.NET)

Build cross platform desktop apps with .NET Core 2.0 and ASP.NET NET Core. 

Electron.NET is a __wrapper__ around a "normal" Electron application with a embedded ASP.NET Core application. Via our Electron.NET IPC bridge we can invoke Electron APIs from .NET.

The CLI extensions hosts our toolset to build and start Electron.NET applications.

## Wait - you host a .NET Core app inside Electron? Why?

Well... there are lots of different approaches how to get a X-plat desktop app running. We thought it would be nice for .NET devs to use the ASP.NET Core environment and just embed it inside a pretty robust X-plat enviroment called Electron. Porting Electron to .NET is not a goal of this project, at least we don't have any clue how to do it. We just combine ASP.NET Core & Electron. 

# NuGet:

* API [![NuGet](https://img.shields.io/nuget/v/ElectronNET.API.svg?style=flat-square)](https://www.nuget.org/packages/ElectronNET.API/) 
* CLI [![NuGet](https://img.shields.io/nuget/v/ElectronNET.CLI.svg?style=flat-square)](https://www.nuget.org/packages/ElectronNET.CLI/)

# Requirements to run:

The current Electron.NET CLI builds Windows/macOS/Linux binaries. Our API uses .NET Core 2.0, so our minimum base OS is the same as [.NET Core 2](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md).

# Usage:

To activate and communicate with the "native" (sort of native...) Electron API include the [ElectronNET.API NuGet package](https://www.nuget.org/packages/ElectronNET.API/) in your ASP.NET Core app.

````
PM> Install-Package ElectronNET.API
````
## Program.cs

You start Electron.NET up with an `UseElectron` WebHostBuilder-Extension. 

```csharp
public static IWebHost BuildWebHost(string[] args)
{
    return WebHost.CreateDefaultBuilder(args)
        .UseElectron(args)
        .UseStartup<Startup>()
        .Build();
}
```

## Startup.cs

Open the Electron Window in the Startup.cs file: 

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });

    // Open the Electron-Window here
    Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
}
```

__Please note:__ Currently it is important to use ASP.NET Core with MVC. If you are working with the dotnet CLI, use

    dotnet new mvc

## Start the Application

For the tooling you will need your dotnet-electronize package [ElectronNET.CLI NuGet package](https://www.nuget.org/packages/ElectronNET.CLI/). This package __must__ be referenced in the .csproj like this:

```
    <ItemGroup>
         <DotNetCliToolReference Include="ElectronNET.CLI" Version="*" />
    </ItemGroup>
```
After you edited the .csproj-file, you need to restore your NuGet packages within your Project. Run the follwoing command in your ASP.NET Core folder:

```
    dotnet restore
```

* Make sure you have __node.js v8.6.0__ and on __macOS/Linux__ the electron-packager installed! 
    
    sudo npm install electron-packager --global

At the first time, you need an Electron.NET project initialization. Type the following command in your ASP.NET Core folder:

```
    dotnet electronize init
```

* Now a electronnet.manifest.json should appear in your ASP.NET Core project
* Now run the following:

```
    dotnet electronize start
```
### Note
> Only the first electronize start is slow. The next will go on faster.

## Debug

Start your Electron.NET application with the Electron.NET CLI command. In Visual Studio attach to your running application instance. Go in the __Debug__ Menu and click on __Attach to Process...__. Sort by your projectname on the right and select it on the list.

## Usage of the Electron-API

A complete documentation will follow. Until then take a look in the source code of the sample application:  
[Electron.NET API Demos](https://github.com/ElectronNET/electron.net-api-demos)  

In this YouTube video, we show you how you can create a new project, use the Electron.NET API, debug a application and build an executable desktop app for Windows: [Electron.NET - Getting Started](https://www.youtube.com/watch?v=nuM6AojRFHk)  
  
## Build

Here you need the Electron.NET CLI too. Type following command in your ASP.NET Core folder:

```
    dotnet electronize build
```

In your default setting we just build the application for the OS you are running (Windows builds Windows, macOS builds macOS etc.), but this can be changed with:

```
    dotnet electronize build win
    dotnet electronize build osx
    dotnet electronize build linux
```

The end result should be an electron app under your __/bin/desktop__ folder.

### Note
> macOS builds on Windows are currently not supported, because the build just hangs, but I'm not sure why. The macOS builds works on Linux/macOS however.

# Working with this Repo

This repository consists of the main parts (API & CLI) and it's own "playground" ASP.NET Core application. Both main parts produce local NuGet packages, that are versioned with 1.0.0. The first thing you will need is to run one of the buildAll scripts (.cmd for Windows, the other for macOS/Linux).

If you look for pure __[demo projects](https://github.com/ElectronNET)__ checkout the other repositories. 

The problem working with this repository is, that NuGet has a pretty aggressive cache, see [here for further information](https://github.com/ElectronNET/Electron.NET/wiki).

# Contributing
Feel free to submit a pull request if you find any bugs (to see a list of active issues, visit the [Issues section](https://github.com/ElectronNET/Electron.NET/issues).
Please make sure all commits are properly documented.

# Authors

* **Gregor Biswanger** - (Microsoft MVP, Intel Black Belt and Intel Software Innovator) is a freelance lecturer, consultant, trainer, author and speaker. He is a consultant for large and medium-sized companies, organizations and agencies for software architecture, web- and cross-platform development. You can find Gregor often on the road attending or speaking at international conferences. - [Cross-Platform-Blog](http://www.cross-platform-blog.com) - Twitter [@BFreakout](https://www.twitter.com/BFreakout)  
* **Robert Muehsig** - Software Developer - from Dresden, Germany, now living & working in Switzerland. Microsoft MVP & Web Geek. - [codeinside Blog](https://blog.codeinside.eu) - Twitter [@robert0muehsig](https://twitter.com/robert0muehsig)  
  
See also the list of [contributors](https://github.com/ElectronNET/Electron.NET/graphs/contributors) who participated in this project.
  
# License
MIT-licensed

**Enjoy!**
