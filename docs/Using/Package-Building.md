# Package Building

ElectronNET.Core integrates with Visual Studio's publishing system to create distributable Electron packages using electron-builder. The process leverages .NET's build system while automatically generating the necessary Electron configuration files.

## 🎯 Publishing Overview

The publishing process differs slightly between ASP.NET and console applications:

- **ASP.NET Apps** - Use folder publishing with SelfContained=true
- **Console Apps** - Use folder publishing with SelfContained=false


## 🚀 Publishing Process

### Step 1: Create Publish Profiles

Add publish profiles to `Properties/PublishProfiles/`:

#### ASP.NET Application Profile (Windows)

**win-x64.pubxml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <DeleteExistingFiles>true</DeleteExistingFiles>
    <PublishProvider>FileSystem</PublishProvider>
    <PublishUrl>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishUrl>
    <WebPublishMethod>FileSystem</WebPublishMethod>
    <_TargetId>Folder</_TargetId>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <ProjectGuid>48eff821-2f4d-60cc-aa44-be0f1d6e5f35</ProjectGuid>
    <SelfContained>true</SelfContained>
  </PropertyGroup>
</Project>
```

#### ASP.NET Application Profile (Linux)

**linux-x64.pubxml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <DeleteExistingFiles>true</DeleteExistingFiles>
    <PublishProvider>FileSystem</PublishProvider>
    <PublishUrl>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishUrl>
    <WebPublishMethod>FileSystem</WebPublishMethod>
    <_TargetId>Folder</_TargetId>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <ProjectGuid>48eff821-2f4d-60cc-aa44-be0f1d6e5f35</ProjectGuid>
    <SelfContained>true</SelfContained>
  </PropertyGroup>
</Project>
```

#### Console Application Profile (Windows)

**win-x64.pubxml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
    <PublishReadyToRun>false</PublishReadyToRun>
  </PropertyGroup>
</Project>
```

#### Console Application Profile (Linux)

**linux-x64.pubxml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
  </PropertyGroup>
</Project>
```

### Step 2: Configure Electron Builder

ElectronNET.Core automatically adds a default `electron-builder.json` file under `Properties\electron-builder.json`.  
Please see here for details of the available configuration options: https://www.electron.build/.


### Step 3: Publish from Visual Studio

1. **Right-click your project** in Solution Explorer
2. **Select "Publish"**
4. **Select your publish profile** (Windows/Linux)
5. **Click "Publish"**

The publish process will:
- Build your .NET application
- Copy all files as needed
- Install npm dependencies
- Run electron-builder

> [!NOTE]  
> When running publish for a Linux configuration on Windows, Electron.NET will automatically use WSL for the platform-specific steps.

**After publishing**, the final results will be in 

`publish\Release\netx.0\xxx-xxx\`


## MacOS

> [!NOTE]
> macOS builds can't be created on Windows machines because they require symlinks that aren't supported on Windows (per [this Electron issue](https://github.com/electron-userland/electron-packager/issues/71)). macOS builds can be produced on either Linux or macOS machines.


## 🚀 Next Steps

- **[Startup Methods](Startup-Methods.md)** - Understanding different launch modes for packaged apps
- **[Debugging](Debugging.md)** - Debug packaged applications
- **[Migration Guide](../Core/Migration-Guide.md)** - Update existing projects for new publishing

## 💡 Benefits

✅ **Native VS Integration** - Use familiar publish workflows  
✅ **Cross-Platform Building** - Build Linux packages from Windows  
✅ **Automatic Configuration** - No manual electron-builder setup  
✅ **Multiple Package Types** - NSIS, AppImage, DMG, etc.  
✅ **CI/CD Ready** - Easy integration with build pipelines  
