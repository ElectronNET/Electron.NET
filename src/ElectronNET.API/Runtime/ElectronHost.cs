namespace ElectronNET.Runtime
{
    using System.Collections.Immutable;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;

    /// <summary>
    /// Default implementation of <see cref="IElectronHost"/> that keeps track of the
    /// runtime state shared between the Electron.NET CLI bootstrapper and ASP.NET
    /// applications.
    /// </summary>
    public sealed class ElectronHost : IElectronHost
    {
        private readonly StartupManager startupManager;

        public ElectronHost()
        {
            this.Options = new ElectronNetOptions();
            this.startupManager = new StartupManager(this);
            this.startupManager.Initialize();
        }

        public string ElectronExtraArguments { get; set; }

        public int? ElectronSocketPort { get; set; }

        public int? AspNetWebPort { get; set; }

        public StartupMethod StartupMethod { get; set; }

        public DotnetAppType DotnetAppType { get; set; }

        public string ElectronExecutable { get; set; }

        public ImmutableList<string> ProcessArguments { get; set; }

        public BuildInfo BuildInfo { get; set; }

        public IElectronNetRuntimeController RuntimeController => this.RuntimeControllerCore;

        public int? ElectronProcessId { get; set; }

        public ElectronNetOptions Options { get; private set; }

        internal RuntimeControllerBase RuntimeControllerCore { get; set; }

        public SocketIoFacade GetSocket()
        {
            return this.RuntimeControllerCore?.Socket;
        }

        public void ApplyOptions(ElectronNetOptions options)
        {
            this.Options = options ?? new ElectronNetOptions();
        }
    }
}
