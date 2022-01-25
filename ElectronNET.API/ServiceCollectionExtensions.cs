using ElectronNET.API.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="Electron"/> Members to the Service Collection
        /// </summary>
        public static IServiceCollection AddElectron(this IServiceCollection services)
            => services
                // adding in this manner to ensure late binding.
                // this set for backwards compatibility
                .AddSingleton(provider => IpcMain.Instance)
                .AddSingleton(provider => App.Instance)
                .AddSingleton(provider => AutoUpdater.Instance)
                .AddSingleton(provider => WindowManager.Instance)
                .AddSingleton(provider => Menu.Instance)
                .AddSingleton(provider => Dialog.Instance)
                .AddSingleton(provider => Notification.Instance)
                .AddSingleton(provider => Tray.Instance)
                .AddSingleton(provider => GlobalShortcut.Instance)
                .AddSingleton(provider => Shell.Instance)
                .AddSingleton(provider => Screen.Instance)
                .AddSingleton(provider => Clipboard.Instance)
                .AddSingleton(provider => HostHook.Instance)
                .AddSingleton(provider => PowerMonitor.Instance)
                .AddSingleton(provider => NativeTheme.Instance)
                .AddSingleton(provider => Dock.Instance)
                .AddSingleton(provider => Process.Instance)
                .AddSingleton(provider => new ApplicationSocket { Socket = BridgeConnector.Socket, })
                // this set for proper dependency injection
                .AddSingleton<IIpcMain>(provider => IpcMain.Instance)
                .AddSingleton<IApp>(provider => App.Instance)
                .AddSingleton<IAutoUpdater>(provider => AutoUpdater.Instance)
                .AddSingleton<IWindowManager>(provider => WindowManager.Instance)
                .AddSingleton<IMenu>(provider => Menu.Instance)
                .AddSingleton<IDialog>(provider => Dialog.Instance)
                .AddSingleton<INotification>(provider => Notification.Instance)
                .AddSingleton<ITray>(provider => Tray.Instance)
                .AddSingleton<IGlobalShortcut>(provider => GlobalShortcut.Instance)
                .AddSingleton<IShell>(provider => Shell.Instance)
                .AddSingleton<IScreen>(provider => Screen.Instance)
                .AddSingleton<IClipboard>(provider => Clipboard.Instance)
                .AddSingleton<IHostHook>(provider => HostHook.Instance)
                .AddSingleton<IPowerMonitor>(provider => PowerMonitor.Instance)
                .AddSingleton<INativeTheme>(provider => NativeTheme.Instance)
                .AddSingleton<IDock>(provider => Dock.Instance)
                .AddSingleton<IApplicationSocket>(provider => provider.GetService<ApplicationSocket>());
    }
}
