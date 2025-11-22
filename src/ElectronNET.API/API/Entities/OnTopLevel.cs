using System.ComponentModel;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// String values for the 'level' parameter of BrowserWindow.setAlwaysOnTop.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("macOS")]
    [SupportedOSPlatform("Windows")]
    public enum OnTopLevel
    {
        /// <summary>
        /// The normal
        /// </summary>
        normal,

        /// <summary>
        /// The floating
        /// </summary>
        floating,

        /// <summary>
        /// The torn off menu
        /// </summary>
        [Description("torn-off-menu")]
        tornOffMenu,

        /// <summary>
        /// The modal panel
        /// </summary>
        [Description("modal-panel")]
        modalPanel,

        /// <summary>
        /// The main menu
        /// </summary>
        [Description("main-menu")]
        mainMenu,

        /// <summary>
        /// The status
        /// </summary>
        status,

        /// <summary>
        /// The pop up menu
        /// </summary>
        [Description("pop-up-menu")]
        popUpMenu,

        /// <summary>
        /// The screen saver
        /// </summary>
        [Description("screen-saver")]
        screenSaver
    }
}