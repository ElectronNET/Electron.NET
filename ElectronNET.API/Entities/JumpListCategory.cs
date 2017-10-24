using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public class JumpListCategory
    {
        /// <summary>
        /// Must be set if type is custom, otherwise it should be omitted.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Array of objects if type is tasks or custom, otherwise it should be omitted.
        /// </summary>
        public JumpListItem[] Items { get; set; }

        /// <summary>
        /// One of the following: "tasks" | "frequent" | "recent" | "custom"
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public JumpListCategoryType Type { get; set; }
    }
}
