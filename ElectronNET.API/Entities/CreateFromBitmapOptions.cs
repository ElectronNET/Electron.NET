using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronNET.API.Entities
{
    public class CreateFromBitmapOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
    }
}
