namespace ElectronNET.API.Entities
{
    public class TrayClickEventResponse
    {
        public TrayClickEventArgs eventArgs { get; set; }
        public Rectangle bounds { get; set; }
    }
}