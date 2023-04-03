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
                .AddSingleton(provider => Dock.Instance);
    }
}
