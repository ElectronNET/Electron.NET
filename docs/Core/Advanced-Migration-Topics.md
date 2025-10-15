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
</ItemGroup>
```

## Custom ElectronHostHook Configuration

> [!NOTE]  
> These changes are only required in case you are using a custom ElectronHostHook implementation!  
> If you have an ElectronHostHook folder in your project but you did not customize that code and aren't using its demo features (Excel and ZIP), you can also just remove that folder from your project.


### TypeScript and Node.js Updates

**Update package.json:**

This shows the delevant changes only: All shown versions are the new required minimum versions.

```json
{
  "devDependencies": {
    "@types/node": "^22.18",
    "typescript": "^5.9.3"
  },
  "dependencies": {
    "socket.io": "^4.8.1",
  }
}
```

**Update Project File:**  

The below modifications will allow you to use the latest TypeScript compiler in your ASP.Net project.

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
2. **Install ElectronNET.Core and ElectronNET.Core.AspNet** only in the startup project
3. **Share configuration** through project references or shared files


## Next Steps

- **[Migration Guide](Migration-Guide.md)** - Complete migration process
- **[What's New?](What's-New.md)** - Overview of all ElectronNET.Core features
- **[Getting Started](../GettingStarted/ASP.Net.md)** - Development workflows
