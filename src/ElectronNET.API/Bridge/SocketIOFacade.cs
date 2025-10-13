﻿#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API;

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketIO.Serializer.NewtonsoftJson;
using SocketIO = SocketIOClient.SocketIO;

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

        _socket.Serializer = jsonSerializer;
    }

    public event EventHandler BridgeDisconnected;

    public event EventHandler BridgeConnected;

    public void Connect()
    {
        _socket.OnError += (sender, e) =>
        {
            Console.WriteLine($"BridgeConnector Error: {sender} {e}");
        };

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
        _socket.On(eventName, _ =>
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
            ////Console.WriteLine($"Called Event {eventName} - data {value}");
            Task.Run(() => action(value));
        });
    }

    public void Once(string eventName, Action action)
    {
        _socket.On(eventName, _ =>
        {
            _socket.Off(eventName);
            Task.Run(action);
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
        await _socket.EmitAsync(eventName, args).ConfigureAwait(false);
    }

    public void DisposeSocket()
    {
        _socket.Dispose();
    }
}