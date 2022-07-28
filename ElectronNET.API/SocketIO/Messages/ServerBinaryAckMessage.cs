using SocketIOClient.Transport;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SocketIOClient.Messages
{
    /// <summary>
    /// The client calls the server's callback with binary
    /// </summary>
    public class ServerBinaryAckMessage : IMessage
    {
        public MessageType Type => MessageType.BinaryAckMessage;

        public string Namespace { get; set; }

        public List<JsonElement> JsonElements { get; set; }

        public string Json { get; set; }

        public int Id { get; set; }

        public int BinaryCount { get; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public void Read(string msg)
        {
        }

        public string Write()
        {
            var builder = new StringBuilder();
            builder
                .Append("46")
                .Append(OutgoingBytes.Count)
                .Append('-');
            if (!string.IsNullOrEmpty(Namespace))
            {
                builder.Append(Namespace).Append(',');
            }
            builder.Append(Id);
            if (string.IsNullOrEmpty(Json))
            {
                builder.Append("[]");
            }
            else
            {
                builder.Append(Json);
            }
            return builder.ToString();
        }
    }
}
