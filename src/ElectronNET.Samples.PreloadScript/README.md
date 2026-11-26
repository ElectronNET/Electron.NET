# Electron.NET Sample: Preload Script

This sample shows how to inject a [preload script](https://www.electronjs.org/docs/latest/tutorial/tutorial-preload) into a `BrowserWindow` and use it to set up IPC messaging between the web content and the .NET app — with Node.js integration **disabled** and context isolation **enabled**.

## What it shows

| Direction | Renderer side (via `window.dotnet`) | .NET side |
| --- | --- | --- |
| Page → .NET | `ipcRenderer.send("greet", name)` | `Electron.IpcMain.On("greet", ...)` |
| Page → .NET → Page | `ipcRenderer.invoke("get-runtime-info")` | `Electron.IpcMain.Handle("get-runtime-info", ...)` |
| .NET → Page | `ipcRenderer.on("clock", ...)` | `Electron.IpcMain.Send(window, "clock", ...)` |

## How it works

1. **`Preload/preload.js`** uses `contextBridge.exposeInMainWorld` to expose a small, purpose-built `window.dotnet` API. The page never sees `ipcRenderer` itself.
2. **The `.csproj`** copies the preload script to the output directory. Electron loads preload scripts from the file system, not via HTTP, so it must not (only) live in `wwwroot`.
3. **`Program.cs`** registers the IPC handlers, then creates the window with an absolute path to the script:

   ```csharp
   WebPreferences = new WebPreferences
   {
       Preload = Path.Combine(AppContext.BaseDirectory, "Preload", "preload.js"),
       NodeIntegration = false,
       ContextIsolation = true,
       Sandbox = true,
   }
   ```

   `AppContext.BaseDirectory` resolves correctly both when debugging and in a packaged app (where the .NET app is placed in `resources/bin`).
4. **`wwwroot/app.js`** is a plain page script that only talks to `window.dotnet`. If the page is opened in a normal browser, it detects that the API is missing.

> [!NOTE]
> Setting `NodeIntegration` explicitly matters: if `WebPreferences` do not specify it, Electron.NET enables Node.js integration and disables context isolation for backwards compatibility.

> [!NOTE]
> With `Sandbox = true`, the preload script can only `require` a limited set of modules (such as `electron`'s `contextBridge` and `ipcRenderer`). Set it to `false` if your preload script needs full Node.js access.

## Run

```sh
dotnet run
```
