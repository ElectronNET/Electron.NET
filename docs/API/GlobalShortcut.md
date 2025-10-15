# Electron.GlobalShortcut

Register global keyboard shortcuts that work even when the application is not focused.

## Overview

The `Electron.GlobalShortcut` API provides the ability to register global keyboard shortcuts that can be triggered even when the application does not have keyboard focus. This is useful for creating system-wide hotkeys and shortcuts.

## Methods

#### 🧊 `Task<bool> IsRegisteredAsync(string accelerator)`
Check if the accelerator is registered.

**Parameters:**
- `accelerator` - Keyboard shortcut to check

**Returns:**

Whether this application has registered the accelerator.

#### 🧊 `void Register(string accelerator, Action function)`
Registers a global shortcut of accelerator. The callback is called when the registered shortcut is pressed by the user.

**Parameters:**
- `accelerator` - Keyboard shortcut combination
- `function` - Callback function to execute when shortcut is pressed

#### 🧊 `void Unregister(string accelerator)`
Unregisters the global shortcut of accelerator.

**Parameters:**
- `accelerator` - Keyboard shortcut to unregister

#### 🧊 `void UnregisterAll()`
Unregisters all of the global shortcuts.

## Usage Examples

### Basic Global Shortcuts

```csharp
// Register global shortcuts
Electron.GlobalShortcut.Register("CommandOrControl+N", () =>
{
    CreateNewDocument();
});

Electron.GlobalShortcut.Register("CommandOrControl+O", () =>
{
    OpenDocument();
});

Electron.GlobalShortcut.Register("CommandOrControl+S", () =>
{
    SaveDocument();
});
```

### Media Control Shortcuts

```csharp
// Media playback shortcuts
Electron.GlobalShortcut.Register("MediaPlayPause", () =>
{
    TogglePlayback();
});

Electron.GlobalShortcut.Register("MediaNextTrack", () =>
{
    NextTrack();
});

Electron.GlobalShortcut.Register("MediaPreviousTrack", () =>
{
    PreviousTrack();
});
```

### Application Control Shortcuts

```csharp
// Application control shortcuts
Electron.GlobalShortcut.Register("CommandOrControl+Shift+Q", async () =>
{
    var result = await Electron.Dialog.ShowMessageBoxAsync("Quit Application?", "Are you sure you want to quit?");
    if (result.Response == 1) // Yes
    {
        Electron.App.Quit();
    }
});

Electron.GlobalShortcut.Register("CommandOrControl+Shift+H", () =>
{
    ToggleMainWindow();
});
```

### Dynamic Shortcut Management

```csharp
// Register shortcuts based on user preferences
public void RegisterUserShortcuts(Dictionary<string, Action> shortcuts)
{
    foreach (var shortcut in shortcuts)
    {
        Electron.GlobalShortcut.Register(shortcut.Key, shortcut.Value);
    }
}

// Check if shortcut is available
public async Task<bool> IsShortcutAvailable(string accelerator)
{
    return await Electron.GlobalShortcut.IsRegisteredAsync(accelerator);
}

// Unregister specific shortcut
public void UnregisterShortcut(string accelerator)
{
    Electron.GlobalShortcut.Unregister(accelerator);
}
```

### Platform-Specific Shortcuts

```csharp
// macOS specific shortcuts
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    Electron.GlobalShortcut.Register("Command+Comma", () =>
    {
        OpenPreferences();
    });

    Electron.GlobalShortcut.Register("Command+H", () =>
    {
        Electron.App.Hide();
    });
}

// Windows/Linux shortcuts
else
{
    Electron.GlobalShortcut.Register("Ctrl+Shift+P", () =>
    {
        OpenPreferences();
    });

    Electron.GlobalShortcut.Register("Alt+F4", () =>
    {
        Electron.App.Quit();
    });
}
```

### Shortcut Validation

```csharp
// Validate shortcuts before registration
public async Task<bool> TryRegisterShortcut(string accelerator, Action callback)
{
    if (await Electron.GlobalShortcut.IsRegisteredAsync(accelerator))
    {
        Console.WriteLine($"Shortcut {accelerator} is already registered");
        return false;
    }

    try
    {
        Electron.GlobalShortcut.Register(accelerator, callback);
        Console.WriteLine($"Successfully registered shortcut: {accelerator}");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to register shortcut {accelerator}: {ex.Message}");
        return false;
    }
}
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle events
- [Electron.Menu](Menu.md) - Menu-based shortcuts
- [Electron.WindowManager](WindowManager.md) - Window focus management

## Additional Resources

- [Electron GlobalShortcut Documentation](https://electronjs.org/docs/api/global-shortcut) - Official Electron global shortcut API
