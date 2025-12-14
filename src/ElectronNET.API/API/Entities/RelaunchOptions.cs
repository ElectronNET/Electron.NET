namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for app.relaunch: optional args array and execPath.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class RelaunchOptions
    {
        /// <summary>
        /// Command-line arguments for the relaunched instance.
        /// </summary>
        public string[] Args { get; set; }

        /// <summary>
        /// Executable path to relaunch instead of the current app.
        /// </summary>
        public string ExecPath { get; set; }
    }
}