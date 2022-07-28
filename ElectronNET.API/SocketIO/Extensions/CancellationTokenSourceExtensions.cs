using System;
using System.Threading;

namespace SocketIOClient.Extensions
{
    internal static class CancellationTokenSourceExtensions
    {
        public static void TryDispose(this CancellationTokenSource cts)
        {
            cts?.Dispose();
        }

        public static void TryCancel(this CancellationTokenSource cts)
        {
            cts?.Cancel();
        }
    }
}
