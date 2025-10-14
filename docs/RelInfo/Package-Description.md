# Package Description

ElectronNET.Core consists of three specialized NuGet packages designed for different development scenarios and project architectures.

## 📦 Package Overview

### ElectronNET.Core (Main Package)
**Primary package for Electron.NET applications**

- **Build system integration** - MSBuild targets and tasks for Electron
- **Project system extensions** - Visual Studio designer integration
- **Runtime orchestration** - Process lifecycle management
- **Automatic configuration** - Generates electron-builder.json and package.json
- **Includes ElectronNET.Core.Api** as a dependency

**When to use:**
- Main application projects (startup projects)
- Projects that need full Electron.NET functionality
- Applications requiring build-time Electron configuration

### ElectronNET.Core.Api (API Package)
**Pure API definitions without build integration**

- **Electron API wrappers** - Complete Electron API surface
- **Type definitions** - Full TypeScript-style IntelliSense
- **No build dependencies** - Clean API-only package
- **Multi-platform support** - Windows, macOS, Linux

**When to use:**
- Class library projects that interact with Electron APIs
- Projects that only need API access without build integration
- Multi-project solutions where only the startup project needs build integration

### ElectronNET.Core.AspNet (ASP.NET Integration)
**ASP.NET-specific runtime components**

- **ASP.NET middleware** - UseElectron() extension methods
- **WebHost integration** - Seamless ASP.NET + Electron hosting
- **Hot Reload support** - Enhanced debugging experience
- **Web-specific optimizations** - ASP.NET tailored performance

**When to use:**
- ASP.NET Core projects building Electron applications
- Applications requiring web server functionality
- Projects using MVC, Razor Pages, or Blazor

## 🏗 Architecture Benefits

### Separation of Concerns
- **Build logic separate from API** - Cleaner dependency management
- **Optional ASP.NET integration** - Use only what you need
- **Multi-project friendly** - Share APIs across projects without build conflicts

### Project Scenarios

**Single Project (ASP.NET):**
```xml
<ItemGroup>
  <PackageReference Include="ElectronNET.Core" Version="1.0.0" />
  <PackageReference Include="ElectronNET.Core.AspNet" Version="1.0.0" />
</ItemGroup>
```

**Single Project (Console):**
```xml
<ItemGroup>
  <PackageReference Include="ElectronNET.Core" Version="1.0.0" />
</ItemGroup>
```

**Multi-Project Solution:**
```xml
<!-- Startup project -->
<ItemGroup>
  <PackageReference Include="ElectronNET.Core" Version="1.0.0" />
  <PackageReference Include="ElectronNET.Core.AspNet" Version="1.0.0" />
</ItemGroup>

<!-- Class library projects -->
<ItemGroup>
  <PackageReference Include="ElectronNET.Core.Api" Version="1.0.0" />
</ItemGroup>
```

## 🔗 Dependency Chain

```
ElectronNET.Core.AspNet
       ↓
ElectronNET.Core  →  ElectronNET.Core.Api
```

- **ElectronNET.Core.AspNet** depends on ElectronNET.Core
- **ElectronNET.Core** depends on ElectronNET.Core.Api
- **ElectronNET.Core.Api** has no dependencies

## 💡 Recommendations

✅ **Start with ElectronNET.Core** for new projects
✅ **Add ElectronNET.Core.AspNet** only for ASP.NET applications
✅ **Use ElectronNET.Core.Api** for class libraries and API-only scenarios
✅ **Multi-project solutions** benefit from the modular architecture
