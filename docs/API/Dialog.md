# Electron.Dialog

Display native system dialogs for opening and saving files, alerting, etc.

## Overview

The `Electron.Dialog` API provides access to native system dialogs for file operations, message boxes, and certificate trust dialogs. These dialogs are modal and provide a consistent user experience across different platforms.

## Methods

#### 🧊 `Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, MessageBoxOptions messageBoxOptions)`
Shows a message box, it will block the process until the message box is closed. It returns the index of the clicked button. If a callback is passed, the dialog will not block the process.

**Parameters:**
- `browserWindow` - The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.
- `messageBoxOptions` - Message content and configuration

**Returns:**

The API call will be asynchronous and the result will be passed via MessageBoxResult.

#### 🧊 `Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, string message)`
Shows a message box, it will block the process until the message box is closed. It returns the index of the clicked button. If a callback is passed, the dialog will not block the process.

**Parameters:**
- `browserWindow` - The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.
- `message` - Message content

**Returns:**

The API call will be asynchronous and the result will be passed via MessageBoxResult.

#### 🧊 `Task<MessageBoxResult> ShowMessageBoxAsync(MessageBoxOptions messageBoxOptions)`
Shows a message box, it will block the process until the message box is closed. It returns the index of the clicked button. If a callback is passed, the dialog will not block the process.

**Parameters:**
- `messageBoxOptions` - Message content and configuration

**Returns:**

The API call will be asynchronous and the result will be passed via MessageBoxResult.

#### 🧊 `Task<MessageBoxResult> ShowMessageBoxAsync(string message)`
Shows a message box, it will block the process until the message box is closed. It returns the index of the clicked button. If a callback is passed, the dialog will not block the process.

**Parameters:**
- `message` - Message content

**Returns:**

The API call will be asynchronous and the result will be passed via MessageBoxResult.

#### 🧊 `Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions options)`
Note: On Windows and Linux an open dialog can not be both a file selector and a directory selector, so if you set properties to ['openFile', 'openDirectory'] on these platforms, a directory selector will be shown.

**Parameters:**
- `browserWindow` - The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.
- `options` - Dialog configuration options

**Returns:**

An array of file paths chosen by the user

#### 🧊 `Task<string> ShowSaveDialogAsync(BrowserWindow browserWindow, SaveDialogOptions options)`
Dialog for save files.

**Parameters:**
- `browserWindow` - The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.
- `options` - Dialog configuration options

**Returns:**

Returns String, the path of the file chosen by the user, if a callback is provided it returns an empty string.

#### 🧊 `void ShowErrorBox(string title, string content)`
Displays a modal dialog that shows an error message.

This API can be called safely before the ready event the app module emits, it is usually used to report errors in early stage of startup.If called before the app readyevent on Linux, the message will be emitted to stderr, and no GUI dialog will appear.

**Parameters:**
- `title` - The title to display in the error box.
- `content` - The text content to display in the error box.

#### 🧊 `Task ShowCertificateTrustDialogAsync(BrowserWindow browserWindow, CertificateTrustDialogOptions options)`
On macOS, this displays a modal dialog that shows a message and certificate information, and gives the user the option of trusting/importing the certificate. If you provide a browserWindow argument the dialog will be attached to the parent window, making it modal.

**Parameters:**
- `browserWindow` - Parent window for modal behavior
- `options` - Certificate trust dialog options

#### 🧊 `Task ShowCertificateTrustDialogAsync(CertificateTrustDialogOptions options)`
On macOS, this displays a modal dialog that shows a message and certificate information, and gives the user the option of trusting/importing the certificate. If you provide a browserWindow argument the dialog will be attached to the parent window, making it modal.

**Parameters:**
- `options` - Certificate trust dialog options

## Usage Examples

### File Operations

```csharp
// Open multiple files
var files = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
{
    Properties = new[] { OpenDialogProperty.OpenFile, OpenDialogProperty.MultiSelections }
});

// Save with custom extension
var path = await Electron.Dialog.ShowSaveDialogAsync(window, new SaveDialogOptions
{
    DefaultPath = "backup.json",
    Filters = new[] { new FileFilter { Name = "JSON", Extensions = new[] { "json" } } }
});
```

### User Confirmation

```csharp
// Confirmation dialog
var result = await Electron.Dialog.ShowMessageBoxAsync(window, new MessageBoxOptions
{
    Type = MessageBoxType.Question,
    Title = "Confirm Delete",
    Message = $"Delete {filename}?",
    Buttons = new[] { "Cancel", "Delete" },
    DefaultId = 0,
    CancelId = 0
});

if (result.Response == 1)
{
    // Delete file
}
```

### Error Handling

```csharp
// Error dialog
Electron.Dialog.ShowErrorBox("Save Failed", "Could not save file. Please check permissions and try again.");

// Warning dialog
await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions
{
    Type = MessageBoxType.Warning,
    Title = "Warning",
    Message = "This operation may take several minutes.",
    Buttons = new[] { "Continue", "Cancel" }
});
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Parent windows for modal dialogs
- [Electron.App](App.md) - Application lifecycle events
- [Electron.Shell](Shell.md) - File operations with selected paths

## Additional Resources

- [Electron Dialog Documentation](https://electronjs.org/docs/api/dialog) - Official Electron dialog API
