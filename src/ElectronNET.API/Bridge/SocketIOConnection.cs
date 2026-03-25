#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API;

using System;
using System.Threading.Tasks;
using ElectronNET.API.Serialization;
using SocketIO.Serializer.SystemTextJson;
using SocketIO = SocketIOClient.SocketIO;

internal class SocketIOConnection : ISocketConnection
{
    private readonly SocketIO _socket;
    private readonly object _lockObj = new object();
    private bool _isDisposed;

    public SocketIOConnection(string uri)
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
            _socket.On(eventName, _ => { Task.Run(action); });
        }
    }

    public void On<T>(string eventName, Action<T> action)
    {
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, response =>
            {
                var value = response.GetValue<T>();
                Task.Run(() => action(value));
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
            });
        }
    }

    public void Once<T>(string eventName, Action<T> action)
    {
        this.CheckDisposed();

        lock (_lockObj)
        {
            _socket.On(eventName, (socketIoResponse) =>
            {
                this.Off(eventName);
                Task.Run(() => action(socketIoResponse.GetValue<T>()));
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
            _socket.Off(eventName);
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