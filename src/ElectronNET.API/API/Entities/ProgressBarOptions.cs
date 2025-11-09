
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ProgressBarOptions
    {
        /// <summary>
        /// Mode for the progress bar. Can be 'none' | 'normal' | 'indeterminate' | 'error' | 'paused'.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProgressBarMode Mode { get; set; }
    }
}

