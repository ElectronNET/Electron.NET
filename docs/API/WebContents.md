# Electron.WebContents

Render and control web pages.

## Overview

The `Electron.WebContents` API provides control over web page content within Electron windows. It handles page loading, navigation, JavaScript execution, and web page lifecycle events.

## Properties

#### 📋 `int Id`
Gets the unique identifier for this web contents.

#### 📋 `Session Session`
Manage browser sessions, cookies, cache, proxy settings, etc.

## Methods

#### 🧊 `void ExecuteJavaScriptAsync(string code, bool userGesture = false)`
Evaluates script code in page.

In the browser window some HTML APIs like `requestFullScreen` can only be invoked by a gesture from the user. Setting `userGesture` to `true` will remove this limitation.

Code execution will be suspended until web page stop loading.

**Parameters:**
- `code` - The code to execute
- `userGesture` - if set to `true` simulate a user gesture

**Returns:**

The result of the executed code.

#### 🧊 `Task<PrinterInfo[]> GetPrintersAsync()`
Get system printers.

**Returns:**

Array of available printers.

#### 🧊 `Task<string> GetUrl()`
Get the URL of the loaded page.

It's useful if a web-server redirects you and you need to know where it redirects. For instance, It's useful in case of Implicit Authorization.

**Returns:**

URL of the loaded page.

#### 🧊 `void InsertCSS(bool isBrowserWindow, string path)`
Inserts CSS into the web page.

See: https://www.electronjs.org/docs/api/web-contents#contentsinsertcsscss-options

Works for both BrowserWindows and BrowserViews.

**Parameters:**
- `isBrowserWindow` - Whether the webContents belong to a BrowserWindow or not (the other option is a BrowserView)
- `path` - Absolute path to the CSS file location

#### 🧊 `Task<string> InsertCSSAsync(string css, string cssOrigin = null)`
Injects CSS into the current web page and returns a unique key for the inserted style sheet.

**Parameters:**
- `css` - The style sheet to inject
- `cssOrigin` - Can be either `user` or `author`, defaults to `author`

**Returns:**

The key of the inserted style sheet, to be used with `RemoveInsertedCSSAsync`.

#### 🧊 `Task RemoveInsertedCSSAsync(string key)`
Removes a previously inserted style sheet from the current web page.

**Parameters:**
- `key` - The key returned by `InsertCSSAsync`

#### 🧊 `Task LoadURLAsync(string url)`
Loads the url in the window. The url must contain the protocol prefix.

The async method will resolve when the page has finished loading, and rejects if the page fails to load.

A noop rejection handler is already attached, which avoids unhandled rejection errors.

Loads the `url` in the window. The `url` must contain the protocol prefix, e.g. the `http://` or `file://`. If the load should bypass http cache then use the `pragma` header to achieve it.

**Parameters:**
- `url` - URL to load

#### 🧊 `Task LoadURLAsync(string url, LoadURLOptions options)`
Loads the url with additional options.

The async method will resolve when the page has finished loading, and rejects if the page fails to load.

A noop rejection handler is already attached, which avoids unhandled rejection errors.

Loads the `url` in the window. The `url` must contain the protocol prefix, e.g. the `http://` or `file://`. If the load should bypass http cache then use the `pragma` header to achieve it.

**Parameters:**
- `url` - URL to load
- `options` - Loading options

#### 🧊 `void OpenDevTools()`
Opens the devtools.

#### 🧊 `void OpenDevTools(OpenDevToolsOptions openDevToolsOptions)`
Opens the devtools with options.

**Parameters:**
- `openDevToolsOptions` - Developer tools options

#### 🧊 `Task<bool> PrintAsync(PrintOptions options = null)`
Prints window's web page.

**Parameters:**
- `options` - Print options

**Returns:**

Whether the print operation succeeded.

