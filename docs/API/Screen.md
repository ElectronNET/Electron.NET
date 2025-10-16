# Electron.Screen

Access display and screen information for responsive layouts.

## Overview

The `Electron.Screen` API provides access to screen and display information, including screen size, display metrics, cursor position, and multi-monitor configurations. This is essential for creating responsive applications that adapt to different screen configurations.

## Methods

#### 🧊 `Task<Display[]> GetAllDisplaysAsync()`
Gets information about all available displays.

**Returns:**

An array of displays that are currently available.

#### 🧊 `Task<Point> GetCursorScreenPointAsync()`
Gets the current position of the mouse cursor on screen.

**Returns:**

The current absolute position of the mouse pointer.

#### 🧊 `Task<Display> GetDisplayMatchingAsync(Rectangle rectangle)`
Gets the display that most closely intersects the provided bounds.

**Parameters:**
- `rectangle` - The rectangle to find the matching display for

**Returns:**

The display that most closely intersects the provided bounds.

#### 🧊 `Task<Display> GetDisplayNearestPointAsync(Point point)`
Gets the display that is closest to the specified point.

**Parameters:**
- `point` - The point to find the nearest display for

**Returns:**

The display nearest the specified point.

#### 🧊 `Task<int> GetMenuBarHeightAsync()`
macOS: The height of the menu bar in pixels.

**Returns:**

The height of the menu bar in pixels.

#### 🧊 `Task<Display> GetPrimaryDisplayAsync()`
Gets information about the primary display (main screen).

**Returns:**

The primary display.

## Events

#### ⚡ `OnDisplayAdded`
Emitted when a new Display has been added.

#### ⚡ `OnDisplayMetricsChanged`
Emitted when one or more metrics change in a display. The changedMetrics is an array of strings that describe the changes. Possible changes are bounds, workArea, scaleFactor and rotation.

#### ⚡ `OnDisplayRemoved`
Emitted when oldDisplay has been removed.

## Usage Examples

### Display Information

```csharp
// Get primary display
var primaryDisplay = await Electron.Screen.GetPrimaryDisplayAsync();
Console.WriteLine($"Primary display: {primaryDisplay.Size.Width}x{primaryDisplay.Size.Height}");

// Get all displays
var displays = await Electron.Screen.GetAllDisplaysAsync();
Console.WriteLine($"Available displays: {displays.Length}");

// Get display near cursor
var cursorPoint = await Electron.Screen.GetCursorScreenPointAsync();
var nearestDisplay = await Electron.Screen.GetDisplayNearestPointAsync(cursorPoint);
Console.WriteLine($"Nearest display scale factor: {nearestDisplay.ScaleFactor}");
```

### Multi-Monitor Setup

```csharp
// Get all displays for multi-monitor setup
var displays = await Electron.Screen.GetAllDisplaysAsync();

foreach (var display in displays)
{
    Console.WriteLine($"Display {display.Id}:");
    Console.WriteLine($"  Size: {display.Size.Width}x{display.Size.Height}");
    Console.WriteLine($"  Position: {display.Bounds.X},{display.Bounds.Y}");
    Console.WriteLine($"  Scale Factor: {display.ScaleFactor}");
    Console.WriteLine($"  Work Area: {display.WorkArea.Width}x{display.WorkArea.Height}");
}
```

### Responsive Window Placement

```csharp
// Create window on appropriate display
var displays = await Electron.Screen.GetAllDisplaysAsync();
var targetDisplay = displays.FirstOrDefault(d => d.Bounds.X > 0) ?? displays.First();

var windowOptions = new BrowserWindowOptions
{
    Width = Math.Min(1200, targetDisplay.WorkArea.Width),
    Height = Math.Min(800, targetDisplay.WorkArea.Height),
    X = targetDisplay.WorkArea.X + (targetDisplay.WorkArea.Width - 1200) / 2,
    Y = targetDisplay.WorkArea.Y + (targetDisplay.WorkArea.Height - 800) / 2
};

var window = await Electron.WindowManager.CreateWindowAsync(windowOptions);
```

### Display Change Monitoring

```csharp
// Monitor display changes
Electron.Screen.OnDisplayAdded += (display) =>
{
    Console.WriteLine($"Display added: {display.Id}");
    UpdateWindowPositions();
};

Electron.Screen.OnDisplayRemoved += (display) =>
{
    Console.WriteLine($"Display removed: {display.Id}");
    UpdateWindowPositions();
};

Electron.Screen.OnDisplayMetricsChanged += (display, metrics) =>
{
    Console.WriteLine($"Display {display.Id} metrics changed: {string.Join(", ", metrics)}");
    UpdateWindowPositions();
};

void UpdateWindowPositions()
{
    // Recalculate window positions based on current displays
}
```

### macOS Menu Bar Height

```csharp
// Account for menu bar height on macOS
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    var menuBarHeight = await Electron.Screen.GetMenuBarHeightAsync();

    var windowOptions = new BrowserWindowOptions
    {
        Y = menuBarHeight, // Position below menu bar
        TitleBarStyle = TitleBarStyle.Hidden // Hide title bar for custom look
    };
}
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Position windows based on screen information
- [Electron.App](App.md) - Handle display-related application events

## Additional Resources

- [Electron Screen Documentation](https://electronjs.org/docs/api/screen) - Official Electron screen API
