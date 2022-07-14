using System;

namespace SocketIOClient.Extensions
{
    internal static class DisposableExtensions
    {
        public static void TryDispose(this IDisposable disposable)
        {
            disposable?.Dispose();
        }
    }
}
