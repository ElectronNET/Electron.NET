# API Reference Overview

The ElectronNET.Core API provides comprehensive access to Electron's native desktop functionality through a .NET interface. This section documents all the available API classes and their methods, events, and usage patterns.

## API Classes

### Core Application Management
- **[Electron.App](App.md)** - Control your application's event lifecycle, manage app metadata, and handle system-level operations
- **[Electron.WindowManager](WindowManager.md)** - Create and manage browser windows, control window behavior and appearance
- **[Electron.Menu](Menu.md)** - Create application menus, context menus, and menu items with full keyboard shortcut support

### User Interface & Interaction
- **[Electron.Dialog](Dialog.md)** - Display native system dialogs for opening/saving files, showing messages and alerts
- **[Electron.Notification](Notification.md)** - Show native desktop notifications with custom content and actions
- **[Electron.Tray](Tray.md)** - Create system tray icons with context menus and tooltip support
- **[Electron.Dock](Dock.md)** - macOS dock integration for bounce effects and badge counts

### System Integration
- **[Electron.Shell](Shell.md)** - Desktop integration for opening files, URLs, and accessing system paths
- **[Electron.Clipboard](Clipboard.md)** - Read from and write to the system clipboard with multiple data formats
- **[Electron.Screen](Screen.md)** - Access display and screen information for responsive layouts
- **[Electron.NativeTheme](NativeTheme.md)** - Detect and respond to system theme changes (light/dark mode)

### Communication & Automation
- **[Electron.IpcMain](IpcMain.md)** - Inter-process communication between main process and renderer processes
- **[Electron.HostHook](HostHook.md)** - Custom host hook functionality for advanced integration scenarios
- **[Electron.GlobalShortcut](GlobalShortcut.md)** - Register global keyboard shortcuts that work even when app is not focused
- **[Electron.AutoUpdater](AutoUpdater.md)** - Handle application updates and installation processes

### System Monitoring
- **[Electron.PowerMonitor](PowerMonitor.md)** - Monitor system power events like sleep, wake, and battery status


## API Relationships

### Window and Dialog Integration
- Use `BrowserWindow` instances as parent windows for dialogs
- Dialogs automatically become modal when parent window is provided
- Window events coordinate with application lifecycle events

### IPC Communication
- `IpcMain` handles communication from renderer processes
- Use with `Electron.WindowManager` for window-specific messaging
- Coordinate with `Electron.App` events for application-wide communication

### System Integration
- `Shell` operations work with file paths from `Dialog` operations
- `Screen` information helps create properly sized windows
- `Notification` and `Tray` provide complementary user interaction methods

## 🚀 Next Steps

- **[Electron.App](App.md)** - Start with application lifecycle management
- **[Electron.WindowManager](WindowManager.md)** - Learn window creation and management
- **[Electron.Dialog](Dialog.md)** - Add file operations and user interactions
- **[Electron.Menu](Menu.md)** - Implement application menus and shortcuts

## 📚 Additional Resources

- **[Electron Documentation](https://electronjs.org/docs)** - Official Electron API reference
- **[Getting Started](../GettingStarted/ASP.Net.md)** - Development setup guides
- **[Migration Guide](../Core/Migration-Guide.md)** - Moving from previous versions
