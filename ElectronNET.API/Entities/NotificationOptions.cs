namespace ElectronNET.API.Entities
{
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

        public NotificationOptions(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }
}
