using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Properties supported by dialog.showSaveDialog / showSaveDialogSync.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public enum SaveDialogProperty
    {
        /// <summary>
        /// Show hidden files in dialog.
        /// </summary>
        showHiddenFiles,

        /// <summary>
        /// Allow creating new directories from dialog (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        createDirectory,

        /// <summary>
        /// Treat packages, such as .app folders, as a directory instead of a file (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        treatPackageAsDirectory,

        /// <summary>
        /// Sets whether the user will be presented a confirmation dialog if the user types a file name that already exists (Linux).
        /// </summary>
        [SupportedOSPlatform("linux")]
        showOverwriteConfirmation,

        /// <summary>
        /// Do not add the item being saved to the recent documents list (Windows).
        /// </summary>
        [SupportedOSPlatform("windows")]
        dontAddToRecent
    }
}