using SocketIOClient.Transport;
using System;
using System.Collections.Generic;

namespace SocketIOClient.Messages
{
    public class PongMessage : IMessage
    {
        public MessageType Type => MessageType.Pong;

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public int BinaryCount { get; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public TimeSpan Duration { get; set; }

        public void Read(string msg)
        {
        }

        public string Write() => "3";
    }
}
