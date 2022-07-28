namespace SocketIOClient
{
    public class DisconnectReason
    {
        public static string IOServerDisconnect = "io server disconnect";
        public static string IOClientDisconnect = "io client disconnect";
        public static string PingTimeout = "ping timeout";
        public static string TransportClose = "transport close";
        public static string TransportError = "transport error";
    }
}
