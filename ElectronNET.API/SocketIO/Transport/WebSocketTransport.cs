using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SocketIOClient.JsonSerializer;

namespace SocketIOClient.Transport
{
    public class WebSocketTransport : BaseTransport
    {
        public WebSocketTransport(IClientWebSocket ws, SocketIOOptions options, IJsonSerializer jsonSerializer, ILogger logger)
            : base(options, jsonSerializer, logger)
        {
            _ws = ws;
            _sendLock = new SemaphoreSlim(1, 1);
            _ws.TextObservable.Subscribe(this);
            _ws.BytesObservable.Subscribe(this);
        }

        const int ReceiveChunkSize = 1024 * 8;
        const int SendChunkSize = 1024 * 8;

        readonly IClientWebSocket _ws;
        readonly SemaphoreSlim _sendLock;

        private async Task SendAsync(TransportMessageType type, byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                await _sendLock.WaitAsync().ConfigureAwait(false);
                if (type == TransportMessageType.Binary && Options.EIO == 3)
                {
                    byte[] buffer = new byte[bytes.Length + 1];
                    buffer[0] = 4;
                    Buffer.BlockCopy(bytes, 0, buffer, 1, bytes.Length);
                    bytes = buffer;
                }
                int pages = (int)Math.Ceiling(bytes.Length * 1.0 / SendChunkSize);
                for (int i = 0; i < pages; i++)
                {
                    int offset = i * SendChunkSize;
                    int length = SendChunkSize;
                    if (offset + length > bytes.Length)
                    {
                        length = bytes.Length - offset;
                    }
                    byte[] subBuffer = new byte[length];
                    Buffer.BlockCopy(bytes, offset, subBuffer, 0, subBuffer.Length);
                    bool endOfMessage = pages - 1 == i;
                    await _ws.SendAsync(subBuffer, type, endOfMessage, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public override async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await _ws.ConnectAsync(uri, cancellationToken);
        }

        public override async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await _ws.DisconnectAsync(cancellationToken);
        }

        public override async Task SendAsync(Payload payload, CancellationToken cancellationToken)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(payload.Text);
            await SendAsync(TransportMessageType.Text, bytes, cancellationToken);
            if (payload.Bytes != null)
            {
                foreach (var item in payload.Bytes)
                {
                    await SendAsync(TransportMessageType.Binary, item, cancellationToken);
                }
            }
        }

        public override void AddHeader(string key, string val) => _ws.AddHeader(key, val);

        public override void Dispose()
        {
            base.Dispose();
            _sendLock.Dispose();
        }
    }
}
