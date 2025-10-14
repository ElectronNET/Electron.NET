namespace ElectronNET.API
{
    using Microsoft.Extensions.DependencyInjection;

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
                .AddSingleton(_ => Dock.Instance);
    }
}
