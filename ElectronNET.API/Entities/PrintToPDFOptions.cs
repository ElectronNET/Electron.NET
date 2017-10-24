namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintToPDFOptions
    {
        /// <summary>
        /// Specifies the type of margins to use. Uses 0 for default margin, 1 for no
        /// margin, and 2 for minimum margin.
        /// </summary>
        public int MarginsType { get; set; }

        /// <summary>
        /// Specify page size of the generated PDF. Can be A3, A4, A5, Legal, Letter,
        /// Tabloid or an Object containing height and width in microns.
        /// </summary>
        public string PageSize { get; set; }

        /// <summary>
        /// Whether to print CSS backgrounds.
        /// </summary>
        public bool PrintBackground { get; set; }

        /// <summary>
        /// Whether to print selection only.
        /// </summary>
        public bool PrintSelectionOnly { get; set; }

        /// <summary>
        /// true for landscape, false for portrait.
        /// </summary>
        public bool Landscape { get; set; }
    }
}