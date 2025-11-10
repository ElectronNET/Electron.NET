namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Docs: https://electronjs.org/docs/api/structures/extension
    /// </summary>
    public class Extension
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Copy of the extension's manifest data.
        /// </summary>
        public dynamic Manifest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The extension's file path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The extension's `chrome-extension://` URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
    }
}
