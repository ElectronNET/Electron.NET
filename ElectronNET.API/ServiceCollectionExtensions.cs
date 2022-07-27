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
                .AddSingleton(provider => new ApplicationSocket { Socket = BridgeConnector.GetSocket(), })
                // this set for proper dependency injection
                .AddSingleton<IIpcMain>(_ => IpcMain.Instance)
                .AddSingleton<IApp>(_ => App.Instance)
                .AddSingleton<IAutoUpdater>(_ => AutoUpdater.Instance)
                .AddSingleton<IWindowManager>(_ => WindowManager.Instance)
                .AddSingleton<IMenu>(_ => Menu.Instance)
                .AddSingleton<IDialog>(_ => Dialog.Instance)
                .AddSingleton<INotification>(_ => Notification.Instance)
                .AddSingleton<ITray>(_ => Tray.Instance)
                .AddSingleton<IGlobalShortcut>(_ => GlobalShortcut.Instance)
                .AddSingleton<IShell>(_ => Shell.Instance)
                .AddSingleton<IScreen>(_ => Screen.Instance)
                .AddSingleton<IClipboard>(_ => Clipboard.Instance)
                .AddSingleton<IHostHook>(_ => HostHook.Instance)
                .AddSingleton<IPowerMonitor>(_ => PowerMonitor.Instance)
                .AddSingleton<INativeTheme>(_ => NativeTheme.Instance)
                .AddSingleton<IDock>(_ => Dock.Instance)
                .AddSingleton<IProcess>(_ => Process.Instance)
                .AddSingleton<IApplicationSocket>(provider => provider.GetService<ApplicationSocket>());
    }
}
