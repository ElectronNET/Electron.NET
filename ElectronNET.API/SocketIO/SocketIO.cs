using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SocketIOClient.Extensions;
using SocketIOClient.JsonSerializer;
using SocketIOClient.Messages;
using SocketIOClient.Transport;
using SocketIOClient.UriConverters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SocketIOClient
{
    /// <summary>
    /// socket.io client class
    /// </summary>
    public class SocketIO : IDisposable
    {
        /// <summary>
        /// Create SocketIO object with default options
        /// </summary>
        /// <param name="uri"></param>
        public SocketIO(string uri) : this(new Uri(uri)) { }

        /// <summary>
        /// Create SocketIO object with options
        /// </summary>
        /// <param name="uri"></param>
        public SocketIO(Uri uri) : this(uri, new SocketIOOptions()) { }

        /// <summary>
        /// Create SocketIO object with options
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="options"></param>
        public SocketIO(string uri, SocketIOOptions options) : this(new Uri(uri), options) { }

        /// <summary>
        /// Create SocketIO object with options
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="options"></param>
        public SocketIO(Uri uri, SocketIOOptions options)
        {
            ServerUri = uri ?? throw new ArgumentNullException("uri");
            Options = options ?? throw new ArgumentNullException("options");
            Initialize();
        }

        Uri _serverUri;
        private Uri ServerUri
        {
            get => _serverUri;
            set
            {
                if (_serverUri != value)
                {
                    _serverUri = value;
                    if (value != null && value.AbsolutePath != "/")
                    {
                        _namespace = value.AbsolutePath;
                    }
                }
            }
        }

        /// <summary>
        /// An unique identifier for the socket session. Set after the connect event is triggered, and updated after the reconnect event.
        /// </summary>
        public string Id { get; private set; }

        string _namespace;

        /// <summary>
        /// Whether or not the socket is connected to the server.
        /// </summary>
        public bool Connected { get; private set; }

        int _attempts;

        [Obsolete]
        /// <summary>
        /// Whether or not the socket is disconnected from the server.
        /// </summary>
        public bool Disconnected => !Connected;

        public SocketIOOptions Options { get; }

        public IJsonSerializer JsonSerializer { get; set; }

        public IUriConverter UriConverter { get; set; }

        internal ILogger Logger { get; set; }

        ILoggerFactory _loggerFactory;
        public ILoggerFactory LoggerFactory
        {
            get => _loggerFactory;
            set
            {
                _loggerFactory = value ?? throw new ArgumentNullException(nameof(LoggerFactory));
                Logger = _loggerFactory.CreateLogger<SocketIO>();
            }
        }

        public HttpClient HttpClient { get; set; }

        public Func<IClientWebSocket> ClientWebSocketProvider { get; set; }
        private IClientWebSocket _clientWebsocket;

        BaseTransport _transport;

        List<Type> _expectedExceptions;

        int _packetId;
        bool _isConnectCoreRunning;
        Uri _realServerUri;
        Exception _connectCoreException;
        Dictionary<int, Action<SocketIOResponse>> _ackHandlers;
        List<OnAnyHandler> _onAnyHandlers;
        Dictionary<string, Action<SocketIOResponse>> _eventHandlers;
        CancellationTokenSource _connectionTokenSource;
        double _reconnectionDelay;
        bool _hasError;
        bool _isFaild;
        readonly static object _connectionLock = new object();

        #region Socket.IO event
        public event EventHandler OnConnected;
        //public event EventHandler<string> OnConnectError;
        //public event EventHandler<string> OnConnectTimeout;
        public event EventHandler<string> OnError;
        public event EventHandler<string> OnDisconnected;

        /// <summary>
        /// Fired upon a successful reconnection.
        /// </summary>
        public event EventHandler<int> OnReconnected;

        /// <summary>
        /// Fired upon an attempt to reconnect.
        /// </summary>
        public event EventHandler<int> OnReconnectAttempt;

        /// <summary>
        /// Fired upon a reconnection attempt error.
        /// </summary>
        public event EventHandler<Exception> OnReconnectError;

        /// <summary>
        /// Fired when couldn’t reconnect within reconnectionAttempts
        /// </summary>
        public event EventHandler OnReconnectFailed;
        public event EventHandler OnPing;
        public event EventHandler<TimeSpan> OnPong;

        #endregion

        #region Observable Event
        //Subject<Unit> _onConnected;
        //public IObservable<Unit> ConnectedObservable { get; private set; }
        #endregion

        private void Initialize()
        {
            _packetId = -1;
            _ackHandlers = new Dictionary<int, Action<SocketIOResponse>>();
            _eventHandlers = new Dictionary<string, Action<SocketIOResponse>>();
            _onAnyHandlers = new List<OnAnyHandler>();

            JsonSerializer = new SystemTextJsonSerializer();
            UriConverter = new UriConverter();

            HttpClient = new HttpClient();
            ClientWebSocketProvider = () => new SystemNetWebSocketsClientWebSocket(Options.EIO);
            _expectedExceptions = new List<Type>
            {
                typeof(TimeoutException),
                typeof(WebSocketException),
                typeof(HttpRequestException),
                typeof(OperationCanceledException),
                typeof(TaskCanceledException)
            };
            LoggerFactory = NullLoggerFactory.Instance;
        }

        private async Task CreateTransportAsync()
        {
            Options.Transport = await GetProtocolAsync();
            if (Options.Transport == TransportProtocol.Polling)
            {
                HttpPollingHandler handler;
                if (Options.EIO == 3)
                    handler = new Eio3HttpPollingHandler(HttpClient);
                else
                    handler = new Eio4HttpPollingHandler(HttpClient);
                _transport = new HttpTransport(HttpClient, handler, Options, JsonSerializer, Logger);
            }
            else
            {
                _clientWebsocket = ClientWebSocketProvider();
                _transport = new WebSocketTransport(_clientWebsocket, Options, JsonSerializer, Logger);
            }
            _transport.Namespace = _namespace;
            SetHeaders();
        }

        private void SetHeaders()
        {
            if (Options.ExtraHeaders != null)
            {
                foreach (var item in Options.ExtraHeaders)
                {
                    _transport.AddHeader(item.Key, item.Value);
                }
            }
        }

        private void SyncExceptionToMain(Exception e)
        {
            _connectCoreException = e;
            _isConnectCoreRunning = false;
        }

        private void ConnectCore()
        {
            DisposeForReconnect();
            _reconnectionDelay = Options.ReconnectionDelay;
            _connectionTokenSource = new CancellationTokenSource();
            var cct = _connectionTokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    _clientWebsocket.TryDispose();
                    _transport.TryDispose();
                    CreateTransportAsync().Wait();
                    _realServerUri = UriConverter.GetServerUri(Options.Transport == TransportProtocol.WebSocket, ServerUri, Options.EIO, Options.Path, Options.Query);
                    try
                    {
                        if (cct.IsCancellationRequested)
                            break;
                        if (_attempts > 0)
                            OnReconnectAttempt.TryInvoke(this, _attempts);
                        var timeoutCts = new CancellationTokenSource(Options.ConnectionTimeout);
                        _transport.Subscribe(OnMessageReceived, OnErrorReceived);
                        await _transport.ConnectAsync(_realServerUri, timeoutCts.Token).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (_expectedExceptions.Contains(e.GetType()))
                        {
                            if (!Options.Reconnection)
                            {
                                SyncExceptionToMain(e);
                                throw;
                            }
                            if (_attempts > 0)
                            {
                                OnReconnectError.TryInvoke(this, e);
                            }
                            _attempts++;
                            if (_attempts <= Options.ReconnectionAttempts)
                            {
                                if (_reconnectionDelay < Options.ReconnectionDelayMax)
                                {
                                    _reconnectionDelay += 2 * Options.RandomizationFactor;
                                }
                                if (_reconnectionDelay > Options.ReconnectionDelayMax)
                                {
                                    _reconnectionDelay = Options.ReconnectionDelayMax;
                                }
                                Thread.Sleep((int)_reconnectionDelay);
                            }
                            else
                            {
                                _isFaild = true;
                                OnReconnectFailed.TryInvoke(this, EventArgs.Empty);
                                break;
                            }
                        }
                        else
                        {
                            SyncExceptionToMain(e);
                            throw;
                        }
                    }
                }
                _isConnectCoreRunning = false;
            });
        }

        private async Task<TransportProtocol> GetProtocolAsync()
        {
            if (Options.Transport == TransportProtocol.Polling && Options.AutoUpgrade)
            {
                Uri uri = UriConverter.GetServerUri(false, ServerUri, Options.EIO, Options.Path, Options.Query);
                try
                {
                    string text = await HttpClient.GetStringAsync(uri);
                    if (text.Contains("websocket"))
                    {
                        return TransportProtocol.WebSocket;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e, e.Message);
                }
            }
            return Options.Transport;
        }

        public async Task ConnectAsync()
        {
            if (Connected || _isConnectCoreRunning)
                return;

            lock (_connectionLock)
            {
                if (_isConnectCoreRunning)
                    return;
                _isConnectCoreRunning = true;
            }
            ConnectCore();
            while (_isConnectCoreRunning)
            {
                await Task.Delay(100);
            }
            if (_connectCoreException != null)
            {
                Logger.LogError(_connectCoreException, _connectCoreException.Message);
                throw _connectCoreException;
            }
            int ms = 0;
            while (!Connected)
            {
                if (_hasError)
                {
                    Logger.LogWarning($"Got a connection error, try to use '{nameof(OnError)}' to detect it.");
                    break;
                }
                if (_isFaild)
                {
                    Logger.LogWarning($"Reconnect failed, try to use '{nameof(OnReconnectFailed)}' to detect it.");
                    break;
                }
                ms += 100;
                if (ms > Options.ConnectionTimeout.TotalMilliseconds)
                {
                    throw new TimeoutException();
                }
                await Task.Delay(100);
            }
        }

        private void PingHandler()
        {
            OnPing.TryInvoke(this, EventArgs.Empty);
        }

        private void PongHandler(PongMessage msg)
        {
            OnPong.TryInvoke(this, msg.Duration);
        }

        private void ConnectedHandler(ConnectedMessage msg)
        {
            Id = msg.Sid;
            Connected = true;
            OnConnected.TryInvoke(this, EventArgs.Empty);
            if (_attempts > 0)
            {
                OnReconnected.TryInvoke(this, _attempts);
            }
            _attempts = 0;
        }

        private void DisconnectedHandler()
        {
            _ = InvokeDisconnect(DisconnectReason.IOServerDisconnect);
        }

        private void EventMessageHandler(EventMessage m)
        {
            var res = new SocketIOResponse(m.JsonElements, this)
            {
                PacketId = m.Id
            };
            foreach (var item in _onAnyHandlers)
            {
                item.TryInvoke(m.Event, res);
            }
            if (_eventHandlers.ContainsKey(m.Event))
            {
                _eventHandlers[m.Event].TryInvoke(res);
            }
        }

        private void AckMessageHandler(ClientAckMessage m)
        {
            if (_ackHandlers.ContainsKey(m.Id))
            {
                var res = new SocketIOResponse(m.JsonElements, this);
                _ackHandlers[m.Id].TryInvoke(res);
                _ackHandlers.Remove(m.Id);
            }
        }

        private void ErrorMessageHandler(ErrorMessage msg)
        {
            _hasError = true;
            OnError.TryInvoke(this, msg.Message);
        }

        private void BinaryMessageHandler(BinaryMessage msg)
        {
            var response = new SocketIOResponse(msg.JsonElements, this)
            {
                PacketId = msg.Id,
            };
            response.InComingBytes.AddRange(msg.IncomingBytes);
            foreach (var item in _onAnyHandlers)
            {
                item.TryInvoke(msg.Event, response);
            }
            if (_eventHandlers.ContainsKey(msg.Event))
            {
                _eventHandlers[msg.Event].TryInvoke(response);
            }
        }

        private void BinaryAckMessageHandler(ClientBinaryAckMessage msg)
        {
            if (_ackHandlers.ContainsKey(msg.Id))
            {
                var response = new SocketIOResponse(msg.JsonElements, this)
                {
                    PacketId = msg.Id,
                };
                response.InComingBytes.AddRange(msg.IncomingBytes);
                _ackHandlers[msg.Id].TryInvoke(response);
            }
        }

        private void OnErrorReceived(Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            _ = InvokeDisconnect(DisconnectReason.TransportClose);
        }

        private void OnMessageReceived(IMessage msg)
        {
            try
            {
                switch (msg.Type)
                {
                    case MessageType.Ping:
                        PingHandler();
                        break;
                    case MessageType.Pong:
                        PongHandler(msg as PongMessage);
                        break;
                    case MessageType.Connected:
                        ConnectedHandler(msg as ConnectedMessage);
                        break;
                    case MessageType.Disconnected:
                        DisconnectedHandler();
                        break;
                    case MessageType.EventMessage:
                        EventMessageHandler(msg as EventMessage);
                        break;
                    case MessageType.AckMessage:
                        AckMessageHandler(msg as ClientAckMessage);
                        break;
                    case MessageType.ErrorMessage:
                        ErrorMessageHandler(msg as ErrorMessage);
                        break;
                    case MessageType.BinaryMessage:
                        BinaryMessageHandler(msg as BinaryMessage);
                        break;
                    case MessageType.BinaryAckMessage:
                        BinaryAckMessageHandler(msg as ClientBinaryAckMessage);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
        }

        public async Task DisconnectAsync()
        {
            if (Connected)
            {
                var msg = new DisconnectedMessage
                {
                    Namespace = _namespace
                };
                try
                {
                    await _transport.SendAsync(msg, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
                await InvokeDisconnect(DisconnectReason.IOClientDisconnect);
            }
        }

        /// <summary>
        /// Register a new handler for the given event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void On(string eventName, Action<SocketIOResponse> callback)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers.Remove(eventName);
            }
            _eventHandlers.Add(eventName, callback);
        }



        /// <summary>
        /// Unregister a new handler for the given event.
        /// </summary>
        /// <param name="eventName"></param>
        public void Off(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers.Remove(eventName);
            }
        }

        public void OnAny(OnAnyHandler handler)
        {
            if (handler != null)
            {
                _onAnyHandlers.Add(handler);
            }
        }

        public void PrependAny(OnAnyHandler handler)
        {
            if (handler != null)
            {
                _onAnyHandlers.Insert(0, handler);
            }
        }

        public void OffAny(OnAnyHandler handler)
        {
            if (handler != null)
            {
                _onAnyHandlers.Remove(handler);
            }
        }

        public OnAnyHandler[] ListenersAny() => _onAnyHandlers.ToArray();

        internal async Task ClientAckAsync(int packetId, CancellationToken cancellationToken, params object[] data)
        {
            IMessage msg;
            if (data != null && data.Length > 0)
            {
                var result = JsonSerializer.Serialize(data);
                if (result.Bytes.Count > 0)
                {
                    msg = new ServerBinaryAckMessage
                    {
                        Id = packetId,
                        Namespace = _namespace,
                        Json = result.Json
                    };
                    msg.OutgoingBytes = new List<byte[]>(result.Bytes);
                }
                else
                {
                    msg = new ServerAckMessage
                    {
                        Namespace = _namespace,
                        Id = packetId,
                        Json = result.Json
                    };
                }
            }
            else
            {
                msg = new ServerAckMessage
                {
                    Namespace = _namespace,
                    Id = packetId
                };
            }
            await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Emits an event to the socket
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data">Any other parameters can be included. All serializable datastructures are supported, including byte[]</param>
        /// <returns></returns>
        public async Task EmitAsync(string eventName, params object[] data)
        {
            await EmitAsync(eventName, CancellationToken.None, data).ConfigureAwait(false);
        }

        public async Task EmitAsync(string eventName, CancellationToken cancellationToken, params object[] data)
        {
            if (data != null && data.Length > 0)
            {
                var result = JsonSerializer.Serialize(data);
                if (result.Bytes.Count > 0)
                {
                    var msg = new BinaryMessage
                    {
                        Namespace = _namespace,
                        OutgoingBytes = new List<byte[]>(result.Bytes),
                        Event = eventName,
                        Json = result.Json
                    };
                    await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var msg = new EventMessage
                    {
                        Namespace = _namespace,
                        Event = eventName,
                        Json = result.Json
                    };
                    await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                var msg = new EventMessage
                {
                    Namespace = _namespace,
                    Event = eventName
                };
                await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Emits an event to the socket
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="ack">will be called with the server answer.</param>
        /// <param name="data">Any other parameters can be included. All serializable datastructures are supported, including byte[]</param>
        /// <returns></returns>
        public async Task EmitAsync(string eventName, Action<SocketIOResponse> ack, params object[] data)
        {
            await EmitAsync(eventName, CancellationToken.None, ack, data).ConfigureAwait(false);
        }

        public async Task EmitAsync(string eventName, CancellationToken cancellationToken, Action<SocketIOResponse> ack, params object[] data)
        {
            _ackHandlers.Add(++_packetId, ack);
            if (data != null && data.Length > 0)
            {
                var result = JsonSerializer.Serialize(data);
                if (result.Bytes.Count > 0)
                {
                    var msg = new ClientBinaryAckMessage
                    {
                        Event = eventName,
                        Namespace = _namespace,
                        Json = result.Json,
                        Id = _packetId,
                        OutgoingBytes = new List<byte[]>(result.Bytes)
                    };
                    await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var msg = new ClientAckMessage
                    {
                        Event = eventName,
                        Namespace = _namespace,
                        Id = _packetId,
                        Json = result.Json
                    };
                    await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                var msg = new ClientAckMessage
                {
                    Event = eventName,
                    Namespace = _namespace,
                    Id = _packetId
                };
                await _transport.SendAsync(msg, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task InvokeDisconnect(string reason)
        {
            if (Connected)
            {
                Connected = false;
                Id = null;
                OnDisconnected.TryInvoke(this, reason);
                try
                {
                    await _transport.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                }
                if (reason != DisconnectReason.IOServerDisconnect && reason != DisconnectReason.IOClientDisconnect)
                {
                    //In the this cases (explicit disconnection), the client will not try to reconnect and you need to manually call socket.connect().
                    if (Options.Reconnection)
                    {
                        ConnectCore();
                    }
                }
            }
        }

        public void AddExpectedException(Type type)
        {
            if (!_expectedExceptions.Contains(type))
            {
                _expectedExceptions.Add(type);
            }
        }

        private void DisposeForReconnect()
        {
            _hasError = false;
            _isFaild = false;
            _packetId = -1;
            _ackHandlers.Clear();
            _connectCoreException = null;
            _hasError = false;
            _connectionTokenSource.TryCancel();
            _connectionTokenSource.TryDispose();
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            _transport.TryDispose();
            _ackHandlers.Clear();
            _onAnyHandlers.Clear();
            _eventHandlers.Clear();
            _connectionTokenSource.TryCancel();
            _connectionTokenSource.TryDispose();
        }
    }
}