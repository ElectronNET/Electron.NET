using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Windows launch entry as returned by app.getLoginItemSettings().launchItems.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("windows")]
    public class LoginItemLaunchItem
    {
        /// <summary>
        /// Name value of a registry entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The executable to an app that corresponds to a registry entry.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The command-line arguments to pass to the executable.
        /// </summary>
        public string[] Args { get; set; }

        /// <summary>
        /// One of <c>user</c> or <c>machine</c>. Indicates whether the registry entry is under HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// True if the app registry key is startup approved and therefore shows as enabled in Task Manager and Windows settings.
        /// </summary>
        public bool Enabled { get; set; }
    }
}