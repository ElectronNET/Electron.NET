namespace ElectronNET.Runtime
{
    /// <summary>
    /// Provides shared default values for the Electron.NET runtime host.
    /// </summary>
    public static class ElectronHostDefaults
    {
        public const int DefaultSocketPort = 8000;
        public const int DefaultWebPort = 8001;
        public const string ElectronPortArgumentName = "electronPort";
        public const string ElectronPidArgumentName = "electronPID";
    }
}
