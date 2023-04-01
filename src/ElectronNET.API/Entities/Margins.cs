namespace ElectronNET.API.Entities;

/// <summary>
/// 
/// </summary>
public class Margins
{
    /// <summary>
    /// Can be `default`, `none`, `printableArea`, or `custom`. If `custom` is chosen,
    /// you will also need to specify `top`, `bottom`, `left`, and `right`.
    /// </summary>
    public string MarginType { get; set; }

    /// <summary>
    /// The top margin of the printed web page, in pixels.
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// The bottom margin of the printed web page, in pixels.
    /// </summary>
    public int Bottom { get; set; }

    /// <summary>
    /// The left margin of the printed web page, in pixels.
    /// </summary>
    public int Left { get; set; }

    /// <summary>
    /// The right margin of the printed web page, in pixels.
    /// </summary>
    public int Right { get; set; }
}