namespace ElectronNET.API
{
    using ElectronNET.AspNet.Hubs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Extension methods for mapping the Electron SignalR hub.
    /// </summary>
    public static class ElectronEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps the Electron SignalR hub to the /electron-hub endpoint.
        /// This is required when using SignalR-based startup modes.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The endpoint route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapElectronHub(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ElectronHub>("/electron-hub");
            return endpoints;
        }
    }
}
