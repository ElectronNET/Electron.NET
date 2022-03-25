using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocketIOClient.Transport
{
    public interface IClientWebSocket : IDisposable
    {
        IObservable<string> TextObservable { get; }
        IObservable<byte[]> BytesObservable { get; }
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
        Task DisconnectAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] bytes, TransportMessageType type, bool endOfMessage, CancellationToken cancellationToken);
        void AddHeader(string key, string val);
    }
}
