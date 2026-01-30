using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNET.Samples.BlazorSignalR.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddElectron();

// Configure Electron.NET with SignalR mode
builder.WebHost.UseElectron(args, async () =>
{
    Console.WriteLine("[SignalR Sample] ===== APP READY CALLBACK STARTED =====");
    
    var options = new BrowserWindowOptions
    {
        Show = false,
        Width = 1200,
        Height = 800,
        IsRunningBlazor = true, // Crucial for Blazor support
    };

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        options.AutoHideMenuBar = true;

    Console.WriteLine("[SignalR Sample] About to create window...");
    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    Console.WriteLine($"[SignalR Sample] Window created with ID: {browserWindow.Id}");
    
    browserWindow.OnReadyToShow += () =>
    {
        browserWindow.Show();
        Console.WriteLine("[SignalR Sample] Window ready and visible!");
    };

    Console.WriteLine("[SignalR Sample] Electron app ready callback executed!");
});

var app = builder.Build();

// Enable WebSockets (required for SignalR)
app.UseWebSockets();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

// Map the Electron SignalR hub (required for SignalR mode)
app.MapElectronHub();

app.MapStaticAssets();
app.MapRazorComponents<ElectronNET.Samples.BlazorSignalR.Components.App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("[SignalR Sample] Application configured and starting...");

app.Run();
