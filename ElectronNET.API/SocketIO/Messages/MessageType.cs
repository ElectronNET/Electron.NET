namespace SocketIOClient.Messages
{
    public enum MessageType
    {
        Opened,
        Ping = 2,
        Pong,
        Connected = 40,
        Disconnected,
        EventMessage,
        AckMessage,
        ErrorMessage,
        BinaryMessage,
        BinaryAckMessage
    }
}
