using ElectronNET.API.Entities;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
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
        /// Custom label for the confirmation button, when left empty the default label will
        /// be used.
        /// </summary>
        public string ButtonLabel { get; set; }

        /// <summary>
        /// The filters specifies an array of file types that can be displayed or 
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
        /// Message to display above text fields.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Custom label for the text displayed in front of the filename text field.
        /// </summary>
        public string NameFieldLabel { get; set; }

        /// <summary>
        /// Show the tags input box, defaults to true.
        /// </summary>
        public bool ShowsTagField { get; set; }
    }
}