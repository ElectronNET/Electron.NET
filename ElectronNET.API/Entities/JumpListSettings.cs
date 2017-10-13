namespace ElectronNET.API.Entities
{
    public class JumpListSettings
    {
        public int MinItems { get; set; } = 0;

        public JumpListItem[] RemovedItems { get; set; } = new JumpListItem[0];
    }
}
