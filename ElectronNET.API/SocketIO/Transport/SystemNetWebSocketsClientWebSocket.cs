using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketIOClient.Transport
{
    public class SystemNetWebSocketsClientWebSocket : IClientWebSocket
    {
        public SystemNetWebSocketsClientWebSocket(int eio)
        {
            _eio = eio;
            _textSubject = new Subject<string>();
            _bytesSubject = new Subject<byte[]>();
            TextObservable = _textSubject.AsObservable();
            BytesObservable = _bytesSubject.AsObservable();
            _ws = new ClientWebSocket();
            _listenCancellation = new CancellationTokenSource();
            _sendLock = new SemaphoreSlim(1, 1);
        }

        const int ReceiveChunkSize = 1024 * 8;

        readonly int _eio;
        readonly ClientWebSocket _ws;
        readonly Subject<string> _textSubject;
        readonly Subject<byte[]> _bytesSubject;
        readonly CancellationTokenSource _listenCancellation;
        readonly SemaphoreSlim _sendLock;

        public IObservable<string> TextObservable { get; }
        public IObservable<byte[]> BytesObservable { get; }

        private void Listen()
        {
            Task.Factory.StartNew(async() =>
            {
                while (true)
                {
                    if (_listenCancellation.IsCancellationRequested)
                    {
                        break;
                    }
                    var buffer = new byte[ReceiveChunkSize];
                    int count = 0;
                    WebSocketReceiveResult result = null;

                    while (_ws.State == WebSocketState.Open)
                    {
                        var subBuffer = new byte[ReceiveChunkSize];
                        try
                        {
                            result = await _ws.ReceiveAsync(new ArraySegment<byte>(subBuffer), CancellationToken.None).ConfigureAwait(false);

                            // resize
                            if (buffer.Length - count < result.Count)
                            {
                                Array.Resize(ref buffer, buffer.Length + result.Count);
                            }
                            Buffer.BlockCopy(subBuffer, 0, buffer, count, result.Count);
                            count += result.Count;
                            if (result.EndOfMessage)
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            _textSubject.OnError(e);
                            break;
                        }
                    }

                    if (result == null)
                    {
                        break;
                    }

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            string text = Encoding.UTF8.GetString(buffer, 0, count);
                            _textSubject.OnNext(text);
                            break;
                        case WebSocketMessageType.Binary:
                            byte[] bytes;
                            if (_eio == 3)
                            {
                                bytes = new byte[count - 1];
                                Buffer.BlockCopy(buffer, 1, bytes, 0, bytes.Length);
                            }
                            else
                            {
                                bytes = new byte[count];
                                Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);
                            }
                            _bytesSubject.OnNext(bytes);
                            break;
                        case WebSocketMessageType.Close:
                            _textSubject.OnError(new WebSocketException("Received a Close message"));
                            break;
                    }
                }
            });
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await _ws.ConnectAsync(uri, cancellationToken);
            Listen();
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
        }

        public async Task SendAsync(byte[] bytes, TransportMessageType type, bool endOfMessage, CancellationToken cancellationToken)
        {
            var msgType = WebSocketMessageType.Text;
            if (type == TransportMessageType.Binary)
            {
                msgType = WebSocketMessageType.Binary;
            }
            await _ws.SendAsync(new ArraySegment<byte>(bytes), msgType, endOfMessage, cancellationToken).ConfigureAwait(false);
        }

        public void AddHeader(string key, string val)
        {
            _ws.Options.SetRequestHeader(key, val);
        }

        public void Dispose()
        {
            _textSubject.Dispose();
            _bytesSubject.Dispose();
            _ws.Dispose();
        }
    }
}
