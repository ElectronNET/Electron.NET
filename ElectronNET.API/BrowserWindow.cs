namespace ElectronNET.API
{
    public class BrowserWindow
    {
        public int Id { get; private set; }

        internal BrowserWindow(int id) {
            Id = id;
        }

        public void Minimize()
        {
            BridgeConnector.Socket.Emit("browserWindow-minimize", Id);
        }
    }
}
