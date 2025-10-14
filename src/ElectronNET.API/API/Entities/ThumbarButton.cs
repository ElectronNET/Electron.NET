using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ThumbarButton
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets or sets the click.
        /// </summary>
        /// <value>
        /// The click.
        /// </value>
        [JsonIgnore]
        public Action Click { get; set; }

        /// <summary>
        /// Control specific states and behaviors of the button. By default, it is ["enabled"].
        /// 
        /// enabled - The button is active and available to the user.
        /// disabled - The button is disabled.It is present, but has a visual state indicating it will not respond to user action.
        /// dismissonclick - When the button is clicked, the thumbnail window closes immediately.
        /// nobackground - Do not draw a button border, use only the image.
        /// hidden - The button is not shown to the user.
        /// noninteractive - The button is enabled but not interactive; no pressed button state is drawn.This value is intended for instances where the button is used in a notification.
        /// </summary>
        [JsonProperty("flags", ItemConverterType = typeof(StringEnumConverter))]
        public ThumbarButtonFlag[] Flags { get; set; }

        /// <summary>
        /// The icon showing in thumbnail toolbar.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The text of the button's tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThumbarButton"/> class.
        /// </summary>
        /// <param name="icon">The icon.</param>
        public ThumbarButton(string icon)
        {
            Icon = icon;
        }
    }
}