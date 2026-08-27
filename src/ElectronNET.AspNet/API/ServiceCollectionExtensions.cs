namespace ElectronNET.API
{
    using System;
    using ElectronNET.Runtime;
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

        /// <summary>
        /// Lets the app customize the underlying SocketIOClient connection used to bridge to
        /// Electron - e.g. register a custom <see cref="System.Net.Http.HttpClient"/> for
        /// retries/observability, or add logging - without forking Electron.NET. Can be called
        /// multiple times; contributions combine instead of overwriting each other. Call this on
        /// the same <see cref="IServiceCollection"/> passed to <c>UseElectron(...)</c>.
        /// </summary>
        public static IServiceCollection ConfigureElectronSocketIO(this IServiceCollection services, Action<IServiceCollection> configure)
            => services.Configure<ElectronSocketIOOptions>(options => options.ConfigureServices += configure);
    }
}
