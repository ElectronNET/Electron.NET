namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Proxy configuration for app.setProxy / session.setProxy. Matches Electron's ProxyConfig structure.
    /// </summary>
    public class ProxyConfig
    {
        /// <summary>
        /// The proxy mode. One of: 'direct' | 'auto_detect' | 'pac_script' | 'fixed_servers' | 'system'.
        /// Defaults to 'pac_script' if 'PacScript' is specified, otherwise defaults to 'fixed_servers'.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// The URL associated with the PAC file.
        /// </summary>
        public string PacScript { get; set; }

        /// <summary>
        /// Rules indicating which proxies to use.
        /// </summary>
        public string ProxyRules { get; set; }

        /// <summary>
        /// Rules indicating which URLs should bypass the proxy settings.
        /// </summary>
        public string ProxyBypassRules { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pacScript">The URL associated with the PAC file.</param>
        /// <param name="proxyRules">Rules indicating which proxies to use.</param>
        /// <param name="proxyBypassRules">Rules indicating which URLs should bypass the proxy settings.</param>
        public ProxyConfig(string pacScript, string proxyRules, string proxyBypassRules)
        {
            PacScript = pacScript;
            ProxyRules = proxyRules;
            ProxyBypassRules = proxyBypassRules;
        }
    }
}