namespace ElectronNET.API.Entities
{
    public class AddRepresentationOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
        public byte[] Buffer { get; set; }
        public string DataUrl { get; set; } 
    }
}