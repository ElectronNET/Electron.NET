# Electron.Dock

Control your app in the macOS dock.

## Overview

The `Electron.Dock` API provides control over your application's appearance and behavior in the macOS dock. This includes bouncing the dock icon, setting badges, managing menus, and controlling visibility.

## Properties

#### 📋 `IReadOnlyCollection<MenuItem> MenuItems`
Gets a read-only collection of all current dock menu items.

## Methods

#### 🧊 `Task<int> BounceAsync(DockBounceType type, CancellationToken cancellationToken = default)`
When `DockBounceType.Critical` is passed, the dock icon will bounce until either the application becomes active or the request is canceled. When `DockBounceType.Informational` is passed, the dock icon will bounce for one second. However, the request remains active until either the application becomes active or the request is canceled.

Note: This method can only be used while the app is not focused; when the app is focused it will return -1.

**Parameters:**
- `type` - Can be critical or informational. The default is informational.
- `cancellationToken` - The cancellation token

**Returns:**

An ID representing the request.

#### 🧊 `void CancelBounce(int id)`
Cancel the bounce of id.

**Parameters:**
- `id` - Id of the request

#### 🧊 `void DownloadFinished(string filePath)`
Bounces the Downloads stack if the filePath is inside the Downloads folder.

**Parameters:**
- `filePath` - Path to the downloaded file

#### 🧊 `Task<string> GetBadgeAsync(CancellationToken cancellationToken = default)`
Gets the string to be displayed in the dock's badging area.

**Returns:**

The badge string of the dock.

#### 🧊 `Task<Menu> GetMenu(CancellationToken cancellationToken = default)`
Gets the application's dock menu.

**Returns:**

The application's dock menu.

#### 🧊 `void Hide()`
Hides the dock icon.

#### 🧊 `Task<bool> IsVisibleAsync(CancellationToken cancellationToken = default)`
Whether the dock icon is visible. The app.dock.show() call is asynchronous so this method might not return true immediately after that call.

**Returns:**

Whether the dock icon is visible.

#### 🧊 `void SetBadge(string text)`
Sets the string to be displayed in the dock's badging area.

**Parameters:**
- `text` - Badge text to display

#### 🧊 `void SetIcon(string image)`
Sets the image associated with this dock icon.

**Parameters:**
- `image` - Icon image path

#### 🧊 `void SetMenu(MenuItem[] menuItems)`
Sets the application's dock menu.

**Parameters:**
- `menuItems` - Array of menu items for the dock menu

#### 🧊 `void Show()`
Shows the dock icon.

## Usage Examples

### Basic Dock Operations

```csharp
// Hide/Show dock icon
Electron.Dock.Hide();
await Task.Delay(2000);
Electron.Dock.Show();

// Check visibility
var isVisible = await Electron.Dock.IsVisibleAsync();
Console.WriteLine($"Dock visible: {isVisible}");
```

### Badge Notifications

```csharp
// Set badge count
Electron.Dock.SetBadge("5");

// Get current badge
var badge = await Electron.Dock.GetBadgeAsync();
Console.WriteLine($"Current badge: {badge}");

// Clear badge
Electron.Dock.SetBadge("");
```

### Dock Icon Animation

```csharp
// Bounce for attention
var bounceId = await Electron.Dock.BounceAsync(DockBounceType.Critical);
Console.WriteLine($"Bounce ID: {bounceId}");

// Cancel bounce after 3 seconds
await Task.Delay(3000);
Electron.Dock.CancelBounce(bounceId);

// Informational bounce
await Electron.Dock.BounceAsync(DockBounceType.Informational);
```

### Dock Menu

```csharp
// Create dock menu
var dockMenuItems = new[]
{
    new MenuItem { Label = "Show Window", Click = () => ShowMainWindow() },
    new MenuItem { Label = "Settings", Click = () => OpenSettings() },
    new MenuItem { Type = MenuType.Separator },
    new MenuItem { Label = "Exit", Click = () => Electron.App.Quit() }
};

// Set dock menu
Electron.Dock.SetMenu(dockMenuItems);

// Get current menu
var currentMenu = await Electron.Dock.GetMenu();
Console.WriteLine($"Menu items: {Electron.Dock.MenuItems.Count}");
```

### Download Notifications

```csharp
// Notify about completed download
var downloadPath = "/Users/username/Downloads/document.pdf";
Electron.Dock.DownloadFinished(downloadPath);
```

### Custom Dock Icon

```csharp
// Set custom dock icon
Electron.Dock.SetIcon("assets/custom-dock-icon.png");

// Set icon based on status
if (isConnected)
{
    Electron.Dock.SetIcon("assets/connected-icon.png");
}
else
{
    Electron.Dock.SetIcon("assets/disconnected-icon.png");
}
```

### Application Integration

```csharp
// Update dock badge based on unread count
UpdateDockBadge(unreadMessageCount);

void UpdateDockBadge(int count)
{
    if (count > 0)
    {
        Electron.Dock.SetBadge(count.ToString());
    }
    else
    {
        Electron.Dock.SetBadge("");
    }
}

// Animate dock when receiving message
private async void OnMessageReceived()
{
    await Electron.Dock.BounceAsync(DockBounceType.Informational);
    Electron.Dock.SetBadge((unreadCount + 1).ToString());
}
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle events
- [Electron.Notification](Notification.md) - Desktop notifications
- [Electron.Menu](Menu.md) - Menu items for dock menu

## Additional Resources

- [Electron Dock Documentation](https://electronjs.org/docs/api/dock) - Official Electron dock API
