namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BrowserViewConstructorOptions
    {
        /// <summary>
        /// See BrowserWindow.
        /// </summary>
        public WebPreferences WebPreferences { get; set; }

        /// <summary>
        /// A proxy to set on creation in the format host:port.
        /// The proxy can be alternatively set using the BrowserView.WebContents.SetProxyAsync function.
        /// </summary>
        public string Proxy { get; set; }

        /// <summary>
        /// The credentials of the Proxy in the format username:password.
        /// These will only be used if the Proxy field is also set.
        /// </summary>
        public string ProxyCredentials { get; set; }
    }
}
