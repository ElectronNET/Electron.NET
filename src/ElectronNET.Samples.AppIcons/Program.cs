using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElectron();

builder.WebHost.UseElectron(args, async () =>
{
    // The icons are copied next to the .NET assemblies (see the .csproj), so this works
    // both during development and in the packaged app (where the .NET app lives in resources/bin).
    var iconsDir = Path.Combine(AppContext.BaseDirectory, "Assets", "icons");

    var options = new BrowserWindowOptions
    {
        Width = 800,
        Height = 600,
        Show = false,
        Title = "App Icons Sample",
    };

    if (OperatingSystem.IsWindows())
    {
        // Window + taskbar icon. Packaged apps already get the icon from the executable,
        // but during development (unpackaged) the default Electron icon would be shown.
        options.Icon = Path.Combine(iconsDir, "app.ico");
        options.AutoHideMenuBar = true;
    }
    else if (OperatingSystem.IsLinux())
    {
        // Window icon. Linux has no icon embedded in the executable, so this is also relevant
        // for packaged apps (in addition to the icon of the desktop entry).
        options.Icon = Path.Combine(iconsDir, "linux", "512x512.png");
        options.AutoHideMenuBar = true;
    }
    else if (OperatingSystem.IsMacOS())
    {
        // macOS windows have no icon. In the packaged app, the dock icon comes from the .icns
        // inside the app bundle - but during development (unpackaged) the Electron icon would be
        // shown, so we set the dock icon explicitly (harmless for the packaged app).
        Electron.Dock.SetIcon(Path.Combine(iconsDir, "linux", "512x512.png"));
    }

    var window = await Electron.WindowManager.CreateWindowAsync(options);

    window.OnReadyToShow += () => window.Show();
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
