namespace ElectronNET.API.Entities
{
    using System.Runtime.Versioning;

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
        /// The treat package as directory
        /// </summary>
        [SupportedOSPlatform("macos")]
        treatPackageAsDirectory,

        /// <summary>
        /// Do not add the item being opened to the recent documents list (Windows).
        /// </summary>
        [SupportedOSPlatform("windows")]
        dontAddToRecent
    }
}