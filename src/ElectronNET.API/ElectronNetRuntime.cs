namespace ElectronNET
{
    using ElectronNET.API;
    using ElectronNET.Runtime;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public static class ElectronNetRuntime
    {
        internal static StartupManager StartupManager;

        internal const int DefaultSocketPort = 8000;
        internal const int DefaultWebPort = 8001;
        internal const string ElectronPortArgumentName = "electronPort";
        internal const string ElectronPidArgumentName = "electronPID";

        /// <summary>Initializes the <see cref="ElectronNetRuntime"/> class.</summary>
        static ElectronNetRuntime()
        {
            StartupManager = new StartupManager();
            StartupManager.Initialize();
        }

        public static string ElectronExtraArguments { get; set; }

        public static int? ElectronSocketPort { get; internal set; }

        public static int? AspNetWebPort { get; internal set; }

        public static StartupMethod StartupMethod { get; internal set; }

        public static DotnetAppType DotnetAppType { get; internal set; }

        public static string ElectronExecutable { get; internal set; }

        public static ImmutableList<string> ProcessArguments { get; internal set; }

        public static BuildInfo BuildInfo { get; internal set; }

        public static IElectronNetRuntimeController RuntimeController => RuntimeControllerCore;

        // The below properties are non-public
        internal static RuntimeControllerBase RuntimeControllerCore { get; set; }

        internal static int? ElectronProcessId { get; set; }

        /// <summary>
        /// Global configuration options for the Electron.NET runtime, including
        /// lifecycle events that can be configured from the ASP.NET host builder.
        /// </summary>
        public static ElectronNetOptions Options { get; set; } = new ElectronNetOptions();

        internal static SocketIoFacade GetSocket()
        {
            return RuntimeControllerCore?.Socket;
        }
    }
}