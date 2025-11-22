namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class BrowserViewConstructorOptions
    {
        /// <summary>
        /// Gets or sets the web preferences for the view (see WebPreferences).
        /// </summary>
        public WebPreferences WebPreferences { get; set; }

        /// <summary>
        /// Gets or sets a proxy to use on creation in the format host:port.
        /// The proxy can be alternatively set using the BrowserView.WebContents.SetProxyAsync function.
        /// </summary>
        /// <remarks>This is custom shortcut. Not part of the Electron API.</remarks>
        public string Proxy { get; set; }

        /// <summary>
        /// Gets or sets the credentials of the proxy in the format username:password.
        /// These will only be used if the Proxy field is also set.
        /// </summary>
        /// <remarks>This is custom shortcut. Not part of the Electron API.</remarks>
        public string ProxyCredentials { get; set; }
    }
}