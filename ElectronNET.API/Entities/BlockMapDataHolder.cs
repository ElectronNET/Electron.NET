using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BlockMapDataHolder
    {
        /// <summary>
        /// The file size. Used to verify downloaded size (save one HTTP request to get length).
        /// Also used when block map data is embedded into the file(appimage, windows web installer package).
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// The block map file size. Used when block map data is embedded into the file (appimage, windows web installer package).
        /// This information can be obtained from the file itself, but it requires additional HTTP request,
        /// so, to reduce request count, block map size is specified in the update metadata too.
        /// </summary>
        public double BlockMapSize { get; set; }

        /// <summary>
        /// The file checksum.
        /// </summary>
        public string Sha512 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdminRightsRequired { get; set; }
    }
}