#### 🧊 `Task<bool> PrintToPDFAsync(string path, PrintToPDFOptions options = null)`
Prints window's web page as PDF with Chromium's preview printing custom settings.The landscape will be ignored if @page CSS at-rule is used in the web page. By default, an empty options will be regarded as: Use page-break-before: always; CSS style to force to print to a new page.

**Parameters:**
- `path` - Output file path
- `options` - PDF generation options

**Returns:**

Whether the PDF generation succeeded.

#### 🧊 `Task<double> GetZoomFactorAsync()`
Returns the current zoom factor. A factor of `1.0` means 100%.

#### 🧊 `void SetZoomFactor(double factor)`
Changes the zoom factor to the specified factor. The zoom factor is the zoom percent divided by 100, so 300% = `3.0`. The factor must be greater than `0.0`.

**Parameters:**
- `factor` - The zoom factor

#### 🧊 `Task<double> GetZoomLevelAsync()`
Returns the current zoom level.

#### 🧊 `void SetZoomLevel(double level)`
Changes the zoom level to the specified level. The original size is `0` and each increment above or below represents zooming 20% larger or smaller to default limits of 300% and 50% of the original size, respectively.

**Parameters:**
- `level` - The zoom level

#### 🧊 `Task SetVisualZoomLevelLimitsAsync(double minimumLevel, double maximumLevel)`
Sets the maximum and minimum pinch-to-zoom level.

**Parameters:**
- `minimumLevel` - The minimum pinch-to-zoom level
- `maximumLevel` - The maximum pinch-to-zoom level

#### 🧊 `Task LoadFileAsync(string filePath, LoadFileOptions options = null)`
Loads the given HTML file, relative to the root of the application.

**Parameters:**
- `filePath` - Path to the HTML file
- `options` - Optional `Query`, `Search` and `Hash` parts of the resulting URL

#### 🧊 `void SetAudioMuted(bool muted)`
Mutes or unmutes the audio on the current web page.

#### 🧊 `Task<bool> IsAudioMutedAsync()`
Whether this page has been muted.

#### 🧊 `Task<bool> IsCurrentlyAudibleAsync()`
Whether audio is currently playing.

#### 🧊 `Task<string> GetUserAgentAsync()` / `void SetUserAgent(string userAgent)`
Gets or overrides the user agent for this web page.

#### 🧊 `Task<bool> IsLoadingAsync()`
Whether the web page is still loading resources.

#### 🧊 `Task<bool> IsLoadingMainFrameAsync()`
Whether the main frame (and not just iframes or frames within it) is still loading.

#### 🧊 `Task<bool> IsWaitingForResponseAsync()`
Whether the web page is waiting for a first response from the main resource of the page.

#### 🧊 `void Reload()`
Reloads the current web page.

#### 🧊 `void ReloadIgnoringCache()`
Reloads the current web page and ignores the cache.

#### 🧊 `void Stop()`
Stops any pending navigation.

#### 🧊 `void Undo()` / `void Redo()`
Executes the `undo` / `redo` editing command in the web page.

#### 🧊 `void Cut()` / `void Copy()` / `void Paste()` / `void PasteAndMatchStyle()` / `void Delete()`
Executes the corresponding editing command in the web page.

#### 🧊 `void CopyImageAt(int x, int y)`
Copies the image at the given position to the clipboard.

#### 🧊 `void SelectAll()` / `void Unselect()` / `void CenterSelection()`
Selects all content, clears the selection, or scrolls to the current selection.

#### 🧊 `void ScrollToTop()` / `void ScrollToBottom()`
Scrolls to the top or the bottom of the current web page.

#### 🧊 `void AdjustSelection(AdjustSelectionOptions options)`
Adjusts the start and end points of the current text selection by the given amounts. Negative amounts move towards the beginning of the document.

#### 🧊 `Task InsertTextAsync(string text)`
Inserts text into the focused element.

#### 🧊 `void Replace(string text)` / `void ReplaceMisspelling(string text)`
Replaces the current selection, or the currently misspelled word, with the given text.

