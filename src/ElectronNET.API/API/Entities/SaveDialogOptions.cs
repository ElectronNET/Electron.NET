using ElectronNET.API.Entities;
using System.Runtime.Versioning;

namespace ElectronNET.API
{
    /// <summary>
    /// Options for dialog.showSaveDialog / dialog.showSaveDialogSync.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class SaveDialogOptions
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Absolute directory path, absolute file path, or file name to use by default.
        /// </summary>
        public string DefaultPath { get; set; }

        /// <summary>
        /// Gets or sets the custom label for the confirmation button; when left empty the default label will be used.
        /// </summary>
        public string ButtonLabel { get; set; }

        /// <summary>
        /// Properties for the save dialog. Supported values:
        /// showHiddenFiles | createDirectory (macOS) | treatPackageAsDirectory (macOS) | showOverwriteConfirmation (Linux) | dontAddToRecent (Windows)
        /// </summary>
        public SaveDialogProperty[] Properties { get; set; }

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
        /// Gets or sets the message to display above text fields.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the custom label for the text displayed in front of the filename text field.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string NameFieldLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the tags input box. Defaults to true.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool ShowsTagField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a security scoped bookmark when packaged for the Mac App Store. If enabled and the file doesn't already exist a blank file will be created at the chosen path.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool SecurityScopedBookmarks { get; set; }
    }
}