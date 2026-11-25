# Electron.NET Sample: App Icons

A working sample with app icons for all three platforms: Windows, Linux and macOS.

There are two separate places where icons matter:

1. **Packaging** — the icon of the executable, installer, app bundle or desktop entry. It is set by electron-builder via `Properties/electron-builder.json`.
2. **Runtime** — the icon of the window, taskbar or dock while the app runs. It is set in `Program.cs`. This matters most during development, because the unpackaged app would otherwise show the default Electron icon. On Linux, it is also needed in the packaged app.

## Files

```
Assets/
├── app.svg                 source artwork (not used at runtime)
└── icons/
    ├── app.ico             Windows: 16, 24, 32, 48, 64, 128, 256 px
    ├── app.icns            macOS: 16 to 1024 px (incl. @2x variants)
    └── linux/
        ├── 16x16.png       Linux: one PNG per size, named <size>x<size>.png
        ├── ...
        └── 512x512.png
```

The `.csproj` copies `Assets/icons` to the build and publish output. During publishing, the .NET app is placed in the `bin` folder of the electron-builder project, so `electron-builder.json` references the icons as `bin/Assets/icons/...`:

```json
{
  "win":   { "icon": "bin/Assets/icons/app.ico" },
  "linux": { "icon": "bin/Assets/icons/linux" },
  "mac":   { "icon": "bin/Assets/icons/app.icns" }
}
```

`<ApplicationIcon>` in the `.csproj` also sets the icon of the .NET executable itself on Windows (e.g., shown in Task Manager).

## Platform notes

| Platform | Requirements |
| --- | --- |
| Windows | `.ico` with at least a 256×256 image. Include smaller sizes (16, 32, 48) so that the taskbar and Explorer don't have to scale down. |
| Linux | A folder with `<size>x<size>.png` files, or a single PNG of at least 256×256 px. The icon is only part of formats that have a desktop entry (AppImage, deb, rpm, ...) — a plain `tar.xz` archive has none, so there only the runtime window icon applies. |
| macOS | `.icns` with sizes up to 1024×1024 (512×512@2x). Windows on macOS have no icon, so at runtime only the dock icon can be set (`Electron.Dock.SetIcon`). The modern `.icon` format (Icon Composer) is supported too, see [Configuration](../../docs/Using/Configuration.md#app-icon-path). |

## Creating your own icons

Start with a square artwork of 1024×1024 px (e.g., `Assets/app.svg`) and export it into the formats above. For example:

- **ImageMagick** (`.ico` and PNGs):
  ```sh
  magick app.svg -background none -define icon:auto-resize=256,128,64,48,32,24,16 app.ico
  for s in 16 24 32 48 64 128 256 512; do magick app.svg -background none -resize ${s}x${s} linux/${s}x${s}.png; done
  ```
- **iconutil** on macOS (`.icns` from an `.iconset` folder):
  ```sh
  iconutil -c icns app.iconset -o app.icns
  ```

## Run and package

```sh
# Run during development
dotnet run

# Create a package, e.g., for Windows (see Properties/PublishProfiles for the others)
dotnet publish /p:PublishProfile=win-x64
```
