using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Starts the Electron shell and invokes ElectronAppReady once it is up.
builder.UseElectron(args, ElectronAppReady);

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

// Fully qualified because ElectronNET.API also defines an 'App' type.
app.MapRazorComponents<ElectronBlazorApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task ElectronAppReady()
{
    var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
    {
        Width = 1152,
        Height = 940,
        Show = false,
    });

    window.OnReadyToShow += () => window.Show();
}
