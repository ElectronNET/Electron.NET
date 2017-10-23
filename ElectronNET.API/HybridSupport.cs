namespace ElectronNET.API
{
    public static class HybridSupport
    {
        public static bool IsElectronActive
        {
            get
            {
                return !string.IsNullOrEmpty(BridgeSettings.SocketPort);
            }
        }
    }
}