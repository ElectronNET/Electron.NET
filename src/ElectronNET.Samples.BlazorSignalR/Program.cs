using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNET.Samples.BlazorSignalR.Components;

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

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        options.AutoHideMenuBar = true;

    var browserWindow = await Electron.WindowManager.CreateWindowAsync(options);
    
    browserWindow.OnReadyToShow += () => browserWindow.Show();
});

var app = builder.Build();

// Enable WebSockets (required for SignalR)
app.UseWebSockets();

// Enable routing
app.UseRouting();

// Enable CORS
app.UseCors("ElectronPolicy");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Serve static files (CSS, JS, images, etc.)
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// UseAntiforgery must be between UseRouting and UseEndpoints
app.UseAntiforgery();

// Use endpoints for SignalR hub
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ElectronNET.AspNet.Hubs.ElectronHub>("/electron-hub");
});

app.MapStaticAssets();
app.MapRazorComponents<ElectronNET.Samples.BlazorSignalR.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
