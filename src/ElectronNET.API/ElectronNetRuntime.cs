namespace ElectronNET
{
    using ElectronNET.API;
    using ElectronNET.Runtime;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public static class ElectronNetRuntime
    {
        internal static StartupManager StartupManager;

        private static readonly ElectronSocketIOOptions SocketIOOptions = new ElectronSocketIOOptions();

        internal const int DefaultSocketPort = 8000;
        internal const int DefaultWebPort = 8001;
        internal const string ElectronPortArgumentName = "electronPort";
        internal const string ElectronPidArgumentName = "electronPID";
        internal const string ElectronAuthTokenArgumentName = "electronAuthToken";

        /// <summary>Initializes the <see cref="ElectronNetRuntime"/> class.</summary>
        static ElectronNetRuntime()
        {
            StartupManager = new StartupManager();
            StartupManager.Initialize();
        }

        public static string ElectronExtraArguments { get; set; }

        public static string ElectronAuthToken { get; internal set; }

        public static int? ElectronSocketPort { get; internal set; }

        public static int? AspNetWebPort { get; internal set; }

        public static StartupMethod StartupMethod { get; internal set; }

        public static DotnetAppType DotnetAppType { get; internal set; }

        public static string ElectronExecutable { get; internal set; }

        public static ImmutableList<string> ProcessArguments { get; internal set; }

        public static BuildInfo BuildInfo { get; internal set; }

        public static IElectronNetRuntimeController RuntimeController => RuntimeControllerCore;

        /// <summary>
        /// Lets a plain (non ASP.NET Core hosted) app customize the underlying SocketIOClient
        /// connection - e.g. register a custom <see cref="System.Net.Http.HttpClient"/> for
        /// retries/observability, or add logging. Must be called before the runtime controller is
        /// started (ideally at the very top of Main, since first touching this class triggers
        /// eager bootstrap). For ASP.NET Core apps, use IServiceCollection.ConfigureElectronSocketIO(...)
        /// instead.
        /// </summary>
        public static void ConfigureSocketIO(Action<IServiceCollection> configure)
        {
            SocketIOOptions.ConfigureServices += configure;
        }

        // The below properties are non-public
        internal static RuntimeControllerBase RuntimeControllerCore { get; set; }

        internal static int? ElectronProcessId { get; set; }

        internal static Action<IServiceCollection> SocketIOConfiguration => SocketIOOptions.ConfigureServices;

        internal static ISocketConnection GetSocket()
        {
            return RuntimeControllerCore?.Socket;
        }
    }
}