using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

namespace ElectronNET.API;

internal class SocketIoFacade
{
    private readonly SocketIO _socket;

    public SocketIoFacade(string uri)
    {
        _socket = new SocketIO(uri);
        var jsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        });

        _socket.JsonSerializer = jsonSerializer;
    }

    public void Connect()
    {
        _socket.OnError += (sender, e) =>
        {
            Console.WriteLine($"BridgeConnector Error: {sender} {e}");
        };

        _socket.OnConnected += (_, _) =>
        {
            Console.WriteLine("BridgeConnector connected!");
        };

        _socket.OnDisconnected += (_, _) =>
        {
            Console.WriteLine("BridgeConnector disconnected!");
        };

        _socket.ConnectAsync().GetAwaiter().GetResult();
    }

    public void On(string eventName, Action action)
    {
        _socket.On(eventName, response =>
        {
            Task.Run(action);
        });
    }

    public void On<T>(string eventName, Action<T> action)
    {
        _socket.On(eventName, response =>
        {
            var value = response.GetValue<T>();
            Task.Run(() => action(value));
        });
    }        
        
    // TODO: Remove this method when SocketIoClient supports object deserialization
    public void On(string eventName, Action<object> action)
    {
        _socket.On(eventName, response =>
        {
            var value = response.GetValue<object>();
            Console.WriteLine($"Called Event {eventName} - data {value}");
            Task.Run(() => action(value));
        });
    }

    public void Once<T>(string eventName, Action<T> action)
    {
        _socket.On(eventName, (socketIoResponse) =>
        {
            _socket.Off(eventName);
            Task.Run(() => action(socketIoResponse.GetValue<T>()));
        });
    }

    public void Off(string eventName)
    {
        _socket.Off(eventName);
    }

    public async Task Emit(string eventName, params object[] args)
    {
        await _socket.EmitAsync(eventName, args);
    }
}