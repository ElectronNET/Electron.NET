# Advanced Migration Topics

This guide covers advanced scenarios and edge cases that may require additional configuration when migrating to ElectronNET.Core.

## Custom ASP.NET Port Configuration

### Port Configuration Changes

**Previous Approach:**
Specifying the WebPort in `electron.manifest.json` is no longer supported because the ASP.NET-first launch mode makes this timing-dependent.

**New Approach:**
Configure custom ASP.NET ports through MSBuild metadata:

```xml
<ItemGroup>
  <AssemblyMetadata Include="AspNetHttpPort" Value="4000" />
  <AssemblyMetadata Include="AspNetHttpsPort" Value="4001" />
</ItemGroup>
```

**Usage in Code:**
```csharp
// Access configured ports at runtime
var port = int.Parse(Electron.App.GetEnvironmentVariable("AspNetHttpPort") ?? "5000");
```

## Custom ElectronHostHook Configuration

### TypeScript and Node.js Updates

**Update package.json:**
```json
{
  "devDependencies": {
    "eslint": "^9.37.0",
    "@types/node": "^22.18",
    "typescript": "^5.9.3"
  },
  "dependencies": {
    "archiver-utils": "^2.1.0",
    "socket.io": "^4.8.1",
    "exceljs": "^1.10.0"
  }
}
```

**Update Project File:**
```xml
<PackageReference Include="Microsoft.TypeScript.MSBuild" Version="5.9.3" />

<PropertyGroup>
  <TypeScriptModuleKind>commonjs</TypeScriptModuleKind>
  <TypeScriptUseNodeJS>true</TypeScriptUseNodeJS>
  <TypeScriptTSConfig>ElectronHostHook/tsconfig.json</TypeScriptTSConfig>
</PropertyGroup>

<ItemGroup>
  <Compile Remove="publish\**" />
  <Content Remove="publish\**" />
  <EmbeddedResource Remove="publish\**" />
  <None Remove="publish\**" />
  <TypeScriptCompile Remove="**\node_modules\**" />
</ItemGroup>
```

### Integration Benefits

- **Modern TypeScript** - Latest language features and better type checking
- **Updated Node.js Types** - Compatibility with Node.js 22.x APIs
- **ESLint Integration** - Better code quality and consistency
- **MSBuild Compilation** - Integrated with Visual Studio build process

## Troubleshooting Advanced Scenarios

### Multi-Project Solutions

When using ElectronNET.Core in multi-project solutions:

1. **Install ElectronNET.Core.Api** in class library projects
2. **Install ElectronNET.Core** only in the startup project
3. **Share configuration** through project references or shared files

### Custom Build Processes

For advanced build customization:

```xml
<PropertyGroup>
  <ElectronNETCoreOutputPath>custom\output\path</ElectronNETCoreOutputPath>
  <ElectronNETCoreNodeModulesPath>custom\node_modules</ElectronNETCoreNodeModulesPath>
</PropertyGroup>
```

### Environment-Specific Configuration

Handle different environments with conditional configuration:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <ElectronNETCoreEnvironment>Development</ElectronNETCoreEnvironment>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <ElectronNETCoreEnvironment>Production</ElectronNETCoreEnvironment>
</PropertyGroup>
```

## Next Steps

- **[Migration Guide](Migration-Guide.md)** - Complete migration process
- **[What's New?](What's-New.md)** - Overview of all ElectronNET.Core features
- **[Getting Started](../GettingStarted/ASP.Net.md)** - Development workflows
