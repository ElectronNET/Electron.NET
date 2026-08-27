namespace ElectronNET.Runtime
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lets a host application contribute to the <see cref="IServiceCollection"/> used to build the
    /// underlying SocketIOClient connection (e.g. to register a custom <see cref="System.Net.Http.HttpClient"/>
    /// for retries/observability, or add logging), without forking Electron.NET.
    /// </summary>
    public sealed class ElectronSocketIOOptions
    {
        /// <summary>
        /// Invoked with the SocketIOClient connection's own <see cref="IServiceCollection"/>, after
        /// Electron.NET's required registrations have already been applied. Combine multiple
        /// contributions with '+='; do not assign directly.
        /// </summary>
        public Action<IServiceCollection> ConfigureServices { get; set; }
    }
}
