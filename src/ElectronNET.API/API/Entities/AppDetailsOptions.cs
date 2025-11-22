using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("windows")]
    public class AppDetailsOptions
    {
        /// <summary>
        /// Window's App User Model ID. It has to be set, otherwise the other options will have no effect.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Window's relaunch icon resource path.
        /// </summary>
        public string AppIconPath { get; set; }

        /// <summary>
        /// Index of the icon in <see cref="AppIconPath"/>. Ignored when <see cref="AppIconPath"/> is not set. Default is 0.
        /// </summary>
        public int AppIconIndex { get; set; }

        /// <summary>
        /// Window's relaunch command.
        /// </summary>
        public string RelaunchCommand { get; set; }

        /// <summary>
        /// Window's relaunch display name.
        /// </summary>
        public string RelaunchDisplayName { get; set; }
    }
}