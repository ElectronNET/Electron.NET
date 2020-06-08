namespace ElectronNET.API
{
    /// <summary>
    /// Event arguments for the <see cref="App.BeforeQuit"/> / <see cref="App.WillQuit"/> event.
    /// </summary>
    public sealed class QuitEventArgs
    {
        /// <summary>
        /// Will prevent the default behaviour, which is terminating the application.
        /// </summary>
        public void PreventDefault()
        {
            Electron.App.PreventQuit();
        }
    }
}