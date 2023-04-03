namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Print dpi
    /// </summary>
    public class PrintDpi
    {
        /// <summary>
        /// The horizontal dpi
        /// </summary>
        public float Horizontal { get; set; }

        /// <summary>
        /// The vertical dpi
        /// </summary>
        public float Vertical { get; set; }
    }

    /// <summary>
    /// The page range to print
    /// </summary>
    public class PrintPageRange
    {
        /// <summary>
        /// From
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public int To { get; set; }
    }

    /// <summary>
    /// Print options
    /// </summary>
    public class PrintOptions
    {
        /// <summary>
        /// Don't ask user for print settings
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Prints the background color and image of the web page
        /// </summary>
        public bool PrintBackground { get; set; }

        /// <summary>
        /// Set the printer device name to use
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Set whether the printed web page will be in color or grayscale
        /// </summary>
        public bool Color { get; set; }

        /// <summary>
        /// Specifies the type of margins to use. Uses 0 for default margin, 1 for no
        /// margin, and 2 for minimum margin.
        /// </summary>
        public int MarginsType { get; set; }

        /// <summary>
        /// true for landscape, false for portrait.
        /// </summary>
        public bool Landscape { get; set; }
        
        /// <summary>
        /// The scale factor of the web page
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// The number of pages to print per page sheet
        /// </summary>
        public int PagesPerSheet { get; set; }

        /// <summary>
        /// The number of copies of the web page to print
        /// </summary>
        public bool Copies { get; set; }

        /// <summary>
        /// Whether the web page should be collated
        /// </summary>
        public bool Collate { get; set; }

        /// <summary>
        /// The page range to print
        /// </summary>
        public PrintPageRange PageRanges { get; set; }

        /// <summary>
        /// Set the duplex mode of the printed web page. Can be simplex, shortEdge, or longEdge.
        /// </summary>
        public string DuplexMode { get; set; }

        /// <summary>
        /// Dpi
        /// </summary>
        public PrintDpi Dpi { get; set; }

    }
}