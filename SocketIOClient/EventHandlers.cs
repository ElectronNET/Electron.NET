using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SocketIOClient
{
    public delegate void OnAnyHandler(string eventName, SocketIOResponse response);
    public delegate void OnOpenedHandler(string sid, int pingInterval, int pingTimeout);
    //public delegate void OnDisconnectedHandler(string sid, int pingInterval, int pingTimeout);
    public delegate void OnAck(int packetId, List<JsonElement> array);
    public delegate void OnBinaryAck(int packetId, int totalCount, List<JsonElement> array);
    public delegate void OnBinaryReceived(int packetId, int totalCount, string eventName, List<JsonElement> array);
    public delegate void OnDisconnected();
    public delegate void OnError(string error);
    public delegate void OnEventReceived(int packetId, string eventName, List<JsonElement> array);
    public delegate void OnOpened(string sid, int pingInterval, int pingTimeout);
    public delegate void OnPing();
    public delegate void OnPong();
}