#### 🧊 `Task<int> FindInPageAsync(string text, FindInPageOptions options = null)`
Starts a request to find all matches for the text in the web page. Results are reported via the `OnFoundInPage` event.

**Parameters:**
- `text` - Content to be searched, must not be empty
- `options` - Optional `Forward`, `FindNext` and `MatchCase` flags

**Returns:**

The request id of the find request.

#### 🧊 `void StopFindInPage(StopFindInPageAction action)`
Stops any `FindInPageAsync` request with the given action (`ClearSelection`, `KeepSelection` or `ActivateSelection`).

## Events

#### ⚡ `InputEvent`
Emitted when an input event is sent to the WebContents.

#### ⚡ `OnCrashed`
Emitted when the renderer process crashes or is killed.

#### ⚡ `OnDidFailLoad`
Emitted when the load failed.

#### ⚡ `OnDidFinishLoad`
Emitted when the navigation is done, i.e. the spinner of the tab has stopped spinning, and the onload event was dispatched.

#### ⚡ `OnDidNavigate`
Emitted when a main frame navigation is done.

#### ⚡ `OnDidRedirectNavigation`
Emitted after a server side redirect occurs during navigation.

#### ⚡ `OnDidStartNavigation`
Emitted when any frame (including main) starts navigating.

#### ⚡ `OnDomReady`
Emitted when the document in the top-level frame is loaded.

#### ⚡ `OnWillRedirect`
Emitted when a server side redirect occurs during navigation.

#### ⚡ `OnZoomChanged`
Emitted when the user changes the zoom level using the mouse wheel or the keyboard. The handler receives the `ZoomDirection` (`In` or `Out`).

#### ⚡ `OnFoundInPage`
Emitted when a result is available for a `FindInPageAsync` request. The handler receives a `FoundInPageResult`.

#### ⚡ `OnAudioStateChanged`
Emitted when media becomes audible or inaudible. The handler receives `true` if one or more frames or child web contents are emitting audio.

#### ⚡ `OnMediaStartedPlaying`
Emitted when media starts playing.

#### ⚡ `OnMediaPaused`
Emitted when media is paused or done playing.

## Usage Examples

### Page Loading

```csharp
// Load URL with options
await webContents.LoadURLAsync("https://example.com", new LoadURLOptions
{
    UserAgent = "MyApp/1.0",
    ExtraHeaders = "Authorization: Bearer token123"
});

// Load local file
await webContents.LoadURLAsync("file://" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app/index.html"));

// Get current URL
var currentUrl = await webContents.GetUrl();
Console.WriteLine($"Current URL: {currentUrl}");
```

### JavaScript Execution

```csharp
// Execute simple JavaScript
var result = await webContents.ExecuteJavaScriptAsync("document.title");
Console.WriteLine($"Page title: {result}");

// Execute with user gesture simulation
await webContents.ExecuteJavaScriptAsync("document.requestFullscreen()", true);

// Execute complex code
var userAgent = await webContents.ExecuteJavaScriptAsync("navigator.userAgent");
Console.WriteLine($"User agent: {userAgent}");
```

### Developer Tools

```csharp
// Open dev tools
webContents.OpenDevTools();

// Open with specific options
webContents.OpenDevTools(new OpenDevToolsOptions
{
    Mode = DevToolsMode.Detached,
    Activate = true
});
```

### CSS Injection

```csharp
// Inject CSS file
webContents.InsertCSS(true, "styles/custom-theme.css");

// Inject CSS for BrowserView
webContents.InsertCSS(false, "styles/browser-view.css");
```

### Printing Operations

```csharp
// Print web page
var printSuccess = await webContents.PrintAsync(new PrintOptions
{
    Silent = false,
    PrintBackground = true,
    DeviceName = "My Printer"
});

if (printSuccess)
{
    Console.WriteLine("Print job sent successfully");
}
```

### PDF Generation

