namespace ElectronNET.API.Entities;

/// <summary>
/// Margins object used by webContents.print options and webContents.printToPDF.
/// </summary>
/// <remarks>Up-to-date with Electron API 39.2</remarks>
public class Margins
{
    /// <summary>
    /// Gets or sets the margin type. Can be `default`, `none`, `printableArea`, or `custom`. If `custom` is chosen,
    /// you will also need to specify `top`, `bottom`, `left`, and `right`.
    /// </summary>
    public string MarginType { get; set; }

    /// <summary>
    /// Gets or sets the top margin of the printed web page. Units depend on API:
    /// - webContents.print: pixels
    /// - webContents.printToPDF: inches
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin of the printed web page. Units depend on API:
    /// - webContents.print: pixels
    /// - webContents.printToPDF: inches
    /// </summary>
    public double Bottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin of the printed web page. Units depend on API:
    /// - webContents.print: pixels
    /// - webContents.printToPDF: inches
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the right margin of the printed web page. Units depend on API:
    /// - webContents.print: pixels
    /// - webContents.printToPDF: inches
    /// </summary>
    public double Right { get; set; }
}