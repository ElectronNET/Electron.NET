using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronNET.API.Entities
{
    public class ResizeOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        /// <summary>
        /// good, better, or best. Default is "best";
        /// </summary>
        public string Quality { get; set; } = "best";
    }
}
