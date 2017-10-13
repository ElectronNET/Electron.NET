namespace ElectronNET.API.Entities
{
    public class LoginItemSettingsOptions
    {
        /// <summary>
        /// The executable path to compare against. Defaults to process.execPath.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The command-line arguments to compare against. Defaults to an empty array.
        /// </summary>
        public string[] Args { get; set; }
    }
}