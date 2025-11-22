namespace ElectronNET.API.Entities;

public class PageSize
{
    private readonly string _value;

    /// <summary>
    /// Represents the page size for printing/PDF.
    /// Matches Electron semantics: either a named size (e.g. 'A4', 'Letter', 'Legal', 'Tabloid', 'Ledger', etc.)
    /// or a custom size specified by Height and Width in inches.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public PageSize()
    {
    }

    private PageSize(string value) : this() => _value = value;

    /// <summary>
    /// Gets or sets the custom page height in inches (when using object form instead of a named size).
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the custom page width in inches (when using object form instead of a named size).
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Implicit conversion to string to represent named page sizes (e.g. 'A4', 'Letter').
    /// </summary>
    public static implicit operator string(PageSize pageSize) => pageSize?._value;

    /// <summary>
    /// Implicit conversion from string to represent named page sizes (e.g. 'A4', 'Letter').
    /// </summary>
    public static implicit operator PageSize(string value) => new(value);
}