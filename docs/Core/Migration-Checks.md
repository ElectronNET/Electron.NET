# Migration Checks

Electron.NET includes automatic build-time validation checks that help users migrating from previous versions avoid common configuration issues. These checks run automatically during the build process and provide helpful guidance when problems are detected.

## Overview

When you build an Electron.NET project, the following validation checks are performed:

| Code | Check | Description |
|------|-------|-------------|
| [ELECTRON001](#1-packagejson-not-allowed) | package.json not allowed | Ensures no package.json exists outside ElectronHostHook |
| [ELECTRON002](#2-electron-manifestjson-not-allowed) | electron-manifest.json not allowed | Detects deprecated manifest files |
| [ELECTRON003](#3-electron-builderjson-location) | electron-builder.json location | Verifies electron-builder.json exists in Properties folder |
| [ELECTRON004](#3-electron-builderjson-location) | electron-builder.json wrong location | Warns if electron-builder.json is found in incorrect locations |
| [ELECTRON005](#4-parent-paths-not-allowed-in-electron-builderjson) | Parent paths not allowed | Checks for `..` references in config |
| [ELECTRON006](#5-publish-profile-validation) | ASP.NET publish profile mismatch | Warns when ASP.NET projects have console-style profiles |
| [ELECTRON007](#5-publish-profile-validation) | Console publish profile mismatch | Warns when console projects have ASP.NET-style profiles |

---

## 1. package.json not allowed

**Warning Code:** `ELECTRON001`

### What is checked

The build system scans for `package.json` and `package-lock.json` files in your project directory. These files should not exist in the project root or subdirectories (with one exception).

### Why this matters

In previous versions of Electron.NET, a `package.json` file was required in the project. The new version generates this file automatically from MSBuild properties defined in your `.csproj` file.

### Exception

A `package.json` file **is allowed** in the `ElectronHostHook` folder if you're using custom host hooks. This is the only valid location for a manually maintained package.json.

### How to fix

1. **Open your project's `.csproj` file**
2. **Add the required properties** to a PropertyGroup with the label `ElectronNetCommon`:

```xml
<PropertyGroup Label="ElectronNetCommon">
  <ElectronPackageId>my-electron-app</ElectronPackageId>
  <Title>My Electron App</Title>
  <Version>1.0.0</Version>
  <Description>My awesome Electron.NET application</Description>
  <Company>My Company</Company>
  <Copyright>Copyright © 2025</Copyright>
  <ElectronVersion>30.0.9</ElectronVersion>
</PropertyGroup>
```

3. **Delete the old `package.json`** file from your project root

> **See also:** [Migration Guide](Migration-Guide.md) for complete migration instructions.

---

## 2. electron-manifest.json not allowed

**Warning Code:** `ELECTRON002`

### What is checked

The build system checks for the presence of `electron.manifest.json` or `electron-manifest.json` files in your project.

### Why this matters

The `electron.manifest.json` file format is deprecated. All configuration should now be specified using:
- MSBuild properties in your `.csproj` file (for application metadata)
- The `electron-builder.json` file in the `Properties` folder (for build configuration)

### How to fix

1. **Migrate application properties** to your `.csproj` file (see [Migration Guide](Migration-Guide.md))
2. **Move the `build` section** from `electron.manifest.json` to `Properties/electron-builder.json`
3. **Delete the old `electron.manifest.json`** file

**Example electron-builder.json:**
```json
{
    "compression": "maximum",
    "win": {
        "icon": "Assets/app.ico",
        "target": ["nsis", "portable"]
    },
    "linux": {
        "icon": "Assets/app.png",
        "target": ["AppImage", "deb"]
    },
    "mac": {
        "icon": "Assets/app.icns",
        "target": ["dmg", "zip"]
    }
}
```

---

## 3. electron-builder.json Location

**Warning Codes:** `ELECTRON003`, `ELECTRON004`

### What is checked

- `ELECTRON003`: Verifies that an `electron-builder.json` file exists in the `Properties` folder
- `ELECTRON004`: Warns if `electron-builder.json` is found in incorrect locations

### Why this matters

The `electron-builder.json` file must be located in the `Properties` folder so it can be properly copied to the output directory during publishing.

### How to fix

1. **Create the Properties folder** if it doesn't exist
2. **Move or create** `electron-builder.json` in `Properties/electron-builder.json`
3. **Remove** any `electron-builder.json` files from other locations

**Expected structure:**
```
MyProject/
├── Properties/
│   ├── electron-builder.json    ✅ Correct location
│   ├── launchSettings.json
│   └── PublishProfiles/
├── MyProject.csproj
└── Program.cs
```

---

## 4. Parent paths not allowed in electron-builder.json

**Warning Code:** `ELECTRON005`

### What is checked

The build system scans the `electron-builder.json` file for parent-path references (`..`).

### Why this matters

During the publish process, the `electron-builder.json` file is copied to the build output directory. Any relative paths in this file are resolved from that location, not from your project directory. Parent-path references (`../`) will not work correctly because they would point outside the published application.

### How to fix

1. **Move resource files** (icons, installers, etc.) inside your project folder structure
2. **Configure the files** to be copied to the output directory in your `.csproj`:

```xml
<ItemGroup>
  <Content Include="Assets\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

3. **Update paths** in `electron-builder.json` to use relative paths without `..`:

**Before (incorrect):**
```json
{
    "win": {
        "icon": "../SharedAssets/app.ico"
    }
}
```

**After (correct):**
```json
{
    "win": {
        "icon": "Assets/app.ico"
    }
}
```

---

## 5. Publish Profile Validation

**Warning Codes:** `ELECTRON006`, `ELECTRON007`

### What is checked

The build system examines `.pubxml` files in the `Properties/PublishProfiles` folder and validates that they match the project type:

- **ELECTRON006**: For **ASP.NET projects** (using `Microsoft.NET.Sdk.Web`), checks that publish profiles include `WebPublishMethod`. This property is required for proper ASP.NET publishing.

- **ELECTRON007**: For **console/other projects** (not using the Web SDK), checks that publish profiles do NOT include the `WebPublishMethod`  property. These ASP.NET-specific properties are incorrect for non-web applications.

### Why this matters

Electron.NET supports both ASP.NET and console application project types, each requiring different publish profile configurations:

| Project Type | SDK | Expected Properties |
|--------------|-----|---------------------|
| ASP.NET (Razor Pages, MVC, Blazor) | `Microsoft.NET.Sdk.Web` | `WebPublishMethod`, no `PublishProtocol` |
| Console Application | `Microsoft.NET.Sdk` | `PublishProtocol`, no `WebPublishMethod` |

Using the wrong publish profile type can lead to incomplete or broken builds.

### How to fix

1. **Delete existing publish profiles** from `Properties/PublishProfiles/`
2. **Create new publish profiles** using the Visual Studio Publishing Wizard:
   - Right-click on the project in Solution Explorer
   - Select **Publish...**
   - Follow the wizard to create a **Folder** publish profile

For correct publish profile examples for both ASP.NET and Console applications, see **[Package Building](../Using/Package-Building.md#step-1-create-publish-profiles)**.

---

## Disabling Migration Checks

If you need to disable specific migration checks (not recommended), you can set the following properties in your `.csproj` file:

```xml
<PropertyGroup>
  <!-- Disable all migration checks -->
  <ElectronSkipMigrationChecks>true</ElectronSkipMigrationChecks>
</PropertyGroup>
```

> ⚠️ **Warning:** Disabling migration checks may result in build or runtime errors. Only disable checks if you fully understand the implications.

---

## See Also

- [Migration Guide](Migration-Guide.md) - Complete step-by-step migration instructions
- [Advanced Migration Topics](Advanced-Migration-Topics.md) - Complex migration scenarios
- [Configuration](../Using/Configuration.md) - Project configuration options
- [Package Building](../Using/Package-Building.md) - Building distributable packages
