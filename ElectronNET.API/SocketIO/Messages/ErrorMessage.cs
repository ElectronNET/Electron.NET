using SocketIOClient.Transport;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SocketIOClient.Messages
{
    public class ErrorMessage : IMessage
    {
        public MessageType Type => MessageType.ErrorMessage;

        public string Message { get; set; }

        public string Namespace { get; set; }

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public int BinaryCount { get; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public void Read(string msg)
        {
            if (Eio == 3)
            {
                Message = msg.Trim('"');
            }
            else
            {
                int index = msg.IndexOf('{');
                if (index > 0)
                {
                    Namespace = msg.Substring(0, index - 1);
                    msg = msg.Substring(index);
                }
                var doc = JsonDocument.Parse(msg);
                Message = doc.RootElement.GetProperty("message").GetString();
            }
        }

        public string Write()
        {
            throw new NotImplementedException();
        }
    }
}
