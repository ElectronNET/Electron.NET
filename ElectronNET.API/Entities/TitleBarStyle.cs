using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    public enum TitleBarStyle
    {
        [JsonProperty("default")]
        defaultStyle,
        hidden,
        hiddenInset,
        customButtonsOnHover
    }
}