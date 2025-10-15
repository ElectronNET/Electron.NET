# Advanced Migration Topics

// 1. WebPort
// specifying the WebPort in the manifest is no longer supported
// from commit message:
//- Removed the 'electronWebPort' handling
//  When ASP.Net is launched first, then the information which port it
//  should use would be coming too late; anyway, there's no need for
//  letting the port number round-trip all the way through the manifest
//  file, loaded by main.js and then sent to dotnet.
//

if the asp web port needs to be specified manually, this can be by setting it via MSBuild like this:

  <ItemGroup>
    <AssemblyMetadata Include="AspNetHttpPort" Value="4000" />
  </ItemGroup>


// 2. ElectronHostHook


Users who have a custom ElectronHostHook in their project, need to do the following:


In their ElectronHostHook\package.json, they need to set typescript to 5.9.3 or later. If @types/node is presnt, it must be 22.x

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

Next step is to edit the project file and add a reference like this

    <PackageReference Include="Microsoft.TypeScript.MSBuild" Version="5.9.3" />

then the following propertygroup:

  <PropertyGroup>
    <TypeScriptModuleKind>commonjs</TypeScriptModuleKind>
    <TypeScriptUseNodeJS>true</TypeScriptUseNodeJS>
    <TypeScriptTSConfig>ElectronHostHook/tsconfig.json</TypeScriptTSConfig>
  </PropertyGroup>


and this itemgroup:

  <ItemGroup>
    <Compile Remove="publish\**" />
    <Content Remove="publish\**" />
    <EmbeddedResource Remove="publish\**" />
    <None Remove="publish\**" />
    <TypeScriptCompile Remove="**\node_modules\**" />
  </ItemGroup>


