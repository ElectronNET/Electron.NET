#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API;

using ElectronNET.API.Serialization;
using SocketIO.Serializer.SystemTextJson;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SocketIO = SocketIOClient.SocketIO;

internal class SocketIoFacade
{
    private readonly SocketIO _socket;
    private readonly object _lockObj = new object();

    public SocketIoFacade(string uri)
    {
        _socket = new SocketIO(uri);
        _socket.Serializer = new SystemTextJsonSerializer(ElectronJson.Options);
        // Use default System.Text.Json serializer from SocketIOClient.
        // Outgoing args are normalized to camelCase via SerializeArg in Emit.
    }

    public event EventHandler BridgeDisconnected;

    public event EventHandler BridgeConnected;

    public void Connect()
    {
        _socket.OnError += (sender, e) => { Console.WriteLine($"BridgeConnector Error: {sender} {e}"); };

        _socket.OnConnected += (_, _) =>
        {
            Console.WriteLine("BridgeConnector connected!");
            this.BridgeConnected?.Invoke(this, EventArgs.Empty);
        };

        _socket.OnDisconnected += (_, _) =>
        {
            Console.WriteLine("BridgeConnector disconnected!");
            this.BridgeDisconnected?.Invoke(this, EventArgs.Empty);
        };

        _socket.ConnectAsync().GetAwaiter().GetResult();
    }

    public void On(string eventName, Action action)
    {
        lock (_lockObj)
        {
            _socket.On(eventName, _ => { Task.Run(action); });
        }
    }

    public void On<T>(string eventName, Action<T> action)
    {
        lock (_lockObj)
        {
            _socket.On(eventName, response =>
            {
                var value = response.GetValue<T>();
                Task.Run(() => action(value));
            });
        }
    }

    // Keep object overload for compatibility; value will be a JsonElement boxed as object.
    public void On(string eventName, Action<object> action)
    {
        lock (_lockObj)
        {
            _socket.On(eventName, response =>
            {
                var value = (object)response.GetValue<JsonElement>();
                ////Console.WriteLine($"Called Event {eventName} - data {value}");
                Task.Run(() => action(value));
            });
        }
    }

    public void Once(string eventName, Action action)
    {
        lock (_lockObj)
        {
            _socket.On(eventName, _ =>
            {
                _socket.Off(eventName);
                Task.Run(action);
            });
        }
    }

    public void Once<T>(string eventName, Action<T> action)
    {
        lock (_lockObj)
        {
            _socket.On(eventName, (socketIoResponse) =>
            {
                _socket.Off(eventName);
                Task.Run(() => action(socketIoResponse.GetValue<T>()));
            });
        }
    }

    public void Off(string eventName)
    {
        lock (_lockObj)
        {
            _socket.Off(eventName);
        }
    }

    public async Task Emit(string eventName, params object[] args)
    {
        await _socket.EmitAsync(eventName, args).ConfigureAwait(false);
    }

    public void DisposeSocket()
    {
        _socket.Dispose();
    }
}
