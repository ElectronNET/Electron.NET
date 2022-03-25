using SocketIOClient.Transport;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SocketIOClient.Messages
{
    /// <summary>
    /// The server calls the client's callback with binary
    /// </summary>
    public class ClientBinaryAckMessage : IMessage
    {
        public MessageType Type => MessageType.BinaryAckMessage;

        public string Namespace { get; set; }

        public string Event { get; set; }

        public List<JsonElement> JsonElements { get; set; }

        public string Json { get; set; }

        public int Id { get; set; }

        public int BinaryCount { get; set; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public void Read(string msg)
        {
            int index1 = msg.IndexOf('-');
            BinaryCount = int.Parse(msg.Substring(0, index1));

            int index2 = msg.IndexOf('[');

            int index3 = msg.LastIndexOf(',', index2);
            if (index3 > -1)
            {
                Namespace = msg.Substring(index1 + 1, index3 - index1 - 1);
                Id = int.Parse(msg.Substring(index3 + 1, index2 - index3 - 1));
            }
            else
            {
                Id = int.Parse(msg.Substring(index1 + 1, index2 - index1 - 1));
            }

            string json = msg.Substring(index2);
            JsonElements = JsonDocument.Parse(json).RootElement.EnumerateArray().ToList();
        }

        public string Write()
        {
            var builder = new StringBuilder();
            builder
                .Append("45")
                .Append(OutgoingBytes.Count)
                .Append('-');
            if (!string.IsNullOrEmpty(Namespace))
            {
                builder.Append(Namespace).Append(',');
            }
            builder.Append(Id);
            if (string.IsNullOrEmpty(Json))
            {
                builder.Append("[\"").Append(Event).Append("\"]");
            }
            else
            {
                string data = Json.Insert(1, $"\"{Event}\",");
                builder.Append(data);
            }
            return builder.ToString();
        }
    }
}
