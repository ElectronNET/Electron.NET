namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum ThumbarButtonFlag
    {
        /// <summary>
        /// The button is active and available to the user.
        /// </summary>
        enabled,

        /// <summary>
        /// The button is disabled.It is present, but has a visual state indicating it will not respond to user action.
        /// </summary>
        disabled,

        /// <summary>
        /// When the button is clicked, the thumbnail window closes immediately.
        /// </summary>
        dismissonclick,

        /// <summary>
        /// Do not draw a button border, use only the image.
        /// </summary>
        nobackground,

        /// <summary>
        /// The button is not shown to the user.
        /// </summary>
        hidden,

        /// <summary>
        /// The button is enabled but not interactive; no pressed button state is drawn.This value is intended for instances where the button is used in a notification.
        /// </summary>
        noninteractive
    }
}