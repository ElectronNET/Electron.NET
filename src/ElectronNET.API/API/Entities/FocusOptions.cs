namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Controls the behavior of <see cref="App.Focus(FocusOptions)"/>.
    /// </summary>
    public class FocusOptions
    {
        /// <summary>
        /// Make the receiver the active app even if another app is currently active.
        /// <para/>
        /// You should seek to use the <see cref="Steal"/> option as sparingly as possible.
        /// </summary>
        public bool Steal { get; set; }
    }
}