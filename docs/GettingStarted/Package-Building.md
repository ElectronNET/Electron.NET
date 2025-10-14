# Package Building

ElectronNET.Core integrates with Visual Studio's publishing system to create distributable Electron packages using electron-builder. The process leverages .NET's build system while automatically generating the necessary Electron configuration files.

## 🎯 Publishing Overview

The publishing process differs slightly between ASP.NET and console applications:

- **ASP.NET Apps** - Use folder publishing with SelfContained=true
- **Console Apps** - Use folder publishing with SelfContained=false

## 📋 Prerequisites

Before publishing, ensure you have:

- **Node.js 22.x** installed
- **RuntimeIdentifier** set correctly for your target platform
- **Project configured** for Release builds

## 🚀 Publishing Process

### Step 1: Configure Runtime Identifier

Set the target platform in your `.csproj` file:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>  <!-- or linux-x64, osx-x64, etc. -->
</PropertyGroup>
```

### Step 2: Create Publish Profile

Add publish profiles to `Properties/PublishProfiles/`:

#### ASP.NET Application Profile (Windows)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
  </PropertyGroup>
</Project>
```

#### ASP.NET Application Profile (Linux)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
  </PropertyGroup>
</Project>
```

#### Console Application Profile (Windows)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
    <PublishReadyToRun>false</PublishReadyToRun>
  </PropertyGroup>
</Project>
```

#### Console Application Profile (Linux)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <PublishDir>publish\$(Configuration)\$(TargetFramework)\$(RuntimeIdentifier)\</PublishDir>
    <PublishProtocol>FileSystem</PublishProtocol>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <PublishSingleFile>false</PublishSingleFile>
  </PropertyGroup>
</Project>
```

### Step 3: Configure Electron Builder

ElectronNET.Core automatically generates `electron-builder.json` based on your project configuration. Key settings include:

```json
{
  "productName": "My Electron App",
  "appId": "com.mycompany.myapp",
  "directories": {
    "output": "release"
  },
  "files": [
    "**/*",
    "!**/*.pdb"
  ],
  "win": {
    "target": "nsis",
    "icon": "assets/app.ico"
  },
  "linux": {
    "target": "AppImage",
    "icon": "assets/app.png"
  }
}
```

### Step 4: Publish from Visual Studio

1. **Right-click your project** in Solution Explorer
2. **Select "Publish"**
3. **Choose "Folder"** as the publish target
4. **Select your publish profile** (Windows/Linux)
5. **Click "Publish"**

The publish process will:
- Build your .NET application
- Generate Electron configuration files
- Copy Electron runtime files
- Install npm dependencies
- Create electron-builder configuration

### Step 5: Build Final Package

After publishing, build the final package:

```bash
# Navigate to publish directory
cd publish\Release\net8.0\win-x64\

# Install dependencies and build
npm install
npx electron-builder

# Find your package in the 'release' folder
```

## 📁 Output Structure

After publishing, your folder structure will look like:

```
publish\Release\net8.0\win-x64\
├── MyElectronApp.exe          # Your .NET application
├── .electron\                 # Electron runtime files
│   ├── main.js
│   ├── package.json
│   └── node_modules\
├── wwwroot\                   # (ASP.NET only) Web assets
├── electron-builder.json      # Build configuration
└── release\                   # Final packages (after electron-builder)
    ├── MyElectronApp-1.0.0.exe
    └── MyElectronApp-1.0.0-setup.exe
```

## 🔧 Configuration Options

### Project Configuration

Configure Electron settings in your `.csproj`:

```xml
<PropertyGroup>
  <ElectronNETCoreDescription>My Electron Application</ElectronNETCoreDescription>
  <ElectronNETCoreDisplayName>MyApp</ElectronNETCoreDisplayName>
  <ElectronNETCoreAuthorName>Your Company</ElectronNETCoreAuthorName>
  <ElectronNETCoreVersion>1.0.0</ElectronNETCoreVersion>
</PropertyGroup>
```

### Electron Builder Configuration

Customize `electron-builder.json` for your needs:

```json
{
  "productName": "MyApp",
  "appId": "com.mycompany.myapp",
  "copyright": "Copyright © 2024 My Company",
  "win": {
    "target": "nsis",
    "icon": "assets/icon.ico",
    "publisherName": "My Company"
  },
  "linux": {
    "target": "AppImage",
    "icon": "assets/icon.png",
    "category": "Office"
  }
}
```

## 🎯 Platform-Specific Settings

### Windows Configuration

```json
{
  "win": {
    "target": "nsis",
    "icon": "assets/app.ico",
    "verifyUpdateCodeSignature": false
  }
}
```

### Linux Configuration

```json
{
  "linux": {
    "target": "AppImage",
    "icon": "assets/app.png",
    "category": "Development"
  }
}
```

### macOS Configuration

```json
{
  "mac": {
    "target": "dmg",
    "icon": "assets/app.icns",
    "category": "public.app-category.developer-tools"
  }
}
```

## 🚀 Advanced Publishing

### Cross-Platform Building

Build for multiple platforms from Windows using WSL:

1. **Set RuntimeIdentifier** to `linux-x64`
2. **Publish to folder** using Linux profile
3. **Copy to WSL** or build directly in WSL

### CI/CD Integration

For automated builds:

```bash
# Restore packages
dotnet restore

# Publish for specific platform
dotnet publish -c Release -r win-x64 --self-contained

# Build Electron package
cd publish\Release\net8.0\win-x64
npm install
npx electron-builder --publish=never
```

## 🛠 Troubleshooting

### Common Issues

**"electron-builder.json not found"**
- Ensure project is published first
- Check that RuntimeIdentifier is set
- Verify .NET build succeeded

**"npm install fails"**
- Ensure Node.js 22.x is installed
- Check internet connection for npm packages
- Verify no conflicting package versions

**"WSL publishing fails"**
- Ensure WSL2 is properly configured
- Check that Linux RID is set correctly
- Verify WSL can access Windows files

## 🎨 Publishing Workflow

*Placeholder for image showing Visual Studio publish dialog and electron-builder output*

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
