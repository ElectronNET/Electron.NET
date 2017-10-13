using ElectronNET.API.Entities;

namespace ElectronNET.API
{
    public class JumpListCategory
    {
        public string Name { get; set; } = string.Empty;
        public JumpListItem[] Items { get; set; } = new JumpListItem[0];
        public string Type { get; set; } = "tasks";
    }
}
