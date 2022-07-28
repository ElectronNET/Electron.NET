using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum OpenDialogProperty
    {
        /// <summary>
        /// The open file
        /// </summary>
        openFile,

        /// <summary>
        /// The open directory
        /// </summary>
        openDirectory,

        /// <summary>
        /// The multi selections
        /// </summary>
        multiSelections,

        /// <summary>
        /// The show hidden files
        /// </summary>
        showHiddenFiles,

        /// <summary>
        /// The create directory
        /// </summary>
        [SupportedOSPlatform("macos")]
        createDirectory,

        /// <summary>
        /// The prompt to create
        /// </summary>
        [SupportedOSPlatform("windows")]
        promptToCreate,

        /// <summary>
        /// The no resolve aliases
        /// </summary>
        [SupportedOSPlatform("macos")]
        noResolveAliases,

        /// <summary>
        /// Treat packages, such as .app folders, as a directory instead of a file.
        /// </summary>
        [SupportedOSPlatform("macos")]
        treatPackageAsDirectory,

        /// <summary>
        /// Don't add the item being opened to recent documents list
        /// </summary>
        [SupportedOSPlatform("windows")]
        dontAddToRecent
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SaveDialogProperty
    {
        /// <summary>
        /// The show hidden files
        /// </summary>
        showHiddenFiles,

        /// <summary>
        /// The create directory
        /// </summary>
        [SupportedOSPlatform("macos")]
        createDirectory,

        /// <summary>
        /// Treat packages, such as .app folders, as a directory instead of a file.
        /// </summary>
        [SupportedOSPlatform("macos")]
        treatPackageAsDirectory,

        /// <summary>
        /// Sets whether the user will be presented a confirmation dialog if the user types a file name that already exists.
        /// </summary>
        [SupportedOSPlatform("linux")]
        showOverwriteConfirmation,

        /// <summary>
        /// Don't add the item being opened to recent documents list
        /// </summary>
        [SupportedOSPlatform("windows")]
        dontAddToRecent
    }
}