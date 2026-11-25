using ElectronNET.API;
using ElectronNET.Samples.BlazorServer.Components;
using ElectronNET.Samples.BlazorServer.Desktop;

var builder = WebApplication.CreateBuilder(args);

// UI: Blazor with interactive server rendering. The components run in this .NET process and
// talk to the page in the Electron window via a SignalR connection.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Desktop: the bridge between the Blazor UI and Electron.
// Singletons, because there is one Electron main process (and one main window) per app.
builder.Services.AddSingleton<AppEvents>();
builder.Services.AddSingleton<WindowService>();
builder.Services.AddSingleton<NativeService>();

builder.Services.AddElectron();

// Called with the app's service provider once Electron is ready: creates the window and menu.
builder.UseElectron(args, ElectronStartup.CreateMainWindowAsync);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
