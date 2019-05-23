using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ProgressInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Progress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BytesPerSecond { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Transferred { get; set; }
    }
}
