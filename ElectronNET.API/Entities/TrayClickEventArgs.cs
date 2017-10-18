namespace ElectronNET.API
{
    public class TrayClickEventArgs
    {
        public bool AltKey { get; set; }
        public bool ShiftKey { get; set; }
        public bool CtrlKey { get; set; }
        public bool MetaKey { get; set; }
    }
}