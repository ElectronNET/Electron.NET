using ElectronNET.Converter;
using Newtonsoft.Json;

namespace ElectronNET.API.Entities;

/// <summary>
/// 
/// </summary>
public class PrintToPDFOptions
{
    /// <summary>
    /// Paper orientation. `true` for landscape, `false` for portrait. Defaults to false.
    /// </summary>
    public bool Landscape { get; set; } = false;

    /// <summary>
    /// Whether to display header and footer. Defaults to false.
    /// </summary>
    public bool DisplayHeaderFooter { get; set; } = false;

    /// <summary>
    /// Whether to print background graphics. Defaults to false.
    /// </summary>
    public bool PrintBackground { get; set; } = false;

    /// <summary>
    /// Scale of the webpage rendering. Defaults to 1.
    /// </summary>
    public double Scale { get; set; } = 1;

    /// <summary>
    /// Specify page size of the generated PDF. Can be `A0`, `A1`, `A2`, `A3`, `A4`,
    /// `A5`, `A6`, `Legal`, `Letter`, `Tabloid`, `Ledger`, or an Object containing
    /// `height` and `width` in inches. Defaults to `Letter`.
    /// </summary>
    [JsonConverter(typeof(PageSizeConverter))]
    public PageSize PageSize { get; set; } = "Letter";

    /// <summary>
    /// Paper ranges to print, e.g., '1-5, 8, 11-13'. Defaults to the empty string,
    /// which means print all pages.
    /// </summary>
    public string PageRanges { get; set; } = "";

    /// <summary>
    /// HTML template for the print header. Should be valid HTML markup with following
    /// classes used to inject printing values into them: `date` (formatted print date),
    /// `title` (document title), `url` (document location), `pageNumber` (current page
    /// number) and `totalPages` (total pages in the document). For example, `<span class="title"></span>`
    /// would generate span containing the title.
    /// </summary>
    public string HeaderTemplate { get; set; }

    /// <summary>
    /// HTML template for the print footer. Should use the same format as the
    /// `headerTemplate`.
    /// </summary>
    public string FooterTemplate { get; set; }

    /// <summary>
    /// Whether or not to prefer page size as defined by css. Defaults to false, in
    /// which case the content will be scaled to fit the paper size.
    /// </summary>
    public bool PreferCSSPageSize { get; set; } = false;

    public Margins Margins { get; set; }
}