# What's New in ElectronNET.Core

## A Complete Transformation

ElectronNET.Core represents a fundamental modernization of Electron.NET, addressing years of accumulated pain points while preserving full API compatibility. This isn't just an update—it's a complete rethinking of how .NET developers build and debug cross-platform desktop applications with Electron.

## Complete Build System Overhaul

### From CLI Complexity to MSBuild Simplicity

The most visible change is the complete elimination of the CLI tool dependency. Where developers once needed to manage complex command-line operations and JSON configuration files, everything now flows through Visual Studio's native project system.

The old `electron.manifest.json` file is gone, replaced by clean MSBuild project properties that integrate seamlessly with Visual Studio's project designer. This provides not just a better development experience, but also eliminates entire categories of configuration errors that plagued earlier versions.

### Intelligent Package Structure

The new package architecture reflects a clearer separation of concerns:

- **ElectronNET.Core** - The main package containing build logic and project system integration
- **ElectronNET.Core.Api** - Pure API definitions for Electron integration
- **ElectronNET.Core.AspNet** - ASP.NET-specific runtime components

This modular approach allows projects to include only what they need while maintaining the flexibility to scale from simple console applications to complex web applications.  
More about the available nuget packages: [Package Description](../RelInfo/Package-Description.md).

## Beyond ASP.NET: Console Application Support

### A Shift in Accessibility

A major new opportunity in ElectronNET.Core is the removal of the ASP.NET requirement. Developers can now build Electron solutions using simple DotNet console applications, expanding the use cases and removing a major barrier to adoption for a number of use cases.

### Flexible Content Sources

Console applications with ElectronNET.Core support multiple content scenarios:

- **File System HTML/JS**: Serve static web content directly from the file system
- **Remote Server Integration**: Connect to existing web servers or APIs
- **Lightweight Architecture**: Avoid the overhead of ASP.NET when it's not needed
- **Simplified Deployment**: Package and distribute with minimal dependencies

This capability transforms ElectronNET from a web-focused framework into a versatile platform that can integrate with any HTML/JS content source, making it accessible to a much broader range of development scenarios and team structures.

## Revolutionary Development Experience

### Debugging Reimagined

The debugging experience has been completely transformed. The new DotNet-first launch mode means developers can now debug their .NET code directly, with full access to familiar debugging tools and Hot Reload capabilities. No more attaching to processes or working around limited debugging scenarios — the development workflow now matches standard .NET development patterns.

### Cross-Platform Development Without Compromises

One of the most significant breakthroughs is the ability to build and debug Linux applications directly from Windows Visual Studio through WSL integration. Developers can now:

- Build Linux packages while working on Windows
- Debug Linux application behavior in real-time
- Test cross-platform functionality without context switching
- Deploy to Linux targets with confidence

This capability eliminates the traditional barriers between Windows development environments and Linux deployment targets.

### Flexible Runtime Identifier Support

Runtime Identifier (RID) selection is now a first-class part of the project configuration, allowing developers to explicitly target specific platforms and architectures. The build system automatically structures output folders using standard .NET conventions (`bin\net8.0\win-x64`) instead of the ambiguous `bin\Desktop` layout, making multi-target builds clean and predictable.

## Modernized Architecture

### Process Lifecycle Revolution

The underlying process architecture has been fundamentally redesigned. Instead of Electron launching first and managing the .NET process, ElectronNET.Core puts .NET in control. The .NET application launches first and runs Electron as a child process, providing:

- Better process lifecycle management
- More reliable application termination
- Enhanced error handling and recovery
- Cleaner separation between web and native concerns

This architecture supports eight different launch scenarios, covering every combination of packaged/unpackaged deployment, console/ASP.NET hosting, and dotnet-first/electron-first initialization. The Electron-first launch method is still available or course.  
For more details, see: [Startup Methods](../GettingStarted/Startup-Methods.md).

### Unpackaged Development Mode

The new unpackaged run-mode transforms development workflows by using regular .NET builds with unpackaged Electron configurations. This approach leverages .NET's incremental build capabilities for both managed and native code, dramatically reducing rebuild times and improving the development feedback loop.

## Enhanced Technical Foundation

### TypeScript Integration

TypeScript compilation is now fully integrated with ASP.NET tooling, providing consistent builds across different development environments. The updated toolchain uses modern TypeScript versions with ESLint configuration, eliminating the compatibility issues that previously affected custom ElectronHostHook implementations.

### API Enhancements

The improved splash screen handling with automatic path resolution eliminates common configuration pitfalls, while maintaining full backward compatibility with existing ElectronHostHook code.

### Performance Optimizations

Package sizes have been reduced by eliminating unnecessary dependencies, while build performance has improved through intelligent incremental compilation. The new architecture also minimizes startup times through optimized build and launch procedures.

## Seamless Migration Path

### Backward Compatibility Focus

Despite the extensive changes, ElectronNET.Core maintains complete API compatibility with existing applications. The modular package structure allows for incremental adoption, and existing ElectronHostHook implementations continue to work without modification.

### Clear Upgrade Journey

The migration path is designed to be straightforward:
1. Update package references to the new structure
2. Remove the old manifest file
3. Configure project properties through Visual Studio
4. Adopt new debugging workflows at your own pace  

Further reading: [Migration Guide](Migration-Guide.md).

## Future Horizons

### Unlocked Possibilities

This modernization removes the technical debt that was limiting Electron.NET's evolution. The flexible Electron versioning, integrated build system, and cross-platform capabilities create a foundation for:

- More frequent updates and feature additions
- Enhanced community contributions
- Better tooling and IDE integration
- Expanded platform support

### Version Independence

The removal of rigid Electron version coupling means developers can now choose the Electron version that best fits their needs, with build-time validation ensuring compatibility. This approach encourages community feedback and enables faster adoption of new Electron features.

## Conclusion

ElectronNET.Core represents more than just new features—it's a complete reimagining of what .NET + Electron development can be. By eliminating friction points, removing the ASP.NET requirement to support console applications, improving debugging experiences, and enabling true cross-platform development, it transforms Electron.NET from a challenging framework to work with into a modern, efficient platform for building cross-platform desktop applications.

The changes address the core issues that were driving developers away from Electron.NET while opening new possibilities for the future. This foundation will enable more rapid innovation and better support for the growing demands of cross-platform .NET development.
