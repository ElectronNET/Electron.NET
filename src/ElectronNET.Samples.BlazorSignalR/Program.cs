using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNET.AspNet.Middleware;
using ElectronNET.AspNet.Services;

var watch = new System.Diagnostics.Stopwatch();
watch.Start();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("ElectronPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register Electron authentication service as singleton
builder.Services.AddSingleton<IElectronAuthenticationService, ElectronAuthenticationService>();

builder.Services.AddElectron();

// Configure Electron.NET with SignalR mode
builder.WebHost.UseElectron(args, async () =>
{
    var options = new BrowserWindowOptions
    {
        Show = false,
        Width = 1200,
        Height = 800,
        IsRunningBlazor = true,
    };

    Console.WriteLine($"App startup time until Electron launch: {watch.ElapsedMilliseconds} ms");

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        options.AutoHideMenuBar = true;

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

var app = builder.Build();

// Register authentication middleware FIRST (before routing, static files, etc.)
app.UseMiddleware<ElectronAuthenticationMiddleware>();

// Enable routing
app.UseRouting();

// Enable CORS
app.UseCors("ElectronPolicy");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

// Serve static files (CSS, JS, images, etc.)
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// UseAntiforgery must be after UseRouting
app.UseAntiforgery();

// Map SignalR hub for Electron communication
app.MapHub<ElectronNET.AspNet.Hubs.ElectronHub>("/electron-hub");

app.MapStaticAssets();
app.MapRazorComponents<ElectronNET.Samples.BlazorSignalR.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
