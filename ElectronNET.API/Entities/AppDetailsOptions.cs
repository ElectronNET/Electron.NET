namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDetailsOptions
    {
        /// <summary>
        /// Window’s App User Model ID. It has to be set, otherwise the other options will have no effect.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Window’s Relaunch Icon.
        /// </summary>
        public string AppIconPath { get; set; }

        /// <summary>
        /// Index of the icon in appIconPath. Ignored when appIconPath is not set. Default is 0.
        /// </summary>
        public int AppIconIndex { get; set; }

        /// <summary>
        /// Window’s Relaunch Command.
        /// </summary>
        public string RelaunchCommand { get; set; }

        /// <summary>
        /// Window’s Relaunch Display Name.
        /// </summary>
        public string RelaunchDisplayName { get; set; }
    }
}