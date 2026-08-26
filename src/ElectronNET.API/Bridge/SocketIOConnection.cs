#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API.Serialization;
using SocketIOClient;
using SocketIO = SocketIOClient.SocketIO;
using SocketIOOptions = SocketIOClient.SocketIOOptions;

internal class SocketIOConnection : ISocketConnection
{
    private readonly SocketIO _socket;
    private readonly object _lockObj = new object();
    private bool _isDisposed;

    public SocketIOConnection(string uri, string authorization)
    {
        var opts = string.IsNullOrEmpty(authorization) ? new SocketIOOptions() : new SocketIOOptions
        {
            ExtraHeaders = new Dictionary<string, string>
            {
                ["authorization"] = authorization
            },
        };
        _socket = new SocketIO(new Uri(uri), opts, services => services.AddSystemTextJson(ElectronJson.Options));
        // Outgoing args are normalized to camelCase via SerializeArg in Emit.
    }

    public event EventHandler BridgeDisconnected;

    public event EventHandler BridgeConnected;

    public void Connect()
    {
        this.CheckDisposed();

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
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, _ =>
            {
                Task.Run(action);
                return Task.CompletedTask;
            });
        }
    }

    public void On<T>(string eventName, Action<T> action)
    {
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, ctx =>
            {
                var value = ctx.GetValue<T>(0);
                Task.Run(() => action(value));
                return Task.CompletedTask;
            });
        }
    }

    public void Once(string eventName, Action action)
    {
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, _ =>
            {
                this.Off(eventName);
                Task.Run(action);
                return Task.CompletedTask;
            });
        }
    }

    public void Once<T>(string eventName, Action<T> action)
    {
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, ctx =>
            {
                this.Off(eventName);
                Task.Run(() => action(ctx.GetValue<T>(0)));
                return Task.CompletedTask;
            });
        }
    }

    public void Off(string eventName)
    {
        if (_isDisposed)
        {
            return;
        }

        lock (_lockObj)
        {
            // SocketIOClient v4 has no Off(eventName) API; On() overwrites the
            // handler dictionary entry, so registering a no-op emulates removal.
            _socket.On(eventName, _ => Task.CompletedTask);
        }
    }

    public async Task Emit(string eventName, params object[] args)
    {
        if (!_isDisposed)
        {
            await _socket.EmitAsync(eventName, args).ConfigureAwait(false);
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isDisposed = true;
            _socket.Dispose();
        }
    }

    private void CheckDisposed()
    {
        if (this._isDisposed)
        {
            throw new ObjectDisposedException(nameof(SocketIOConnection));
        }
    }
}