using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBoxOptions
    {
        /// <summary>
        ///  Can be "none", "info", "error", "question" or "warning". On Windows, "question"
        ///  displays the same icon as "info", unless you set an icon using the "icon"
        ///  option. On macOS, both "warning" and "error" display the same warning icon.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageBoxType Type { get; set; }

        /// <summary>
        /// Array of texts for buttons. On Windows, an empty array will result in one button
        /// labeled "OK".
        /// </summary>
        public string[] Buttons { get; set; }

        /// <summary>
        /// Index of the button in the buttons array which will be selected by default when
        /// the message box opens.
        /// </summary>
        public int DefaultId { get; set; }

        /// <summary>
        /// Title of the message box, some platforms will not show it.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content of the message box.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Extra information of the message.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// If provided, the message box will include a checkbox with the given label. The
        /// checkbox state can be inspected only when using callback.
        /// </summary>
        public string CheckboxLabel { get; set; }

        /// <summary>
        /// Initial checked state of the checkbox. false by default.
        /// </summary>
        public bool CheckboxChecked { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public string Icon { get; set; }

        /// <summary>
        /// The index of the button to be used to cancel the dialog, via the Esc key. By
        /// default this is assigned to the first button with "cancel" or "no" as the label.
        /// If no such labeled buttons exist and this option is not set, 0 will be used as
        /// the return value or callback response. This option is ignored on Windows.
        /// </summary>
        public int CancelId { get; set; }

        /// <summary>
        /// On Windows Electron will try to figure out which one of the buttons are common
        /// buttons(like "Cancel" or "Yes"), and show the others as command links in the
        /// dialog.This can make the dialog appear in the style of modern Windows apps. If
        /// you don't like this behavior, you can set noLink to true.
        /// </summary>
        public bool NoLink { get; set; }

        /// <summary>
        /// Normalize the keyboard access keys across platforms. Default is false. Enabling
        /// this assumes AND character is used in the button labels for the placement of the keyboard
        /// shortcut access key and labels will be converted so they work correctly on each
        /// platform, AND characters are removed on macOS, converted to _ on Linux, and left
        /// untouched on Windows.For example, a button label of VieANDw will be converted to
        /// Vie_w on Linux and View on macOS and can be selected via Alt-W on Windows and
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
