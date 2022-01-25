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
                .AddSingleton(_ => IpcMain.Instance)
                .AddSingleton(_ => App.Instance)
                .AddSingleton(_ => AutoUpdater.Instance)
                .AddSingleton(_ => WindowManager.Instance)
                .AddSingleton(_ => Menu.Instance)
                .AddSingleton(_ => Dialog.Instance)
                .AddSingleton(_ => Notification.Instance)
                .AddSingleton(_ => Tray.Instance)
                .AddSingleton(_ => GlobalShortcut.Instance)
                .AddSingleton(_ => Shell.Instance)
                .AddSingleton(_ => Screen.Instance)
                .AddSingleton(_ => Clipboard.Instance)
                .AddSingleton(_ => HostHook.Instance)
                .AddSingleton(_ => PowerMonitor.Instance)
                .AddSingleton(_ => NativeTheme.Instance)
                .AddSingleton(_ => Dock.Instance)
                .AddSingleton(_ => Process.Instance)
                .AddSingleton(_ => new ApplicationSocket { Socket = BridgeConnector.Socket, })
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