```csharp
// Generate PDF
var pdfSuccess = await webContents.PrintToPDFAsync("document.pdf", new PrintToPDFOptions
{
    MarginsType = PrintToPDFMarginsType.None,
    PageSize = PrintToPDFPageSize.A4,
    PrintBackground = true,
    Landscape = false
});

if (pdfSuccess)
{
    Console.WriteLine("PDF generated successfully");
}
```

### Navigation Monitoring

```csharp
// Monitor navigation events
webContents.OnDidStartNavigation += (url) =>
{
    Console.WriteLine($"Starting navigation to: {url}");
};

webContents.OnDidNavigate += (navInfo) =>
{
    Console.WriteLine($"Navigated to: {navInfo.Url}");
};

webContents.OnDidFinishLoad += () =>
{
    Console.WriteLine("Page finished loading");
};

webContents.OnDidFailLoad += (failInfo) =>
{
    Console.WriteLine($"Page failed to load: {failInfo.ErrorCode} - {failInfo.ErrorDescription}");
};
```

### Content Interaction

```csharp
// Wait for DOM ready
webContents.OnDomReady += () =>
{
    Console.WriteLine("DOM is ready");
    // Safe to execute DOM-related JavaScript now
};

// Handle page crashes
webContents.OnCrashed += (killed) =>
{
    Console.WriteLine($"Renderer crashed, killed: {killed}");
    // Optionally reload the page
};
```

### Zoom Control

```csharp
// Set the zoom to 150%
webContents.SetZoomFactor(1.5);

var factor = await webContents.GetZoomFactorAsync();
Console.WriteLine($"Current zoom: {factor * 100}%");

// Zoom levels are relative: each step is 20% larger/smaller, 0 is the original size
webContents.SetZoomLevel(2);

// Restrict pinch-to-zoom
await webContents.SetVisualZoomLevelLimitsAsync(1, 3);

// React to zoom changes triggered by the user
webContents.OnZoomChanged += (direction) =>
{
    Console.WriteLine($"User zoomed {direction}");
};
```

> **Note:** The zoom factor is shared by all windows using the same session partition. Assign a unique `WebPreferences.Partition` per window if each window should keep its own zoom.

### Dynamic CSS

```csharp
var key = await webContents.InsertCSSAsync("body { background-color: #202020; }");

// ... later
await webContents.RemoveInsertedCSSAsync(key);
```

### Editing and Selection

```csharp
await webContents.InsertTextAsync("Hello from .NET");

webContents.SelectAll();
webContents.Copy();
webContents.Unselect();
```

### Find in Page

```csharp
webContents.OnFoundInPage += (result) =>
{
    Console.WriteLine($"{result.ActiveMatchOrdinal}/{result.Matches} matches");

    if (result.FinalUpdate)
    {
        webContents.StopFindInPage(StopFindInPageAction.ClearSelection);
    }
};

var requestId = await webContents.FindInPageAsync("electron", new FindInPageOptions { MatchCase = false });
```

### Audio

```csharp
webContents.SetAudioMuted(true);

var muted = await webContents.IsAudioMutedAsync();
var audible = await webContents.IsCurrentlyAudibleAsync();

webContents.OnAudioStateChanged += (isAudible) =>
{
    Console.WriteLine(isAudible ? "Page started emitting audio" : "Page went silent");
};

webContents.OnMediaStartedPlaying += () => Console.WriteLine("Media playing");
webContents.OnMediaPaused += () => Console.WriteLine("Media paused");
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Windows containing web contents
- [Electron.Session](Session.md) - Session management for web contents
- [Electron.IpcMain](IpcMain.md) - Communication with web contents

## Additional Resources

- [Electron WebContents Documentation](https://electronjs.org/docs/api/web-contents) - Official Electron web contents API
- [Web Content Management](../Core/What's-New.md) - Understanding web content handling
- [Security Considerations](../Using/Configuration.md) - Secure web content integration
