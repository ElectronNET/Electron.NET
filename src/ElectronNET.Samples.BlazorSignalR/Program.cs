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
// Note: Callback is registered now but executes after app starts
IServiceProvider? serviceProvider = null;

builder.WebHost.UseElectron(args, async () =>
{
    if (serviceProvider is null)
    {
        throw new InvalidOperationException("ServiceProvider not initialized. This callback should only execute after app.Build().");
    }

    var options = new BrowserWindowOptions
    {
        Show = false,
        Width = 1200,
        Height = 800,
        IsRunningBlazor = true,
    };

    // Log startup time using ILogger - serviceProvider is captured after app.Build()
    var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Electron.Startup");
    logger.LogInformation("App startup time until Electron launch: {ElapsedMilliseconds} ms", watch.ElapsedMilliseconds);

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        options.AutoHideMenuBar = true;

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

var app = builder.Build();
serviceProvider = app.Services; // Capture for use in Electron callback above

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
