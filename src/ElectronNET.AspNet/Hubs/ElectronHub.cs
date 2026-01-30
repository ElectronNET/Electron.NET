namespace ElectronNET.AspNet.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// SignalR hub for bidirectional communication between ASP.NET Core and Electron.
    /// Replaces socket.io for SignalR-based startup modes.
    /// </summary>
    public class ElectronHub : Hub
    {
        /// <summary>
        /// Called when Electron client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[ElectronHub] Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when Electron client disconnects from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[ElectronHub] Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                Console.WriteLine($"[ElectronHub] Disconnect reason: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Registers the Electron client. Called by Electron on connection.
        /// </summary>
        public async Task RegisterElectronClient()
        {
            Console.WriteLine($"[ElectronHub] Electron client registered: {Context.ConnectionId}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Invokes an Electron API method. Called by .NET to control Electron.
        /// </summary>
        /// <param name="method">The API method name (e.g., "createWindow", "showDialog")</param>
        /// <param name="data">The method parameters as JSON</param>
        /// <returns>The result of the API call</returns>
        public async Task<string> InvokeElectronApi(string method, string data)
        {
            Console.WriteLine($"[ElectronHub] InvokeElectronApi called: {method}");
            
            // This will be called by .NET code
            // We need to forward it to the Electron client
            var result = await Clients.Caller.SendAsync("electronApiCall", method, data);
            return result?.ToString();
        }

        /// <summary>
        /// Handles responses from Electron API calls.
        /// Called by Electron to send results back to .NET.
        /// </summary>
        /// <param name="callId">The unique identifier for this API call</param>
        /// <param name="result">The result data as JSON</param>
        public async Task ElectronApiResponse(string callId, string result)
        {
            Console.WriteLine($"[ElectronHub] ElectronApiResponse received: {callId}");
            // This will be handled by the SignalR facade to complete pending tasks
            await Task.CompletedTask;
        }

        /// <summary>
        /// Handles events from Electron.
        /// Called by Electron to notify .NET about events (e.g., window closed).
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="eventData">The event data as JSON</param>
        public async Task ElectronEvent(string eventName, string eventData)
        {
            Console.WriteLine($"[ElectronHub] ElectronEvent received: {eventName}");
            // This will be handled by the event system
            await Task.CompletedTask;
        }
    }
}
