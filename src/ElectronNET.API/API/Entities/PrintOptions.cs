namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Print dpi
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class PrintDpi
    {
        /// <summary>
        /// Gets or sets the horizontal DPI.
        /// </summary>
        public float Horizontal { get; set; }

        /// <summary>
        /// Gets or sets the vertical DPI.
        /// </summary>
        public float Vertical { get; set; }
    }

    /// <summary>
    /// The page range to print
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class PrintPageRange
    {
        /// <summary>
        /// Gets or sets the starting page index (0-based).
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Gets or sets the ending page index (inclusive, 0-based).
        /// </summary>
        public int To { get; set; }
    }

    /// <summary>
    /// Print options
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class PrintOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to suppress print settings prompts.
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to print background graphics.
        /// </summary>
        public bool PrintBackground { get; set; }

        /// <summary>
        /// Gets or sets the printer device name to use.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page will be printed in color.
        /// </summary>
        public bool Color { get; set; }

        /// <summary>
        /// Gets or sets the margins for the print job. Use MarginType plus top/bottom/left/right for custom.
        /// Units for margins with webContents.print are pixels (per MCP); for webContents.printToPDF, inches.
        /// </summary>
        public Margins Margins { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to print in landscape orientation.
        /// </summary>
        public bool Landscape { get; set; }

        /// <summary>
        /// Gets or sets the scale factor of the web page.
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Gets or sets the number of pages to print per sheet.
        /// </summary>
        public int PagesPerSheet { get; set; }

        /// <summary>
        /// Gets or sets the number of copies to print.
        /// </summary>
        public int Copies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pages should be collated.
        /// </summary>
        public bool Collate { get; set; }

        /// <summary>
        /// Gets or sets the page range(s) to print. On macOS, only one range is honored.
        /// </summary>
        public PrintPageRange[] PageRanges { get; set; }

        /// <summary>
        /// Gets or sets the duplex mode of the printed web page. Can be simplex, shortEdge, or longEdge.
        /// </summary>
        public string DuplexMode { get; set; }

        /// <summary>
        /// Gets or sets the DPI settings for the print job.
        /// </summary>
        public PrintDpi Dpi { get; set; }

        /// <summary>
        /// Gets or sets the string to be printed as page header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the string to be printed as page footer.
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets the page size of the printed document. Can be A0–A6, Legal, Letter, Tabloid,
        /// or an object. For webContents.print, custom sizes use microns (Chromium validates width_microns/height_microns).
        /// </summary>
        public PageSize PageSize { get; set; }
    }
}