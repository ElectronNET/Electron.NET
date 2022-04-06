namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public static class BridgeSettings
    {
        /// <summary>
        /// Gets the socket port.
        /// </summary>
        /// <value>
        /// The socket port.
        /// </value>
        public static string SocketPort { get; internal set; }

        /// <summary>
        /// Gets the web port.
        /// </summary>
        /// <value>
        /// The web port.
        /// </value>
        public static string WebPort { get; internal set; }

        /// <summary>
        /// Manually set the port values instead of using the UseElectron extension method
        /// </summary>
        /// <param name="socketPort"></param>
        /// <param name="webPort"></param>
        public static void InitializePorts(int socketPort, int webPort)
        {
            SocketPort = socketPort.ToString();
            WebPort    = webPort.ToString();
        }
    }
}
