using ElectronNET.Converter;
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities;

/// <summary>
/// 
/// </summary>
/// <remarks>Up-to-date with Electron API 39.2</remarks>
public class PrintToPDFOptions
{
    /// <summary>
    /// Gets or sets the paper orientation. `true` for landscape, `false` for portrait. Defaults to false.
    /// </summary>
    public bool Landscape { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to display header and footer. Defaults to false.
    /// </summary>
    public bool DisplayHeaderFooter { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to print background graphics. Defaults to false.
    /// </summary>
    public bool PrintBackground { get; set; } = false;

    /// <summary>
    /// Gets or sets the scale of the webpage rendering. Defaults to 1.
    /// </summary>
    public double Scale { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size of the generated PDF. Can be `A0`, `A1`, `A2`, `A3`, `A4`,
    /// `A5`, `A6`, `Legal`, `Letter`, `Tabloid`, `Ledger`, or an Object containing
    /// `height` and `width` in inches. Defaults to `Letter`.
    /// </summary>
    [JsonConverter(typeof(PageSizeConverter))]
    public PageSize PageSize { get; set; } = "Letter";

    /// <summary>
    /// Gets or sets the paper ranges to print, e.g., '1-5, 8, 11-13'. Defaults to the empty string,
    /// which means print all pages.
    /// </summary>
    public string PageRanges { get; set; } = "";

    /// <summary>
    /// Gets or sets the HTML template for the print header. Should be valid HTML markup with following
    /// classes used to inject printing values into them: `date` (formatted print date),
    /// `title` (document title), `url` (document location), `pageNumber` (current page
    /// number) and `totalPages` (total pages in the document). For example, `<span class="title"></span>`
    /// would generate span containing the title.
    /// </summary>
    public string HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the HTML template for the print footer. Should use the same format as the
    /// `headerTemplate`.
    /// </summary>
    public string FooterTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether to prefer page size as defined by css. Defaults to false, in
    /// which case the content will be scaled to fit the paper size.
    /// </summary>
    public bool PreferCSSPageSize { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to generate a tagged (accessible) PDF. Defaults to false.
    /// Experimental per Electron docs; the generated PDF may not adhere fully to PDF/UA and WCAG standards.
    /// </summary>
    public bool GenerateTaggedPDF { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to generate a PDF document outline from content headers. Defaults to false.
    /// Experimental per Electron docs.
    /// </summary>
    public bool GenerateDocumentOutline { get; set; } = false;

    public Margins Margins { get; set; }
}