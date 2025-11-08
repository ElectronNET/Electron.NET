namespace ElectronNET.API.Entities;

public class TitleBarOverlay
{
    private readonly bool? _value;

    public TitleBarOverlay()
    {
    }

    private TitleBarOverlay(bool value) : this() => _value = value;

    public string Color { get; set; }

    public double Height { get; set; }

    public string SymbolColor { get; set; }

    public static implicit operator bool?(TitleBarOverlay titleBarOverlay) => titleBarOverlay?._value;

    public static implicit operator TitleBarOverlay(bool value) => new(value);
}