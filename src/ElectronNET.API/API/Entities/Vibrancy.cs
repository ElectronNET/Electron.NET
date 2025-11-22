using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Vibrancy types for BrowserWindow on macOS.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("macos")]
    public enum Vibrancy
    {
        /// <summary>
        /// Appearance-based vibrancy.
        /// </summary>
        [EnumMember(Value = "appearance-based")]
        appearanceBased,

        /// <summary>
        /// Title bar area.
        /// </summary>
        titlebar,

        /// <summary>
        /// Selection highlight.
        /// </summary>
        selection,

        /// <summary>
        /// Menu background.
        /// </summary>
        menu,

        /// <summary>
        /// Popover background.
        /// </summary>
        popover,

        /// <summary>
        /// Sidebar background.
        /// </summary>
        sidebar,

        /// <summary>
        /// Header background.
        /// </summary>
        header,

        /// <summary>
        /// Sheet background.
        /// </summary>
        sheet,

        /// <summary>
        /// Window background.
        /// </summary>
        window,

        /// <summary>
        /// Heads-up display.
        /// </summary>
        hud,

        /// <summary>
        /// Fullscreen UI background.
        /// </summary>
        [EnumMember(Value = "fullscreen-ui")]
        fullscreenUi,

        /// <summary>
        /// Tooltip background.
        /// </summary>
        tooltip,

        /// <summary>
        /// Content background.
        /// </summary>
        content,

        /// <summary>
        /// Under-window background.
        /// </summary>
        [EnumMember(Value = "under-window")]
        underWindow,

        /// <summary>
        /// Under-page background.
        /// </summary>
        [EnumMember(Value = "under-page")]
        underPage
    }
}