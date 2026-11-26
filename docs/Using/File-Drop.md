# Getting File Paths (File Drop)

For security reasons, browsers do not expose the file system path of a `File` object, e.g., one obtained via drag and drop or an `<input type="file">` element. Electron provides [`webUtils.getPathForFile`](https://www.electronjs.org/docs/latest/api/web-utils#webutilsgetpathforfilefile) for this, but it is only accessible from a preload script.

Electron.NET registers a small preload script for every session, which exposes this function as `window.electronNET.getPathForFile`. It works regardless of the `NodeIntegration`, `ContextIsolation`, and `Sandbox` settings of the window, and it runs side by side with your own `WebPreferences.Preload` script.

`getPathForFile` returns an empty string for files that are not backed by the file system (e.g., created via `new File()`).

## Plain JavaScript

```html
<div id="drop-zone">Drop files here</div>

<script>
    const dropZone = document.getElementById('drop-zone');

    dropZone.addEventListener('dragover', (e) => e.preventDefault());

    dropZone.addEventListener('drop', (e) => {
        e.preventDefault();

        for (const file of e.dataTransfer.files) {
            const path = window.electronNET.getPathForFile(file);
            console.log(path);
        }
    });
</script>
```

To process the paths in .NET, send them to the main process, e.g., via `ipcRenderer.send` (with node integration) and `Electron.IpcMain.On`, or post them to an endpoint of your ASP.NET Core app.

## Blazor

In Blazor, the `drop` event does not give you access to the `File` objects, so a small JavaScript helper is used to forward the paths to .NET:

```javascript
// wwwroot/js/fileDrop.js
window.registerFileDrop = (element, dotNetRef) => {
    element.addEventListener('dragover', (e) => e.preventDefault());
    element.addEventListener('drop', (e) => {
        e.preventDefault();
        const paths = Array.from(e.dataTransfer.files)
            .map((file) => window.electronNET.getPathForFile(file))
            .filter((path) => path);
        dotNetRef.invokeMethodAsync('OnFilesDropped', paths);
    });
};
```

```razor
@inject IJSRuntime JS
@implements IDisposable

<div @ref="dropZone" class="drop-zone">Drop files here</div>

<ul>
    @foreach (var path in paths)
    {
        <li>@path</li>
    }
</ul>

@code {
    private ElementReference dropZone;
    private DotNetObjectReference<FileDropZone>? selfRef;
    private string[] paths = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            selfRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("registerFileDrop", dropZone, selfRef);
        }
    }

    [JSInvokable]
    public void OnFilesDropped(string[] droppedPaths)
    {
        paths = droppedPaths;
        StateHasChanged();
    }

    public void Dispose() => selfRef?.Dispose();
}
```

> [!NOTE]
> Dropping a file anywhere outside an element that calls `preventDefault()` on `dragover` / `drop` makes Electron navigate to that file. Handle these events on `document` if you want to prevent this globally.
