using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    using System.Runtime.Versioning;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class OpenDialogOptions
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the default path.
        /// </summary>
        /// <value>
        /// The default path.
        /// </value>
        public string DefaultPath { get; set; }

        /// <summary>
        /// Custom label for the confirmation button, when left empty the default label will be used.
        /// </summary>
        public string ButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets which features the dialog should use. The following values are supported:
        /// 'openFile' | 'openDirectory' | 'multiSelections' | 'showHiddenFiles' | 'createDirectory' | 'promptToCreate' | 'noResolveAliases' | 'treatPackageAsDirectory'
        /// </summary>
        public OpenDialogProperty[] Properties { get; set; }

        /// <summary>
        /// Gets or sets the message to display above input boxes.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the filters specifying an array of file types that can be displayed or
        /// selected when you want to limit the user to a specific type. For example:
        /// </summary>
        /// <example>
        /// <code>
        /// new FileFilter[]
        /// {
        ///  new FileFiler { Name = "Images", Extensions = new string[] { "jpg", "png", "gif" } },
        ///  new FileFiler { Name = "Movies", Extensions = new string[] { "mkv", "avi", "mp4" } },
        ///  new FileFiler { Name = "Custom File Type", Extensions= new string[] {"as" } },
        ///  new FileFiler { Name = "All Files", Extensions= new string[] { "*" } }
        /// }
        /// </code>
        /// </example>
        public FileFilter[] Filters { get; set; }

        /// <summary>
        /// Create security scoped bookmarks when packaged for the Mac App Store.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool SecurityScopedBookmarks { get; set; }
    }
}