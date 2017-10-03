# ElectronNET
Build cross platform desktop apps with .NET Core and ASP.NET NET Core.

## Dev Notes:

Currently there are two main projects and a "WebApp" that serves as a demo playground.

* ElectronNET.API: Defines the Electron / .NET API bridge
* ElectronNET.CLI: Responsible for the dotnet "electronize" tooling.

Both projects create their own nuget package. The resulting nuget packages are saved under "/artifacts".

__Currently__ the packages are just build with version 1.0.0. If you change something you need to clear the nuget cache under this directory (because the version number doesn't change):

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