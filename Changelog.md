# 0.1.0

## ElectronNET.Core

- Updated `PrintToPDFOptions` to also allow specifying the `PageSize` with an object (#769)
- Updated splashscreen image to have 0 margin (#622)
- Updated the IPC API w.r.t. naming and consistency (#905) @agracio
- Updated the IPC bridge w.r.t. synchronization and thread-safety (#918) @agracio
- Updated serialization to use `System.Text.Json` replacing `Newtonsoft.Json` (#917) @Denny09310
- Fixed parameter handling for the `sendToIpcRenderer` function (#922) @softworkz
- Fixed synchronization on removing event handlers (#921) @softworkz
- Fixed creation of windows with `contextIsolation` enabled (#906) @NimbusFox
- Fixed single instance behavior using the `ElectronSingleInstance` property (#901)
- Fixed potential race conditions (#908) @softworkz
- Added option to use `ElectronSplashScreen` with an HTML file (#799)
- Added option to provide floating point value as aspect ratios with `SetAspectRatio` (#793)
- Added option to provide `TitleBarOverlay` as an object (#911) @Denny09310
- Added `TitleBarOverlay` property to `BrowserWindowOptions` (#909)
- Added `RoundedCorners` property to `BrowserWindowOptions`
- Added integration tests and robustness checks (#913) @softworkz
- Added .NET 10 as an explicit target

# 0.0.18

## ElectronNET.Core

### Highlights

- **Complete MSBuild Integration**: Eliminated CLI tool dependency, moved all build processes to MSBuild with deep Visual Studio integration
- **Modernized Architecture**: Restructured process lifecycle with .NET launching first and running Electron as child process for better control and reliability
- **Cross-Platform Development**: Build and debug Linux applications directly from Windows Visual Studio via WSL integration
- **Flexible Electron Versioning**: Removed version lock-in, users can now select any Electron version with build-time validation
- **Enhanced Debugging Experience**: ASP.NET-first debugging with Hot Reload support and improved process termination handling
- **Console App Support**: No longer requires ASP.NET - now works with simple console applications for file system or remote server HTML/JS

### Build System & Project Structure

- Eliminated electron.manifest.json configuration file, replaced with MSBuild project properties
- Introduced new package structure: ElectronNET.Core (main package), ElectronNET.Core.Api (API definitions), ElectronNET.Core.AspNet (ASP.NET integration)
- Added Runtime Identifier (RID) selection as part of project configuration
- Restructured build folder layout to use standard .NET format (bin\net8.0\win-x64) instead of bin\Desktop
- Implemented incremental build support for Electron assets to improve build performance
- Added custom MSBuild tasks for Electron-specific build operations

### Development Experience

- Implemented unpackaged run-mode for development using regular .NET builds with unpackaged Electron configuration
- Added support for building Linux packages on Windows via WSL integration
- Enabled running and debugging Linux application outputs on Windows through WSL
- Integrated TypeScript compilation with ASP.NET tooling for consistent builds
- Added process orchestration supporting 8 different launch scenarios (packaged/unpackaged × console/ASP.NET × dotnet-first/electron-first)

### Debugging & Runtime

- Dramatically improved debugging experience with ASP.NET-first launch mode
- Added Hot Reload support for ASP.NET code during development
- Implemented proper process termination handling for all exit scenarios
- Minimized startup times through optimized build and launch procedures

### Technical Improvements

- Enhanced splash screen handling with automatic path resolution
- Improved ElectronHostHook integration as proper npm package dependency
- Updated to latest TypeScript version with ESLint configuration
- Added support for custom main.js files in projects
- Implemented version management through common.props file
- Added build-time Electron version compatibility validation

### Package & Distribution

- Integrated MSBuild publishing mechanisms for creating Electron packages
- Added folder publishing support with improved parameter handling
- Implemented automated package.json generation from MSBuild properties
- Added GitHub release automation with proper versioning
- Reduced package sizes by eliminating unnecessary TypeScript dependencies

### Migration & Compatibility

- Maintained backward compatibility for existing ElectronHostHook implementations
- Removed ASP.NET requirement: Now works with simple console applications for file system or remote server HTML/JS scenarios
- Added support for both console and ASP.NET Core application types
- Preserved all existing Electron API functionality while modernizing architecture
- Added migration path for existing projects through updated package structure

This represents a comprehensive modernization of Electron.NET, addressing the major pain points around debugging, build complexity, and platform limitations while maintaining full API compatibility.
