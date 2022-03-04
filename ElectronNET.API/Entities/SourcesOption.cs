namespace ElectronNET.API.Entities
{
    public sealed class SourcesOption
    {
        public string[] Types { get; set; }
        public Size ThumbnailSize { get; set; }
        public bool FetchWindowIcons { get; set; }
    }
}