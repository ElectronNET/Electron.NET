namespace ElectronNET.Runtime
{
    using System.Collections.Immutable;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;

    /// <summary>
    /// Represents the mutable runtime state for the Electron.NET host. Consumers can
    /// resolve this interface from dependency injection to inspect the current
    /// startup mode, configured ports and the active runtime controller.
    /// </summary>
    public interface IElectronHost
    {
        string ElectronExtraArguments { get; set; }

        int? ElectronSocketPort { get; set; }

        int? AspNetWebPort { get; set; }

        StartupMethod StartupMethod { get; set; }

        DotnetAppType DotnetAppType { get; set; }

        string ElectronExecutable { get; set; }

        ImmutableList<string> ProcessArguments { get; set; }

        BuildInfo BuildInfo { get; set; }

        IElectronNetRuntimeController RuntimeController { get; }

        int? ElectronProcessId { get; set; }

        ElectronNetOptions Options { get; }

        SocketIoFacade GetSocket();
    }
}
