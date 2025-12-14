using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Controls the behavior of <see cref="App.Focus(FocusOptions)"/>.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class FocusOptions
    {
        /// <summary>
        /// Make the receiver the active app even if another app is currently active.
        /// You should seek to use the steal option as sparingly as possible.
        /// </summary>
        [SupportedOSPlatform("macOS")]
        public bool Steal { get; set; }
    }
}