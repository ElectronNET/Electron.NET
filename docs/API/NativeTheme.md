# Electron.NativeTheme

Detect and respond to changes in Chromium's native color theme.

## Overview

The `Electron.NativeTheme` API provides access to Chromium's native color theme information and allows you to detect and respond to changes in the system's dark/light mode settings. This enables your application to automatically adapt to the user's theme preferences.

## Methods

#### 🧊 `Task<ThemeSourceMode> GetThemeSourceAsync()`
Get the current theme source setting.

**Returns:**

A `ThemeSourceMode` property that can be `ThemeSourceMode.System`, `ThemeSourceMode.Light` or `ThemeSourceMode.Dark`.

#### 🧊 `void SetThemeSource(ThemeSourceMode themeSourceMode)`
Setting this property to `ThemeSourceMode.System` will remove the override and everything will be reset to the OS default. By default 'ThemeSource' is `ThemeSourceMode.System`.

**Parameters:**
- `themeSourceMode` - The new ThemeSource

#### 🧊 `Task<bool> ShouldUseDarkColorsAsync()`
Check if the system is currently using dark colors.

**Returns:**

A bool for if the OS / Chromium currently has a dark mode enabled or is being instructed to show a dark-style UI.

#### 🧊 `Task<bool> ShouldUseHighContrastColorsAsync()`
Check if the system is currently using high contrast colors.

**Returns:**

A bool for if the OS / Chromium currently has high-contrast mode enabled or is being instructed to show a high-contrast UI.

#### 🧊 `Task<bool> ShouldUseInvertedColorSchemeAsync()`
Check if the system is currently using an inverted color scheme.

**Returns:**

A bool for if the OS / Chromium currently has an inverted color scheme or is being instructed to use an inverted color scheme.

## Events

#### ⚡ `Updated`
Emitted when something in the underlying NativeTheme has changed. This normally means that either the value of ShouldUseDarkColorsAsync, ShouldUseHighContrastColorsAsync or ShouldUseInvertedColorSchemeAsync has changed.

## Usage Examples

### Basic Theme Detection

```csharp
// Check current theme
var isDarkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
Console.WriteLine($"Dark mode: {isDarkMode}");

// Get current theme source
var themeSource = await Electron.NativeTheme.GetThemeSourceAsync();
Console.WriteLine($"Theme source: {themeSource}");
```

### Theme Change Monitoring

```csharp
// Monitor theme changes
Electron.NativeTheme.Updated += () =>
{
    Console.WriteLine("Theme updated");
    UpdateApplicationTheme();
};

async void UpdateApplicationTheme()
{
    var isDarkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
    var isHighContrast = await Electron.NativeTheme.ShouldUseHighContrastColorsAsync();

    // Update application appearance
    ApplyTheme(isDarkMode, isHighContrast);
}
```

### Manual Theme Control

```csharp
// Force dark theme
Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);

// Force light theme
Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Light);

// Follow system theme
Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);
```

### Application Theme Integration

```csharp
public async Task InitializeThemeSupport()
{
    // Set initial theme based on system preference
    var isDarkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
    ApplyTheme(isDarkMode);

    // Monitor theme changes
    Electron.NativeTheme.Updated += async () =>
    {
        var darkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
        ApplyTheme(darkMode);
    };
}

private void ApplyTheme(bool isDarkMode)
{
    if (isDarkMode)
    {
        // Apply dark theme
        SetDarkThemeColors();
        UpdateWindowTheme("dark");
    }
    else
    {
        // Apply light theme
        SetLightThemeColors();
        UpdateWindowTheme("light");
    }
}
```

### Advanced Theme Management

```csharp
// Check all theme properties
var isDarkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
var isHighContrast = await Electron.NativeTheme.ShouldUseHighContrastColorsAsync();
var isInverted = await Electron.NativeTheme.ShouldUseInvertedColorSchemeAsync();

Console.WriteLine($"Dark mode: {isDarkMode}");
Console.WriteLine($"High contrast: {isHighContrast}");
Console.WriteLine($"Inverted: {isInverted}");

// Apply appropriate theme
if (isHighContrast)
{
    ApplyHighContrastTheme();
}
else if (isDarkMode)
{
    ApplyDarkTheme();
}
else
{
    ApplyLightTheme();
}
```

### Theme-Aware Window Creation

```csharp
// Create window with theme-appropriate settings
var isDarkMode = await Electron.NativeTheme.ShouldUseDarkColorsAsync();

var windowOptions = new BrowserWindowOptions
{
    Width = 1200,
    Height = 800,
    Title = "My Application",
    BackgroundColor = isDarkMode ? "#1a1a1a" : "#ffffff",
    WebPreferences = new WebPreferences
    {
        // Additional web preferences based on theme
    }
};

var window = await Electron.WindowManager.CreateWindowAsync(windowOptions);
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Apply theme to windows
- [Electron.Screen](Screen.md) - Screen-related theme considerations
- [Electron.App](App.md) - Application-level theme events

## Additional Resources

- [Electron NativeTheme Documentation](https://electronjs.org/docs/api/native-theme) - Official Electron native theme API
- [Theme Support](../Core/What's-New.md) - Understanding theme functionality
- [User Experience](../Using/Configuration.md) - Design theme-aware applications
