using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    public enum Vibrancy
    {
        [JsonProperty("appearance-based")]
        appearanceBased,
        light,
        dark,
        titlebar,
        selection,
        menu,
        popover,
        sidebar,
        [JsonProperty("medium-light")]
        mediumLight,
        [JsonProperty("ultra-dark")]
        ultraDark
    }
}