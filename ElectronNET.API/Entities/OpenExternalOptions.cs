using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenExternalOptions
    {
        /// <summary>
        /// true to bring the opened application to the foreground. The default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Activate { get; set; } = true;
    }
}