namespace ElectronNET.AspNet.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;
    using ElectronNET;
    using ElectronNET.API;
    using ElectronNET.AspNet.Runtime;
    using ElectronNET.Runtime;

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
            
            // Notify the runtime controller about the connection
            var runtimeController = ElectronNetRuntime.RuntimeController as RuntimeControllerAspNetDotnetFirstSignalR;
            if (runtimeController != null)
            {
                runtimeController.OnSignalRConnected(Context.ConnectionId);
            }
            
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
            
            // Notify the runtime controller about the disconnection
            var runtimeController = ElectronNetRuntime.RuntimeController as RuntimeControllerAspNetDotnetFirstSignalR;
            if (runtimeController != null)
            {
                runtimeController.OnSignalRDisconnected();
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
        /// Receives events from Electron (e.g., "BrowserWindowCreated", "dialogResult").
        /// Called by Electron to send data back to .NET.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="args">The event arguments as an array</param>
        public async Task ElectronEvent(string eventName, object[] args)
        {
            Console.WriteLine($"[ElectronHub] Received event from Electron: {eventName}");
            
            // Get the SignalRFacade and trigger the event handlers
            var runtimeController = ElectronNetRuntime.RuntimeController as RuntimeControllerAspNetDotnetFirstSignalR;
            if (runtimeController?.SignalRSocket is SignalRFacade signalRFacade)
            {
                // Invoke the event handlers registered via On/Once
                signalRFacade.TriggerEvent(eventName, args ?? Array.Empty<object>());
            }
            
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
            
            // Forward to Electron client
            await Clients.Caller.SendAsync("electronApiCall", method, data);
            
            // TODO: Implement proper request-response pattern
            // For now, return null - will be enhanced in Phase 6
            return null;
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

    }
}
