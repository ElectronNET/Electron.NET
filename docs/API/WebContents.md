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

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Windows containing web contents
- [Electron.Session](Session.md) - Session management for web contents
- [Electron.IpcMain](IpcMain.md) - Communication with web contents

## Additional Resources

- [Electron WebContents Documentation](https://electronjs.org/docs/api/web-contents) - Official Electron web contents API
- [Web Content Management](../Core/What's-New.md) - Understanding web content handling
- [Security Considerations](../Using/Configuration.md) - Secure web content integration
