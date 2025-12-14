using System;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class NotificationOptions
    {
        /// <summary>
        /// Gets or sets the title for the notification, which will be shown at the top of the notification window when it is shown.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle for the notification, which will be displayed below the title.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the body text of the notification, which will be displayed below the title or subtitle.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress the OS notification noise when showing the notification.
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Gets or sets an icon to use in the notification. Can be a string path or a NativeImage. If a string is passed, it must be a valid path to a local icon file.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add an inline reply option to the notification.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool HasReply { get; set; }

        /// <summary>
        /// Gets or sets the timeout duration of the notification. Can be 'default' or 'never'.
        /// </summary>
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("windows")]
        public string TimeoutType { get; set; }

        /// <summary>
        /// Gets or sets the placeholder to write in the inline reply input field.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string ReplyPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets the name of the sound file to play when the notification is shown.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Sound { get; set; }

        /// <summary>
        /// Gets or sets the urgency level of the notification. Can be 'normal', 'critical', or 'low'.
        /// </summary>
        [SupportedOSPlatform("linux")]
        public string Urgency { get; set; }

        /// <summary>
        /// Gets or sets the actions to add to the notification. Please read the available actions and limitations in the NotificationAction documentation.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public NotificationAction[] Actions { get; set; }

        /// <summary>
        /// Gets or sets a custom title for the close button of an alert. An empty string will cause the default localized text to be used.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string CloseButtonText { get; set; }

        /// <summary>
        /// Gets or sets a custom description of the Notification on Windows superseding all properties above. Provides full customization of design and behavior of the notification.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string ToastXml { get; set; }

        /// <summary>
        /// Emitted when the notification is shown to the user, note this could be fired
        /// multiple times as a notification can be shown multiple times through the Show()
        /// method.
        /// </summary>
        [JsonIgnore]
        public Action OnShow { get; set; }

        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        [JsonInclude]
        internal string ShowID { get; set; }

        /// <summary>
        /// Emitted when the notification is clicked by the user.
        /// </summary>
        [JsonIgnore]
        public Action OnClick { get; set; }

        /// <summary>
        /// Gets or sets the click identifier.
        /// </summary>
        /// <value>
        /// The click identifier.
        /// </value>
        [JsonInclude]
        internal string ClickID { get; set; }

        /// <summary>
        /// Emitted when the notification is closed by manual intervention from the user.
        ///
        /// This event is not guarunteed to be emitted in all cases where the notification is closed.
        /// </summary>
        [JsonIgnore]
        public Action OnClose { get; set; }

        /// <summary>
        /// Gets or sets the close identifier.
        /// </summary>
        /// <value>
        /// The close identifier.
        /// </value>
        [JsonInclude]
        internal string CloseID { get; set; }

        /// <summary>
        /// macOS only: Emitted when the user clicks the “Reply” button on a notification with hasReply: true.
        /// 
        /// The string the user entered into the inline reply field
        /// </summary>
        [JsonIgnore]
        [SupportedOSPlatform("macos")]
        public Action<string> OnReply { get; set; }

        /// <summary>
        /// Gets or sets the reply identifier.
        /// </summary>
        /// <value>
        /// The reply identifier.
        /// </value>
        [JsonInclude]
        internal string ReplyID { get; set; }

        /// <summary>
        /// macOS only - The index of the action that was activated.
        /// </summary>
        [JsonIgnore]
        [SupportedOSPlatform("macos")]
        public Action<int> OnAction { get; set; }

        /// <summary>
        /// Gets or sets the action identifier.
        /// </summary>
        /// <value>
        /// The action identifier.
        /// </value>
        [JsonInclude]
        internal string ActionID { get; set; }

        /// <summary>
        /// Windows only: Emitted when an error is encountered while creating and showing the native notification.
        /// Corresponds to the 'failed' event on Notification.
        /// </summary>
        [JsonIgnore]
        [SupportedOSPlatform("windows")]
        public Action<string> OnFailed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationOptions"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        public NotificationOptions(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }
}