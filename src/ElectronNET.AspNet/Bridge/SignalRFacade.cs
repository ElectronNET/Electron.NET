namespace ElectronNET.API
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using ElectronNET.API.Bridge;
    using ElectronNET.AspNet.Hubs;

    /// <summary>
    /// SignalR-based facade that mimics the SocketIoFacade interface
    /// for compatibility with existing Electron API code.
    /// </summary>
    internal class SignalRFacade : IFacade
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

        /// <summary>
        /// SignalR connections are managed by ASP.NET Core, so this is a no-op.
        /// Connection establishment happens via the ElectronHub.
        /// </summary>
        public void Connect()
        {
            // No-op: SignalR connection is managed by ASP.NET Core
        }

        public void SetConnectionId(string connectionId)
        {
            _connectionId = connectionId;
            this.BridgeConnected?.Invoke(this, EventArgs.Empty);
        }

        public void OnDisconnected()
        {
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
                    var converted = ConvertToType<T>(obj);
                    if (converted != null)
                    {
                        Task.Run(() => action(converted));
                    }
                    else
                    {
                        Console.Error.WriteLine($"[SignalRFacade] Failed to convert event data to type {typeof(T).Name}");
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
                    var converted = ConvertToType<T>(obj);
                    if (converted != null)
                    {
                        Task.Run(() => action(converted));
                    }
                    else
                    {
                        Console.Error.WriteLine($"[SignalRFacade] Failed to convert event data to type {typeof(T).Name} for event '{eventName}'");
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
                // Send message to specific Electron client via the 'event' hub method
                // This will be received by signalr-bridge.js's connection.on('event', ...)
                await _hubContext.Clients.Client(_connectionId).SendAsync("event", eventName, args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SignalRFacade] Error emitting '{eventName}': {ex.Message}");
                throw;
            }
        }

        public void TriggerEvent(string eventName, params object[] args)
        {
            if (_eventHandlers.TryGetValue(eventName, out var handler))
            {
                // If single arg, pass it directly; otherwise pass the array
                var data = args.Length == 1 ? args[0] : args;
                handler(data);
            }
        }

        /// <summary>
        /// Converts an object to the specified type, handling JsonElement and numeric conversions.
        /// </summary>
        private static T ConvertToType<T>(object obj)
        {
            if (obj == null)
                return default;

            // Direct type match
            if (obj is T typedValue)
                return typedValue;

            var targetType = typeof(T);
            
            // Handle JsonElement (common from SignalR deserialization)
            if (obj is JsonElement jsonElement)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[SignalRFacade] JsonElement deserialization failed: {ex.Message}");
                    return default;
                }
            }

            // Handle numeric conversions (SignalR often sends numbers as long/double)
            try
            {
                if (targetType == typeof(int) || targetType == typeof(int?))
                {
                    return (T)(object)Convert.ToInt32(obj);
                }
                if (targetType == typeof(long) || targetType == typeof(long?))
                {
                    return (T)(object)Convert.ToInt64(obj);
                }
                if (targetType == typeof(double) || targetType == typeof(double?))
                {
                    return (T)(object)Convert.ToDouble(obj);
                }
                if (targetType == typeof(bool) || targetType == typeof(bool?))
                {
                    return (T)(object)Convert.ToBoolean(obj);
                }
                if (targetType == typeof(string))
                {
                    return (T)(object)obj.ToString();
                }

                // For arrays, try JSON serialization roundtrip
                if (targetType.IsArray && obj is object[] arr)
                {
                    var json = JsonSerializer.Serialize(arr);
                    return JsonSerializer.Deserialize<T>(json);
                }

                // Last resort: try to serialize and deserialize
                var serialized = JsonSerializer.Serialize(obj);
                return JsonSerializer.Deserialize<T>(serialized);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SignalRFacade] Type conversion failed from {obj.GetType().Name} to {targetType.Name}: {ex.Message}");
                return default;
            }
        }

        public void DisposeSocket()
        {
            // SignalR connections are managed by ASP.NET Core
            _eventHandlers.Clear();
        }
    }
}
