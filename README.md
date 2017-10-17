[![Electron.NET Logo](https://github.com/GregorBiswanger/Electron.NET/blob/master/assets/images/electron.net-logo.png)](https://github.com/GregorBiswanger/Electron.NET)

Master: [![Build status](https://ci.appveyor.com/api/projects/status/u710d6hman5a4beb/branch/master?svg=true)](https://ci.appveyor.com/project/robertmuehsig/electron-net/branch/master)

Build cross platform desktop apps with .NET Core and ASP.NET NET Core.

## Dev Notes:

Currently there are two main projects and a "WebApp" that serves as a demo playground.

* ElectronNET.API: Defines the Electron / .NET API bridge
* ElectronNET.CLI: Responsible for the dotnet "electronize" tooling.

Both projects create their own nuget package. The resulting nuget packages are saved under "/artifacts".

__Currently__ the packages are just build with version 1.0.0.
If you change something you need to clear the nuget cache under this directory (because the version number doesn't change):

    %userprofile%\.nuget\packages

The solution contains a NuGet.config which points to the artifacts directory, so we can just use the produced NuGet packages without uploading to NuGet.org.

__ElectronNET.WebApp:__

The WebApp now has referenced the API NuGet package and via the .csproj reference the CLI:

    <ItemGroup>
       <DotNetCliToolReference Include="ElectronNET.CLI" Version="*" />
    </ItemGroup>

## Dev Notes: Usage

Navigate to the WebApp folder and type this in a command prompt:

   dotnet electronize
   
# Dev Notes: Dev Workflow 

(at least for the CLI extension)

* Change something in the CLI project
* rebuild the project (a nuget package should now appear in the /artifacts directory)
* make sure there is no ElectronNET.CLI package in your %userprofile%\.nuget\packages cache with the same version 
* execute dotnet restore in the WebApp
* execute dotnet electronize
