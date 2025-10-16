# Electron.Tray

Add icons and context menus to the system's notification area.

## Overview

The `Electron.Tray` API provides the ability to add icons and context menus to the system's notification area (system tray). This allows applications to provide quick access to common functions and maintain a presence in the system even when windows are closed.

## Properties

#### 📋 `IReadOnlyCollection<MenuItem> MenuItems`
Gets a read-only collection of all current tray menu items.

## Methods

#### 🧊 `void Destroy()`
Destroys the tray icon immediately.

#### 🧊 `void DisplayBalloon(DisplayBalloonOptions options)`
Windows: Displays a tray balloon notification.

**Parameters:**
- `options` - Balloon notification options

#### 🧊 `Task<bool> IsDestroyedAsync()`
Check if the tray icon has been destroyed.

**Returns:**

Whether the tray icon is destroyed.

#### 🧊 `void SetImage(string image)`
Sets the image associated with this tray icon.

**Parameters:**
- `image` - New image for the tray icon

#### 🧊 `void SetPressedImage(string image)`
Sets the image associated with this tray icon when pressed on macOS.

**Parameters:**
- `image` - Image for pressed state

#### 🧊 `void SetTitle(string title)`
macOS: Sets the title displayed aside of the tray icon in the status bar.

**Parameters:**
- `title` - Title text

#### 🧊 `void SetToolTip(string toolTip)`
Sets the hover text for this tray icon.

**Parameters:**
- `toolTip` - Tooltip text

#### 🧊 `void Show(string image)`
Shows the tray icon without a context menu.

**Parameters:**
- `image` - The image to use for the tray icon

#### 🧊 `void Show(string image, MenuItem menuItem)`
Shows the tray icon with a single menu item.

**Parameters:**
- `image` - The image to use for the tray icon
- `menuItem` - Single menu item for the tray context menu

#### 🧊 `void Show(string image, MenuItem[] menuItems)`
Shows the tray icon with multiple menu items.

**Parameters:**
- `image` - The image to use for the tray icon
- `menuItems` - Array of menu items for the tray context menu

## Events

#### ⚡ `OnBalloonClick`
Windows: Emitted when the tray balloon is clicked.

#### ⚡ `OnBalloonClosed`
Windows: Emitted when the tray balloon is closed because of timeout or user manually closes it.

#### ⚡ `OnBalloonShow`
Windows: Emitted when the tray balloon shows.

#### ⚡ `OnClick`
Emitted when the tray icon is clicked.

#### ⚡ `OnDoubleClick`
macOS, Windows: Emitted when the tray icon is double clicked.

#### ⚡ `OnRightClick`
macOS, Windows: Emitted when the tray icon is right clicked.

## Usage Examples

### Basic Tray Icon

```csharp
// Simple tray icon
await Electron.Tray.Show("assets/tray-icon.png");

// Tray icon with single menu item
await Electron.Tray.Show("assets/tray-icon.png", new MenuItem
{
    Label = "Show Window",
    Click = () => ShowMainWindow()
});
```

### Tray with Context Menu

```csharp
// Tray with multiple menu items
var trayMenuItems = new[]
{
    new MenuItem { Label = "Show Window", Click = () => ShowMainWindow() },
    new MenuItem { Label = "Settings", Click = () => OpenSettings() },
    new MenuItem { Type = MenuType.Separator },
    new MenuItem { Label = "Exit", Click = () => Electron.App.Quit() }
};

await Electron.Tray.Show("assets/tray-icon.png", trayMenuItems);
```

### Dynamic Tray Updates

```csharp
// Update tray tooltip based on status
await Electron.Tray.SetToolTip("MyApp - Connected");

// Change tray icon based on state
if (isConnected)
{
    await Electron.Tray.SetImage("assets/connected.png");
}
else
{
    await Electron.Tray.SetImage("assets/disconnected.png");
}
```

### Tray Event Handling

```csharp
// Handle tray clicks
Electron.Tray.OnClick += (clickArgs, bounds) =>
{
    if (clickArgs.AltKey || clickArgs.ShiftKey)
    {
        // Alt+Click or Shift+Click - show context menu
        Electron.Menu.ContextMenuPopup(Electron.WindowManager.BrowserWindows.First());
    }
    else
    {
        // Regular click - toggle main window
        ToggleMainWindow();
    }
};

Electron.Tray.OnRightClick += (clickArgs, bounds) =>
{
    // Show context menu on right click
    Electron.Menu.ContextMenuPopup(Electron.WindowManager.BrowserWindows.First());
};
```

### Windows Balloon Notifications

```csharp
// Show Windows balloon notification
await Electron.Tray.DisplayBalloon(new DisplayBalloonOptions
{
    Title = "Background Task Complete",
    Content = "Your file has been processed successfully.",
    Icon = "assets/notification-icon.ico"
});

// Handle balloon events
Electron.Tray.OnBalloonClick += () =>
{
    ShowMainWindow();
    Electron.WindowManager.BrowserWindows.First().Focus();
};
```

### macOS Tray Features

```csharp
// macOS specific tray features
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    await Electron.Tray.SetTitle("MyApp");

    // Use template image for dark mode support
    await Electron.Tray.SetImage("assets/tray-template.png");
    await Electron.Tray.SetPressedImage("assets/tray-pressed-template.png");
}
```

### Application Integration

```csharp
// Integrate with application lifecycle
Electron.App.WindowAllClosed += () =>
{
    // Keep app running in tray when windows are closed
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        Electron.App.Hide();
    }
};

// Handle tray double-click
Electron.Tray.OnDoubleClick += (clickArgs, bounds) =>
{
    ShowMainWindow();
    Electron.WindowManager.BrowserWindows.First().Focus();
};
```

## Related APIs

- [Electron.Menu](Menu.md) - Context menus for tray icons
- [Electron.Notification](Notification.md) - Desktop notifications
- [Electron.App](App.md) - Application lifecycle events
- [Electron.WindowManager](WindowManager.md) - Windows to show/hide from tray

## Additional Resources

- [Electron Tray Documentation](https://electronjs.org/docs/api/tray) - Official Electron tray API
