# Electron.Notification

Show native desktop notifications with custom content and actions.

## Overview

The `Electron.Notification` API provides the ability to show native desktop notifications with custom titles, bodies, icons, and actions. Notifications work across Windows, macOS, and Linux with platform-specific behavior.

## Methods

#### 🧊 `Task<bool> IsSupportedAsync()`
Check if desktop notifications are supported on the current platform.

**Returns:**

Whether or not desktop notifications are supported on the current system.

#### 🧊 `void Show(NotificationOptions notificationOptions)`
Create OS desktop notifications with the specified options.

**Parameters:**
- `notificationOptions` - Notification configuration options

## Usage Examples

### Basic Notification

```csharp
// Simple notification
Electron.Notification.Show(new NotificationOptions
{
    Title = "My Application",
    Body = "This is a notification message",
    Icon = "assets/notification-icon.png"
});
```

### Notification with Actions

```csharp
// Notification with reply action
Electron.Notification.Show(new NotificationOptions
{
    Title = "New Message",
    Body = "You have a new message from John",
    Icon = "assets/message-icon.png",
    Actions = new[]
    {
        new NotificationAction { Text = "Reply", Type = NotificationActionType.Button },
        new NotificationAction { Text = "View", Type = NotificationActionType.Button }
    },
    OnClick = () => OpenMessageWindow(),
    OnAction = (action) =>
    {
        if (action == "Reply")
        {
            ShowReplyDialog();
        }
        else if (action == "View")
        {
            OpenMessageWindow();
        }
    }
});
```

### Rich Notifications

```csharp
// Rich notification with all options
Electron.Notification.Show(new NotificationOptions
{
    Title = "Download Complete",
    Subtitle = "Your file has finished downloading",
    Body = "document.pdf has been downloaded to your Downloads folder.",
    Icon = "assets/download-icon.png",
    ImageUrl = "file://" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/preview.png"),
    Sound = NotificationSound.Default,
    Urgency = NotificationUrgency.Normal,
    Category = "transfer.complete",
    Tag = "download-123",
    Actions = new[]
    {
        new NotificationAction { Text = "Open", Type = NotificationActionType.Button },
        new NotificationAction { Text = "Show in Folder", Type = NotificationActionType.Button }
    },
    OnShow = () => Console.WriteLine("Notification shown"),
    OnClick = () => OpenDownloadedFile(),
    OnClose = () => Console.WriteLine("Notification closed"),
    OnAction = (action) =>
    {
        if (action == "Open")
        {
            OpenDownloadedFile();
        }
        else if (action == "Show in Folder")
        {
            ShowInFolder();
        }
    }
});
```

### Platform-Specific Notifications

```csharp
// Windows toast notification
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Electron.Notification.Show(new NotificationOptions
    {
        Title = "Background Task",
        Body = "Your backup is complete",
        Icon = "assets/app-icon.ico",
        Tag = "backup-complete",
        RequireInteraction = true
    });
}

// macOS notification with sound
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    Electron.Notification.Show(new NotificationOptions
    {
        Title = "Alert",
        Body = "Something needs your attention",
        Sound = NotificationSound.Default,
        Actions = new[]
        {
            new NotificationAction { Text = "View", Type = NotificationActionType.Button }
        }
    });
}
```

### Notification Management

```csharp
// Check notification support
var isSupported = await Electron.Notification.IsSupportedAsync();
Console.WriteLine($"Notifications supported: {isSupported}");

// Create notification with events
var notification = new NotificationOptions
{
    Title = "Task Complete",
    Body = "Your long-running task has finished",
    OnShow = () => Console.WriteLine("Notification displayed"),
    OnClick = () => OpenTaskResults(),
    OnClose = () => Console.WriteLine("Notification dismissed")
};

Electron.Notification.Show(notification);
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle events
- [Electron.Tray](Tray.md) - System tray integration with notifications
- [Electron.Screen](Screen.md) - Position notifications based on screen layout

## Additional Resources

- [Electron Notification Documentation](https://electronjs.org/docs/api/notification) - Official Electron notification API
