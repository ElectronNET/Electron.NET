using System.Runtime.Versioning;

namespace ElectronNET.API.Entities;

/// <summary>
/// Configures the window's title bar overlay when using a frameless window.
/// </summary>
/// <remarks>Up-to-date with Electron API 39.2</remarks>
public class TitleBarOverlay
{
    private readonly bool? _value;

    public TitleBarOverlay()
    {
    }

    private TitleBarOverlay(bool value) : this() => _value = value;

    /// <summary>
    /// Gets or sets the CSS color of the Window Controls Overlay when enabled.
    /// OS-specific per MCP: available on Windows and Linux.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets the height of the title bar and Window Controls Overlay in pixels. Default is system height.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the CSS color of the symbols on the Window Controls Overlay when enabled.
    /// OS-specific per MCP: available on Windows and Linux.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public string SymbolColor { get; set; }

    /// <summary>
    /// Allows using a bare boolean for titleBarOverlay in options (true/false).
    /// </summary>
    public static implicit operator bool?(TitleBarOverlay titleBarOverlay) => titleBarOverlay?._value;

    /// <summary>
    /// Allows constructing from a bare boolean (true/false) for titleBarOverlay.
    /// </summary>
    public static implicit operator TitleBarOverlay(bool value) => new(value);
}