namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class ClearStorageDataOptions
    {
        /// <summary>
        /// Should follow window.location.origin’s representation scheme://host:port.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// The types of storages to clear. Can contain: cookies, filesystem, indexdb,
        /// localstorage, shadercache, websql, serviceworkers, cachestorage.
        /// If not specified, all storage types are cleared.
        /// </summary>
        public string[] Storages { get; set; }

        /// <summary>
        /// The types of quotas to clear. Can contain: temporary. If not specified,
        /// all quotas are cleared. The <c>quotas</c> option is deprecated;
        /// "temporary" is the only remaining supported quota type.
        /// </summary>
        public string[] Quotas { get; set; }
    }
}