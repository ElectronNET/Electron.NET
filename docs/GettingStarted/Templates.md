
# Project Templates

Electron.NET ships a `dotnet new` template package so you can scaffold a ready-to-run desktop app instead of wiring up an ASP.NET project by hand.

## 🛠 System Requirements

See [System Requirements](../GettingStarted/System-Requirements.md).

## 📦 Install the Templates

```bash
dotnet new install ElectronNET.Core.Templates
```

To update to the latest version, run the same command again. To remove them:

```bash
dotnet new uninstall ElectronNET.Core.Templates
```

## 🚀 Available Templates

| Template | Short name | Description |
|----------|------------|-------------|
| Electron.NET Blazor App | `electron-blazor` | A Blazor Server app hosted in an Electron shell |

## 🧱 Create a Blazor App

```bash
dotnet new electron-blazor -n MyDesktopApp
cd MyDesktopApp
```

Then start it in the Electron shell:

```bash
dotnet build
electronize start
```

### Options

| Option | Values | Default | Description |
|--------|--------|---------|-------------|
| `-f`, `--framework` | `net8.0`, `net10.0` | `net8.0` | The target framework of the generated project |
| `-e`, `--electron-version` | any Electron version | `38.2.2` | The Electron version the app is built against |
| `-p`, `--port` | port number | `8001` | The port the ASP.NET server uses during development |
| `--no-restore` | — | — | Skip the automatic `dotnet restore` |

Example:

```bash
dotnet new electron-blazor -n MyDesktopApp -f net10.0 -e 30.4.0 -p 8123
```

## 📁 What You Get

```
MyDesktopApp/
├── Components/
│   ├── Layout/          MainLayout.razor, NavMenu.razor
│   ├── Pages/           Home.razor, Counter.razor
│   ├── App.razor
│   ├── Routes.razor
│   └── _Imports.razor
├── Properties/
│   ├── PublishProfiles/ win-x64, linux-x64 and osx-arm64 folder profiles
│   ├── electron-builder.json
│   └── launchSettings.json
├── wwwroot/app.css
├── appsettings.json
├── Program.cs
└── MyDesktopApp.csproj
```

The project already references `ElectronNET.Core` and `ElectronNET.Core.AspNet`, calls `builder.UseElectron(...)` in `Program.cs`, and passes all [Migration Checks](../Core/Migration-Checks.md) out of the box.

> [!Note]
> `ElectronNET.API` also defines a type named `App`, which collides with the Blazor root
> component. That is why the template calls `app.MapRazorComponents<MyDesktopApp.Components.App>()`
> with a fully qualified type name.

## 🚀 Next Steps

- **[Configuration](../Using/Configuration.md)** - Adjust app metadata and Electron settings
- **[Debugging](../Using/Debugging.md)** - Debug the .NET and Electron sides
- **[Package Building](../Using/Package-Building.md)** - Create distributable packages
