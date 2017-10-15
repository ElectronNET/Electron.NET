using ElectronNET.API.Entities;

namespace ElectronNET.API
{
    public class JumpListCategory
    {
        /// <summary>
        /// Must be set if type is custom, otherwise it should be omitted.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Array of objects if type is tasks or custom, otherwise it should be omitted.
        /// </summary>
        public JumpListItem[] Items { get; set; } = new JumpListItem[0];

        /// <summary>
        /// One of the following: "tasks" | "frequent" | "recent" | "custom"
        /// </summary>
        public string Type { get; set; } = "tasks";
    }
}
