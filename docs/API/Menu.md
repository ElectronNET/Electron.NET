# Electron.Menu

Create application menus, context menus, and menu items with full keyboard shortcut support.

## Overview

The `Electron.Menu` API provides comprehensive control over application menus and context menus. It supports native platform menus with custom menu items, submenus, keyboard shortcuts, and role-based menu items.

## Properties

#### 📋 `IReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>> ContextMenuItems`
Gets a read-only dictionary of all current context menu items, keyed by browser window ID.

#### 📋 `IReadOnlyCollection<MenuItem> MenuItems`
Gets a read-only collection of all current application menu items.

## Methods

#### 🧊 `void ContextMenuPopup(BrowserWindow browserWindow)`
Shows the context menu for the specified browser window.

**Parameters:**
- `browserWindow` - The browser window to show the context menu for

#### 🧊 `void SetApplicationMenu(MenuItem[] menuItems)`
Sets the application menu for the entire application.

**Parameters:**
- `menuItems` - Array of MenuItem objects defining the application menu

#### 🧊 `void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems)`
Sets a context menu for a specific browser window.

**Parameters:**
- `browserWindow` - The browser window to attach the context menu to
- `menuItems` - Array of MenuItem objects defining the context menu

## Usage Examples

### Application Menu

```csharp
// Create application menu
var menuItems = new[]
{
    new MenuItem
    {
        Label = "File",
        Submenu = new[]
        {
            new MenuItem { Label = "New", Click = () => CreateNewDocument() },
            new MenuItem { Label = "Open", Click = () => OpenDocument() },
            new MenuItem { Type = MenuType.Separator },
            new MenuItem { Label = "Exit", Click = () => Electron.App.Quit() }
        }
    },
    new MenuItem
    {
        Label = "Edit",
        Submenu = new[]
        {
            new MenuItem { Role = MenuRole.Undo },
            new MenuItem { Role = MenuRole.Redo },
            new MenuItem { Type = MenuType.Separator },
            new MenuItem { Role = MenuRole.Cut },
            new MenuItem { Role = MenuRole.Copy },
            new MenuItem { Role = MenuRole.Paste }
        }
    },
    new MenuItem
    {
        Label = "View",
        Submenu = new[]
        {
            new MenuItem { Role = MenuRole.Reload },
            new MenuItem { Role = MenuRole.ForceReload },
            new MenuItem { Role = MenuRole.ToggleDevTools },
            new MenuItem { Type = MenuType.Separator },
            new MenuItem { Role = MenuRole.ResetZoom },
            new MenuItem { Role = MenuRole.ZoomIn },
            new MenuItem { Role = MenuRole.ZoomOut }
        }
    },
    new MenuItem
    {
        Label = "Window",
        Submenu = new[]
        {
            new MenuItem { Role = MenuRole.Minimize },
            new MenuItem { Role = MenuRole.Close }
        }
    }
};

// Set application menu
Electron.Menu.SetApplicationMenu(menuItems);
```

### Context Menu

```csharp
// Create context menu for specific window
var contextMenuItems = new[]
{
    new MenuItem { Label = "Copy", Click = () => CopySelectedText() },
    new MenuItem { Label = "Paste", Click = () => PasteText() },
    new MenuItem { Type = MenuType.Separator },
    new MenuItem { Label = "Inspect Element", Click = () => InspectElement() }
};

// Set context menu for window
Electron.Menu.SetContextMenu(mainWindow, contextMenuItems);

// Show context menu programmatically
Electron.Menu.ContextMenuPopup(mainWindow);
```

### Menu with Keyboard Shortcuts

```csharp
// Create menu with keyboard shortcuts
var menuItems = new[]
{
    new MenuItem
    {
        Label = "File",
        Submenu = new[]
        {
            new MenuItem
            {
                Label = "New",
                Accelerator = "CmdOrCtrl+N",
                Click = () => CreateNewDocument()
            },
            new MenuItem
            {
                Label = "Open",
                Accelerator = "CmdOrCtrl+O",
                Click = () => OpenDocument()
            },
            new MenuItem
            {
                Label = "Save",
                Accelerator = "CmdOrCtrl+S",
                Click = () => SaveDocument()
            }
        }
    }
};

Electron.Menu.SetApplicationMenu(menuItems);
```

### Dynamic Menu Updates

```csharp
// Update menu items dynamically
var fileMenu = Electron.Menu.MenuItems.FirstOrDefault(m => m.Label == "File");
if (fileMenu?.Submenu != null)
{
    var saveItem = fileMenu.Submenu.FirstOrDefault(m => m.Label == "Save");
    if (saveItem != null)
    {
        saveItem.Enabled = HasUnsavedChanges;
    }
}
```

### Platform-Specific Menus

```csharp
// macOS specific menu items
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    var macMenuItems = new[]
    {
        new MenuItem
        {
            Label = "MyApp",
            Submenu = new[]
            {
                new MenuItem { Role = MenuRole.About },
                new MenuItem { Type = MenuType.Separator },
                new MenuItem { Role = MenuRole.Services },
                new MenuItem { Type = MenuType.Separator },
                new MenuItem { Role = MenuRole.Hide },
                new MenuItem { Role = MenuRole.HideOthers },
                new MenuItem { Role = MenuRole.Unhide },
                new MenuItem { Type = MenuType.Separator },
                new MenuItem { Role = MenuRole.Quit }
            }
        }
    };

    // Insert before File menu
    var allMenus = new List<MenuItem>(macMenuItems);
    allMenus.AddRange(menuItems);
    Electron.Menu.SetApplicationMenu(allMenus.ToArray());
}
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Windows for context menus
- [Electron.App](App.md) - Application lifecycle events
- [Electron.GlobalShortcut](GlobalShortcut.md) - Global keyboard shortcuts

## Additional Resources

- [Electron Menu Documentation](https://electronjs.org/docs/api/menu) - Official Electron menu API
