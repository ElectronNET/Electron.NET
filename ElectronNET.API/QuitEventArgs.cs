namespace ElectronNET.API
{
    /// <summary>
    /// 
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
