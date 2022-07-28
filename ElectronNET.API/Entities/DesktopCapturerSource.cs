using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    public sealed class DesktopCapturerSource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public NativeImage Thumbnail { get; set; }

        [JsonProperty("display_id")]
        public string DisplayId { get; set; }
        public NativeImage AppIcon { get; set; }
    }
}