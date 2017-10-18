[![Electron.NET Logo](https://github.com/GregorBiswanger/Electron.NET/blob/master/assets/images/electron.net-logo.png)](https://github.com/GregorBiswanger/Electron.NET)

Master: [![Build status](https://ci.appveyor.com/api/projects/status/u710d6hman5a4beb/branch/master?svg=true)](https://ci.appveyor.com/project/robertmuehsig/electron-net/branch/master)

Build cross platform desktop apps with .NET Core and ASP.NET NET Core.

# Usage:

* To communicate with the "native" (sort of native...) Electron API include the [ElectronNET.API NuGet package](https://www.nuget.org/packages/ElectronNET.API/) in your ASP.NET Core app.

* For the tooling you will need your dotnet-electronize package [ElectronNET.CLI NuGet package](https://www.nuget.org/packages/ElectronNET.CLI/). This package __must__ be referenced in the .csproj like this:

```
    <ItemGroup>
       <DotNetCliToolReference Include="ElectronNET.CLI" Version="*" />
    </ItemGroup>
```

* Make sure you have node.js and on OSX/Ubuntu the electron-packager installed (via "sudo npm install electron-packager --global")
* In your ASP.NET Core folder run:

```
    dotnet electronize init
```

* Now a electronnet.manifest.json should appear in your ASP.NET Core project
* Now run the following:

```
    dotnet electronize build
```

* In your default setting we just build the application for the OS you are running (Windows builds Windows, OSX builds OSX etc.), but this can be changed with:

```
    dotnet electronize build win
    dotnet electronize build osx
    dotnet electronize build linux
```

* The end result should be a electron app under your /bin/desktop folder

## Dev Notes:

Currently there are two main projects and a "WebApp" that serves as a demo playground.

* ElectronNET.API: Defines the Electron / .NET API bridge
* ElectronNET.CLI: Responsible for the dotnet "electronize" tooling.

Both projects create their own nuget package. The resulting nuget packages are saved under "/artifacts".

__Currently__ the packages are just build with version 1.0.0 on your machine. NuGet might pick the wrong version from its own cache, so please read the dev notes if you want to develop!

__ElectronNET.WebApp:__

The WebApp now has referenced the API NuGet package and via the .csproj reference the CLI:

    <ItemGroup>
       <DotNetCliToolReference Include="ElectronNET.CLI" Version="*" />
    </ItemGroup>


   
# Dev Notes: Dev Workflow 

(at least for the CLI extension)

* Change something in the CLI project
* rebuild the project (a nuget package should now appear in the /artifacts directory)
* make sure there is no ElectronNET.CLI package in your %userprofile%\.nuget\packages cache with the same version 
* execute dotnet restore in the WebApp 
* execute dotnet electronize
