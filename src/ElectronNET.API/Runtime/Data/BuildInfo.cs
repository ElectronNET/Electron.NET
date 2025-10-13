namespace ElectronNET.Runtime.Data
{
    public class BuildInfo
    {
        public string ElectronExecutable { get; internal set; }

        public string ElectronVersion { get; internal set; }

        public string RuntimeIdentifier { get; internal set; }

        public string ElectronSingleInstance { get; internal set; }

        public string Title { get; internal set; }

        public string Version { get; internal set; }

        public string BuildConfiguration { get; internal set; }
    }
}