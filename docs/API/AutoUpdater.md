# Electron.AutoUpdater

Handle application updates and installation processes.

## Overview

The `Electron.AutoUpdater` API provides comprehensive functionality for handling application updates, including checking for updates, downloading, and installation. It supports multiple update providers and platforms with automatic update capabilities.

## Properties

#### 📋 `bool AllowDowngrade`
Whether to allow version downgrade. Default is false.

#### 📋 `bool AllowPrerelease`
GitHub provider only. Whether to allow update to pre-release versions. Defaults to true if application version contains prerelease components.

#### 📋 `bool AutoDownload`
Whether to automatically download an update when it is found. Default is true.

#### 📋 `bool AutoInstallOnAppQuit`
Whether to automatically install a downloaded update on app quit. Applicable only on Windows and Linux.

#### 📋 `string Channel`
Get the update channel. Not applicable for GitHub. Doesn't return channel from the update configuration, only if was previously set.

#### 📋 `Task<string> ChannelAsync`
Get the update channel. Not applicable for GitHub. Doesn't return channel from the update configuration, only if was previously set.

#### 📋 `Task<SemVer> CurrentVersionAsync`
Get the current application version.

#### 📋 `bool FullChangelog`
GitHub provider only. Get all release notes (from current version to latest), not just the latest. Default is false.

#### 📋 `Dictionary<string, string> RequestHeaders`
The request headers.

#### 📋 `Task<Dictionary<string, string>> RequestHeadersAsync`
Get the current request headers.

#### 📋 `string UpdateConfigPath`
For test only. Configuration path for updates.

## Methods

#### 🧊 `Task<UpdateCheckResult> CheckForUpdatesAndNotifyAsync()`
Asks the server whether there is an update and notifies the user if an update is available.

#### 🧊 `Task<UpdateCheckResult> CheckForUpdatesAsync()`
Asks the server whether there is an update.

#### 🧊 `Task<string> DownloadUpdateAsync()`
Start downloading update manually. Use this method if AutoDownload option is set to false.

**Returns:**

Path to downloaded file.

#### 🧊 `Task<string> GetFeedURLAsync()`
Get the current feed URL.

**Returns:**

Feed URL.

#### 🧊 `void QuitAndInstall(bool isSilent = false, bool isForceRunAfter = false)`
Restarts the app and installs the update after it has been downloaded. Should only be called after update-downloaded has been emitted.

Note: QuitAndInstall() will close all application windows first and only emit before-quit event on app after that. This is different from the normal quit event sequence.

**Parameters:**
- `isSilent` - Windows-only: Runs the installer in silent mode. Defaults to false
- `isForceRunAfter` - Run the app after finish even on silent install. Not applicable for macOS

## Events

#### ⚡ `OnCheckingForUpdate`
Emitted when checking if an update has started.

#### ⚡ `OnDownloadProgress`
Emitted on download progress.

#### ⚡ `OnError`
Emitted when there is an error while updating.

#### ⚡ `OnUpdateAvailable`
Emitted when there is an available update. The update is downloaded automatically if AutoDownload is true.

#### ⚡ `OnUpdateDownloaded`
Emitted on download complete.

#### ⚡ `OnUpdateNotAvailable`
Emitted when there is no available update.

## Usage Examples

### Basic Auto-Update Setup

```csharp
// Configure auto-updater
Electron.AutoUpdater.AutoDownload = true;
Electron.AutoUpdater.AutoInstallOnAppQuit = true;

// Check for updates
var updateCheck = await Electron.AutoUpdater.CheckForUpdatesAsync();
if (updateCheck.UpdateInfo != null)
{
    Console.WriteLine($"Update available: {updateCheck.UpdateInfo.Version}");
}
```

### Manual Update Management

```csharp
// Disable auto-download for manual control
Electron.AutoUpdater.AutoDownload = false;

// Check for updates
var result = await Electron.AutoUpdater.CheckForUpdatesAsync();
if (result.UpdateInfo != null)
{
    Console.WriteLine($"Update found: {result.UpdateInfo.Version}");

    // Ask user confirmation
    var confirmResult = await Electron.Dialog.ShowMessageBoxAsync(
        "Update Available",
        $"Version {result.UpdateInfo.Version} is available. Download now?");

    if (confirmResult.Response == 1) // Yes
    {
        // Download update manually
        var downloadPath = await Electron.AutoUpdater.DownloadUpdateAsync();
        Console.WriteLine($"Downloaded to: {downloadPath}");

        // Install update
        Electron.AutoUpdater.QuitAndInstall();
    }
}
```

### Update Event Handling

```csharp
// Handle update events
Electron.AutoUpdater.OnCheckingForUpdate += () =>
{
    Console.WriteLine("Checking for updates...");
};

Electron.AutoUpdater.OnUpdateAvailable += (updateInfo) =>
{
    Console.WriteLine($"Update available: {updateInfo.Version}");
};

Electron.AutoUpdater.OnUpdateNotAvailable += (updateInfo) =>
{
    Console.WriteLine("No updates available");
};

Electron.AutoUpdater.OnDownloadProgress += (progressInfo) =>
{
    Console.WriteLine($"Download progress: {progressInfo.Percent}%");
};

Electron.AutoUpdater.OnUpdateDownloaded += (updateInfo) =>
{
    Console.WriteLine($"Update downloaded: {updateInfo.Version}");

    // Show notification to user
    Electron.Notification.Show(new NotificationOptions
    {
        Title = "Update Downloaded",
        Body = $"Version {updateInfo.Version} is ready to install.",
        Actions = new[]
        {
            new NotificationAction { Text = "Install Now", Type = NotificationActionType.Button },
            new NotificationAction { Text = "Later", Type = NotificationActionType.Button }
        }
    });
};

Electron.AutoUpdater.OnError += (error) =>
{
    Console.WriteLine($"Update error: {error}");
    Electron.Dialog.ShowErrorBox("Update Error", $"Failed to update: {error}");
};
```

### GitHub Provider Configuration

```csharp
// Configure for GitHub releases
Electron.AutoUpdater.AllowPrerelease = true; // Allow pre-release versions
Electron.AutoUpdater.FullChangelog = true;  // Get full changelog
Electron.AutoUpdater.AllowDowngrade = false; // Prevent downgrades

// Set request headers if needed
Electron.AutoUpdater.RequestHeaders = new Dictionary<string, string>
{
    ["Authorization"] = "token your-github-token",
    ["User-Agent"] = "MyApp-Updater"
};
```

### Update Installation

```csharp
// Install update immediately
if (updateDownloaded)
{
    Electron.AutoUpdater.QuitAndInstall();
}

// Silent install (Windows only)
Electron.AutoUpdater.QuitAndInstall(isSilent: true, isForceRunAfter: true);
```

### Version Management

```csharp
// Get current version
var currentVersion = await Electron.AutoUpdater.CurrentVersionAsync;
Console.WriteLine($"Current version: {currentVersion}");

// Get update channel
var channel = await Electron.AutoUpdater.ChannelAsync;
Console.WriteLine($"Update channel: {channel}");

// Set custom feed URL
// Note: This would typically be configured in electron-builder.json
var feedUrl = await Electron.AutoUpdater.GetFeedURLAsync();
Console.WriteLine($"Feed URL: {feedUrl}");
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle events during updates
- [Electron.Notification](Notification.md) - Notify users about update status
- [Electron.Dialog](Dialog.md) - Show update confirmation dialogs

## Additional Resources

- [Electron AutoUpdater Documentation](https://electronjs.org/docs/api/auto-updater) - Official Electron auto-updater API
