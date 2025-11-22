using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Menu item types matching Electron's MenuItem.type values.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public enum MenuType
    {
        /// <summary>
        /// Normal menu item.
        /// </summary>
        normal,

        /// <summary>
        /// Separator between items.
        /// </summary>
        separator,

        /// <summary>
        /// Submenu container.
        /// </summary>
        submenu,

        /// <summary>
        /// Checkbox item.
        /// </summary>
        checkbox,

        /// <summary>
        /// Radio item.
        /// </summary>
        radio,

        /// <summary>
        /// Header item (macOS 14+).
        /// </summary>
        [SupportedOSPlatform("macos")]
        header,

        /// <summary>
        /// Palette item (macOS 14+).
        /// </summary>
        [SupportedOSPlatform("macos")]
        palette
    }
}