using Newtonsoft.Json;
using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationOptions
    {
        /// <summary>
        /// A title for the notification, which will be shown at the top of the notification
        /// window when it is shown
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body text of the notification, which will be displayed below the title or
        /// subtitle
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// A subtitle for the notification, which will be displayed below the title.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Whether or not to emit an OS notification noise when showing the notification
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// An icon to use in the notification
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Whether or not to add an inline reply option to the notification.
        /// </summary>
        public bool HasReply { get; set; }

        /// <summary>
        /// The placeholder to write in the inline reply input field.
        /// </summary>
        public string ReplyPlaceholder { get; set; }

        /// <summary>
        /// The name of the sound file to play when the notification is shown.
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// Actions to add to the notification. Please read the available actions and
        /// limitations in the NotificationAction documentation
        /// </summary>
        public NotificationAction Actions { get; set; }

        /// <summary>
        /// Emitted when the notification is shown to the user, 
        /// note this could be fired multiple times as a notification 
        /// can be shown multiple times through the Show() method.
        /// </summary>
        [JsonIgnore]
        public Action OnShow { get; set; }

        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        [JsonProperty]
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
        [JsonProperty]
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
        [JsonProperty]
        internal string CloseID { get; set; }

        /// <summary>
        /// macOS only: Emitted when the user clicks the “Reply” button on a notification with hasReply: true.
        /// 
        /// The string the user entered into the inline reply field
        /// </summary>
        [JsonIgnore]
        public Action<string> OnReply { get; set; }

        /// <summary>
        /// Gets or sets the reply identifier.
        /// </summary>
        /// <value>
        /// The reply identifier.
        /// </value>
        [JsonProperty]
        internal string ReplyID { get; set; }

        /// <summary>
        /// macOS only - The index of the action that was activated
        /// </summary>
        [JsonIgnore]
        public Action<string> OnAction { get; set; }

        /// <summary>
        /// Gets or sets the action identifier.
        /// </summary>
        /// <value>
        /// The action identifier.
        /// </value>
        [JsonProperty]
        internal string ActionID { get; set; }

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
