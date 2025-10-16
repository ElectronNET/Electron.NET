# Electron.WindowManager

Create and manage browser windows, control window behavior and appearance.

## Overview

The `Electron.WindowManager` API provides comprehensive control over browser windows in your Electron application. It handles window creation, management, and coordination with the application lifecycle.

## Properties

#### 📋 `IReadOnlyCollection<BrowserView> BrowserViews`
Gets a read-only collection of all currently open browser views.

#### 📋 `IReadOnlyCollection<BrowserWindow> BrowserWindows`
Gets a read-only collection of all currently open browser windows.

#### 📋 `bool IsQuitOnWindowAllClosed`
Controls whether the application quits when all windows are closed. Default is true.

## Methods

#### 🧊 `Task<BrowserView> CreateBrowserViewAsync()`
Creates a new browser view with default options.

**Returns:**

The created BrowserView instance.

#### 🧊 `Task<BrowserView> CreateBrowserViewAsync(BrowserViewConstructorOptions options)`
Creates a new browser view with custom options.

**Parameters:**
- `options` - Browser view configuration options

**Returns:**

The created BrowserView instance.

#### 🧊 `Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost")`
Creates a new browser window with default options.

**Parameters:**
- `loadUrl` - URL to load in the window. Defaults to "http://localhost"

**Returns:**

The created BrowserWindow instance.

#### 🧊 `Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost")`
Creates a new browser window with custom options.

**Parameters:**
- `options` - Window configuration options
- `loadUrl` - URL to load in the window. Defaults to "http://localhost"

**Returns:**

The created BrowserWindow instance.

## Usage Examples

### Basic Window Creation

```csharp
// Create window with default options
var mainWindow = await Electron.WindowManager.CreateWindowAsync();

// Create window with custom options
var settingsWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
    Width = 800,
    Height = 600,
    Show = false,
    Title = "Settings",
    WebPreferences = new WebPreferences
    {
        NodeIntegration = false,
        ContextIsolation = true
    }
}, "https://localhost:5001/settings");
```

### Window Management

```csharp
// Get all windows
var windows = Electron.WindowManager.BrowserWindows;
Console.WriteLine($"Open windows: {windows.Count}");

// Configure quit behavior
Electron.WindowManager.IsQuitOnWindowAllClosed = false; // Keep app running when windows close

// Handle window lifecycle
Electron.App.WindowAllClosed += () =>
{
    Console.WriteLine("All windows closed");
    if (Electron.WindowManager.IsQuitOnWindowAllClosed)
    {
        Electron.App.Quit();
    }
};
```

### Browser View Integration

```csharp
// Create browser view
var browserView = await Electron.WindowManager.CreateBrowserViewAsync(new BrowserViewConstructorOptions
{
    WebPreferences = new WebPreferences
    {
        NodeIntegration = false,
        ContextIsolation = true
    }
});

// Add to window
await mainWindow.SetBrowserViewAsync(browserView);
await browserView.WebContents.LoadURLAsync("https://example.com");

// Set view bounds
await mainWindow.SetBoundsAsync(browserView, new Rectangle
{
    X = 0,
    Y = 100,
    Width = 800,
    Height = 400
});
```

### Window Options Configuration

```csharp
// Comprehensive window options
var options = new BrowserWindowOptions
{
    Width = 1200,
    Height = 800,
    MinWidth = 600,
    MinHeight = 400,
    MaxWidth = 1920,
    MaxHeight = 1080,
    X = 100,
    Y = 100,
    Center = true,
    Frame = true,
    Title = "My Application",
    Icon = "assets/app-icon.png",
    Show = false,
    AlwaysOnTop = false,
    SkipTaskbar = false,
    Kiosk = false,
    TitleBarStyle = TitleBarStyle.Default,
    BackgroundColor = "#FFFFFF",
    DarkTheme = false,
    Transparent = false,
    WebPreferences = new WebPreferences
    {
        NodeIntegration = false,
        ContextIsolation = true,
        EnableWebSQL = false,
        Partition = "persist:electron",
        ZoomFactor = 1.0f,
        DevTools = true
    }
};
```

### Multi-Window Applications

```csharp
// Create main window
var mainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
    Width = 1200,
    Height = 800,
    Show = false
});

// Create secondary window
var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
    Width = 600,
    Height = 400,
    Parent = mainWindow,
    Modal = true,
    Show = false
});

// Load different content
await mainWindow.WebContents.LoadURLAsync("https://localhost:5001");
await secondaryWindow.WebContents.LoadURLAsync("https://localhost:5001/settings");

// Show windows when ready
mainWindow.OnReadyToShow += () => mainWindow.Show();
secondaryWindow.OnReadyToShow += () => secondaryWindow.Show();
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle and window events
- [Electron.Dialog](Dialog.md) - Parent windows for modal dialogs
- [Electron.Menu](Menu.md) - Window-specific menus
- [Electron.WebContents](WebContents.md) - Window content management

## Additional Resources

- [Electron Window Management Documentation](https://electronjs.org/docs/api/browser-window) - Official Electron window API
