namespace ElectronNET.API
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using ElectronNET.AspNet.Hubs;

    /// <summary>
    /// SignalR-based facade that mimics the SocketIoFacade interface
    /// for compatibility with existing Electron API code.
    /// </summary>
    internal class SignalRFacade
    {
        private readonly IHubContext<ElectronHub> _hubContext;
        private string _connectionId;
        private readonly ConcurrentDictionary<string, Action<object>> _eventHandlers;
        private readonly object _lockObj = new object();

        public SignalRFacade(IHubContext<ElectronHub> hubContext)
        {
            _hubContext = hubContext;
            _eventHandlers = new ConcurrentDictionary<string, Action<object>>();
        }

        public event EventHandler BridgeDisconnected;
        public event EventHandler BridgeConnected;

        public void SetConnectionId(string connectionId)
        {
            _connectionId = connectionId;
            Console.WriteLine($"[SignalRFacade] Connection ID set: {connectionId}");
            this.BridgeConnected?.Invoke(this, EventArgs.Empty);
        }

        public void OnDisconnected()
        {
            Console.WriteLine($"[SignalRFacade] Connection {_connectionId} disconnected");
            this.BridgeDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public void On(string eventName, Action action)
        {
            lock (_lockObj)
            {
                _eventHandlers[eventName] = _ => Task.Run(action);
            }
        }

        public void On<T>(string eventName, Action<T> action)
        {
            lock (_lockObj)
            {
                _eventHandlers[eventName] = obj =>
                {
                    if (obj is T typedValue)
                    {
                        Task.Run(() => action(typedValue));
                    }
                };
            }
        }

        public void Once(string eventName, Action action)
        {
            lock (_lockObj)
            {
                _eventHandlers[eventName] = _ =>
                {
                    this.Off(eventName);
                    Task.Run(action);
                };
            }
        }

        public void Once<T>(string eventName, Action<T> action)
        {
            lock (_lockObj)
            {
                _eventHandlers[eventName] = obj =>
                {
                    this.Off(eventName);
                    if (obj is T typedValue)
                    {
                        Task.Run(() => action(typedValue));
                    }
                };
            }
        }

        public void Off(string eventName)
        {
            lock (_lockObj)
            {
                _eventHandlers.TryRemove(eventName, out _);
            }
        }

        public async Task Emit(string eventName, params object[] args)
        {
            if (string.IsNullOrEmpty(_connectionId))
            {
                Console.Error.WriteLine($"[SignalRFacade] Cannot emit '{eventName}' - no connection ID");
                return;
            }

            try
            {
                // Send message to specific Electron client
                await _hubContext.Clients.Client(_connectionId).SendAsync(eventName, args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SignalRFacade] Error emitting '{eventName}': {ex.Message}");
                throw;
            }
        }

        public void TriggerEvent(string eventName, object data)
        {
            if (_eventHandlers.TryGetValue(eventName, out var handler))
            {
                handler(data);
            }
        }

        public void DisposeSocket()
        {
            // SignalR connections are managed by ASP.NET Core
            _eventHandlers.Clear();
        }
    }
}
