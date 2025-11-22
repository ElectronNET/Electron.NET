using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for dialog.showMessageBox / dialog.showMessageBoxSync.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class MessageBoxOptions
    {
        /// <summary>
        /// Gets or sets the type. Can be "none", "info", "error", "question" or "warning". On Windows, "question" displays the same icon as "info", unless you set an icon using the "icon" option. On macOS, both "warning" and "error" display the same warning icon.
        /// </summary>
        public MessageBoxType Type { get; set; }

        /// <summary>
        /// Gets or sets the array of texts for buttons. On Windows, an empty array will result in one button labeled "OK".
        /// </summary>
        public string[] Buttons { get; set; }

        /// <summary>
        /// Gets or sets the index of the button in the buttons array which will be selected by default when the message box opens.
        /// </summary>
        public int DefaultId { get; set; }

        /// <summary>
        /// Gets or sets the title of the message box; some platforms will not show it.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the message box.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the extra information of the message.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets the checkbox label. If provided, the message box will include a checkbox with the given label.
        /// </summary>
        public string CheckboxLabel { get; set; }

        /// <summary>
        /// Gets or sets the initial checked state of the checkbox. Defaults to false.
        /// </summary>
        public bool CheckboxChecked { get; set; }

        /// <summary>
        /// Gets or sets the icon for the message box.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the custom width of the text in the message box.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public int? TextWidth { get; set; }

        /// <summary>
        /// Gets or sets the index of the button to be used to cancel the dialog via the Esc key. By default this is assigned to the first button with "cancel" or "no" as the label. If no such labeled buttons exist and this option is not set, 0 will be used.
        /// </summary>
        public int CancelId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable Windows command-links behavior (noLink).
        /// On Windows Electron will try to figure out which one of the buttons are common buttons (like "Cancel" or "Yes"), and show the others as command links in the dialog. Set to true to disable this behavior.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public bool NoLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to normalize the keyboard access keys across platforms. Default is false. Enabling this assumes '&amp;' is used in the button labels for the placement of the keyboard
        /// shortcut access key and labels will be converted so they work correctly on each
        /// platform, '&amp;' characters are removed on macOS, converted to '_' on Linux, and left
        /// untouched on Windows. For example, a button label of "View" will be converted to "Vie_w" on Linux and "View" on macOS and can be selected via Alt-W on Windows and
        /// Linux.
        /// </summary>
        public bool NormalizeAccessKeys { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxOptions"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageBoxOptions(string message)
        {
            Message = message;
        }
    }
}