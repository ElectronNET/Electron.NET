namespace ElectronNET.API.Entities;

public class PageSize
{
    private readonly string _value;

    public PageSize()
    {
    }

    private PageSize(string value) : this() => _value = value;

    public double Height { get; set; }

    public double Width { get; set; }

    public static implicit operator string(PageSize pageSize) => pageSize?._value;

    public static implicit operator PageSize(string value) => new(value);
}