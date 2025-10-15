

# Electron.NET Wiki Home

Welcome to the **Electron.NET Core** documentation! This wiki covers everything you need to know about building cross-platform desktop applications with ASP.NET Core and Electron.NET.

## 🚀 What is Electron.NET Core?

Electron.NET Core is a complete modernization of Electron.NET that eliminates the CLI tool dependency and integrates deeply with Visual Studio's MSBuild system. It transforms the development experience by providing:

- **Native Visual Studio Integration** - No more CLI tools or JSON configuration files
- **Console Application Support** - Build Electron apps with simple console applications, not just ASP.NET
- **Cross-Platform Development** - Build and debug Linux applications from Windows via WSL
- **Enhanced Debugging** - ASP.NET-first debugging with Hot Reload support
- **Flexible Architecture** - Choose any Electron version and target multiple platforms

## 📚 Documentation Sections

### [Core Documentation](Core/What's-New.md)
- **[What's New?](Core/What's-New.md)** - Complete overview of ElectronNET.Core features and improvements
- **[Migration Guide](Core/Migration-Guide.md)** - Step-by-step migration from previous versions
- **[Advanced Migration Topics](Core/Advanced-Migration-Topics.md)** - Technical details for complex scenarios

### [Getting Started](GettingStarted/ASP.Net.md)
- **[ASP.NET Applications](GettingStarted/ASP.Net.md)** - Build Electron apps with ASP.NET Core
- **[Console Applications](GettingStarted/Console-App.md)** - Use console apps for file system or remote content
- **[Startup Methods](GettingStarted/Startup-Methods.md)** - Understanding different launch scenarios
- **[Debugging](GettingStarted/Debugging.md)** - Debug Electron apps effectively in Visual Studio
- **[Package Building](GettingStarted/Package-Building.md)** - Create distributable packages

### [Release Information](RelInfo/Package-Description.md)
- **[Package Description](RelInfo/Package-Description.md)** - Understanding the three NuGet packages
- **[Changelog](../Changelog.md)** - Complete list of changes and improvements

## 🛠 System Requirements

- **.NET 8.0** or later
- **Node.js 22.x** or later
- **Visual Studio 2022** (recommended) or other .NET IDE
- **WSL2** (for Linux development on Windows)

## 💡 Key Benefits

✅ **No CLI Tools Required** - Everything works through Visual Studio
✅ **Console App Support** - Use any HTML/JS source, not just ASP.NET
✅ **True Cross-Platform** - Build Linux apps from Windows
✅ **Modern Debugging** - Hot Reload and ASP.NET-first debugging
✅ **Flexible Packaging** - Choose any Electron version
✅ **MSBuild Integration** - Leverages .NET's build system

## 🚦 Getting Started

New to ElectronNET.Core? Start here:

1. **[ASP.NET Setup](GettingStarted/ASP.Net.md)** - Traditional web application approach
2. **[Console App Setup](GettingStarted/Console-App.md)** - Lightweight console application approach
3. **[Migration Guide](Core/Migration-Guide.md)** - Moving from previous versions

## 🤝 Contributing

Found an issue or want to improve the documentation? Contributions are welcome! The wiki is auto-generated from the `/docs` folder in the [GitHub repository](https://github.com/ElectronNET/Electron.NET).
