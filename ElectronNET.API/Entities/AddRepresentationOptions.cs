namespace ElectronNET.API.Entities
{
    public class AddRepresentationOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int ScaleFactor { get; set; }
        public byte[] Buffer { get; set; }
        public string DataUrl { get; set; } 
    }
}