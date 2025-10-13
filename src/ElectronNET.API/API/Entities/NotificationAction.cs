namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationAction
    {
        /// <summary>
        /// The label for the given action.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The type of action, can be button.
        /// </summary>
        public string Type { get; set; }
    }
}