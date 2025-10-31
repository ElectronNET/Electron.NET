namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class SemVer
    {
        /// <summary>
        /// 
        /// </summary>
       public string  Raw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Loose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SemVerOptions Options { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Patch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] Build { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] Prerelease { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SemVerOptions {
        /// <summary>
        /// 
        /// </summary>
        public bool? Loose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludePrerelease { get; set; }
    }
}
