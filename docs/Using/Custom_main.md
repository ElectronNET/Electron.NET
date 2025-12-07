# Using custom_main.js

This guide explains how to include and use a `custom_main.js` file in your Electron.NET application for advanced Electron/Node.js customization.

## Why use custom_main.js?

- Add custom Electron features (global shortcuts, tray icons, menus, etc.)
- Integrate Node.js modules (e.g., telemetry, OS APIs)
- Control startup logic (abort, environment checks)
- Set up IPC messaging or preload scripts

## Step-by-Step Process

### 1. Create the custom_main.js file

Place your custom logic in `electron/custom_main.js`:

```javascript
module.exports.onStartup = function(host) {
    // Example: Register a global shortcut for opening dev tools
    const { app, globalShortcut, BrowserWindow } = require('electron');
    app.on('ready', () => {
        const ret = globalShortcut.register('Control+Shift+I', () => {
            BrowserWindow.getAllWindows().forEach(win => win.webContents.openDevTools());
            console.log('Ctrl+Shift+I is pressed: DevTools opened!');
        });
    });
    app.on('will-quit', () => {
        globalShortcut.unregisterAll();
    });
    return true;
};
```

### 2. Configure your .csproj to copy custom_main.js to output

Add this to your `.csproj` file:

```xml
<ItemGroup>
  <None Update="electron\custom_main.js">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <TargetPath>.electron\custom_main.js</TargetPath>
  </None>
</ItemGroup>
```

### 3. Build and run your app

Use the standard build/run commands:

```powershell
dotnet build
dotnet run
```

Electron.NET will automatically load and execute your `custom_main.js` before initializing the .NET backend.

## Advanced Usage

Use environment variables to control features:

  ```javascript
  const env = process.env.ASPNETCORE_ENVIRONMENT || 'Production';
  if (env === 'Development') { /* enable dev features */ }
  ```

## Notes

- `custom_main.js` must use CommonJS syntax (`module.exports.onStartup = ...`).
- Place the file in your source directory and copy it to `.electron` using `.csproj`.
- Electron.NET will abort startup if `onStartup` returns `false`.

### Complete example is available here [ElectronNetSampleApp](https://github.com/niteshsinghal85/ElectronNetSampleApp)