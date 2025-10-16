# Electron.Shell

Desktop integration for opening files, URLs, and accessing system paths.

## Overview

The `Electron.Shell` API provides system integration functionality for opening files and URLs with their default applications, managing trash/recycle bin, and creating/reading shortcut links.

## Methods

#### 🧊 `void Beep()`
Play the beep sound.

#### 🧊 `Task<string> OpenExternalAsync(string url)`
Open the given external protocol URL in the desktop's default manner (e.g., mailto: URLs in the user's default mail agent).

**Parameters:**
- `url` - Max 2081 characters on windows

**Returns:**

The error message corresponding to the failure if a failure occurred, otherwise empty string.

#### 🧊 `Task<string> OpenExternalAsync(string url, OpenExternalOptions options)`
Open the given external protocol URL with additional options.

**Parameters:**
- `url` - Max 2081 characters on windows
- `options` - Controls the behavior of OpenExternal

**Returns:**

The error message corresponding to the failure if a failure occurred, otherwise empty string.

#### 🧊 `Task<string> OpenPathAsync(string path)`
Open the given file in the desktop's default manner.

**Parameters:**
- `path` - The path to the directory or file

**Returns:**

The error message corresponding to the failure if a failure occurred, otherwise empty string.

#### 🧊 `Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)`
Resolves the shortcut link at shortcutPath. An exception will be thrown when any error happens.

**Parameters:**
- `shortcutPath` - The path to the shortcut

**Returns:**

ShortcutDetails of the shortcut.

#### 🧊 `Task ShowItemInFolderAsync(string fullPath)`
Show the given file in a file manager. If possible, select the file.

**Parameters:**
- `fullPath` - The full path to the directory or file

#### 🧊 `Task<bool> TrashItemAsync(string fullPath)`
Move the given file to trash and returns a bool status for the operation.

**Parameters:**
- `fullPath` - The full path to the directory or file

**Returns:**

Whether the item was successfully moved to the trash.

#### 🧊 `Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)`
Creates or updates a shortcut link at shortcutPath.

**Parameters:**
- `shortcutPath` - The path to the shortcut
- `operation` - Default is ShortcutLinkOperation.Create
- `options` - Structure of a shortcut

**Returns:**

Whether the shortcut was created successfully.

## Usage Examples

### File Operations

```csharp
// Open file with default application
var error = await Electron.Shell.OpenPathAsync(filePath);
if (string.IsNullOrEmpty(error))
{
    Console.WriteLine("File opened successfully");
}
else
{
    Console.WriteLine($"Failed to open file: {error}");
}

// Show file in file manager
await Electron.Shell.ShowItemInFolderAsync(filePath);

// Move file to trash
var trashed = await Electron.Shell.TrashItemAsync(filePath);
Console.WriteLine($"File trashed: {trashed}");
```

### URL Operations

```csharp
// Open URL in default browser
var error = await Electron.Shell.OpenExternalAsync("https://electron.net");
if (!string.IsNullOrEmpty(error))
{
    Console.WriteLine($"Failed to open URL: {error}");
}

// Open email client
await Electron.Shell.OpenExternalAsync("mailto:user@example.com");

// Open with options
var error = await Electron.Shell.OpenExternalAsync("https://example.com", new OpenExternalOptions
{
    Activate = true
});
```

### System Integration

```csharp
// Play system beep
Electron.Shell.Beep();

// Create desktop shortcut
var success = await Electron.Shell.WriteShortcutLinkAsync(
    @"C:\Users\Public\Desktop\MyApp.lnk",
    ShortcutLinkOperation.Create,
    new ShortcutDetails
    {
        Target = "C:\\Program Files\\MyApp\\MyApp.exe",
        Description = "My Application",
        WorkingDirectory = "C:\\Program Files\\MyApp"
    }
);

// Read shortcut information
var details = await Electron.Shell.ReadShortcutLinkAsync(@"C:\Users\Public\Desktop\MyApp.lnk");
Console.WriteLine($"Target: {details.Target}");
```

### Integration with Dialog API

```csharp
// Use with file dialog results
var files = await Electron.Dialog.ShowOpenDialogAsync(window, options);
if (files.Length > 0)
{
    var selectedFile = files[0];

    // Open the selected file
    await Electron.Shell.OpenPathAsync(selectedFile);

    // Show in file manager
    await Electron.Shell.ShowItemInFolderAsync(selectedFile);
}
```

## Related APIs

- [Electron.Dialog](Dialog.md) - Select files to open with Shell
- [Electron.App](App.md) - Application lifecycle events
- [Electron.Clipboard](Clipboard.md) - Copy file paths for Shell operations

## Additional Resources

- [Electron Shell Documentation](https://electronjs.org/docs/api/shell) - Official Electron shell API
