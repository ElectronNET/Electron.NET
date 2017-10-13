namespace ElectronNET.API.Entities
{
    public class JumpListItem
    {
        public string Args { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IconIndex { get; set; } = 0;
        public string IconPath { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type {get; set; } = string.Empty;
    }
}
