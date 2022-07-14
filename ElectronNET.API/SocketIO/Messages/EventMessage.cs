using SocketIOClient.Transport;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SocketIOClient.Messages
{
    public class EventMessage : IMessage
    {
        public MessageType Type => MessageType.EventMessage;

        public string Namespace { get; set; }

        public string Event { get; set; }

        public int Id { get; set; }

        public List<JsonElement> JsonElements { get; set; }

        public string Json { get; set; }

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public int BinaryCount { get; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public void Read(string msg)
        {
            int index = msg.IndexOf('[');
            int lastIndex = msg.LastIndexOf(',', index);
            if (lastIndex > -1)
            {
                string text = msg.Substring(0, index);
                Namespace = text.Substring(0, lastIndex);
                if (index - lastIndex > 1)
                {
                    Id = int.Parse(text.Substring(lastIndex + 1));
                }
            }
            else
            {
                if (index > 0)
                {
                    Id = int.Parse(msg.Substring(0, index));
                }
            }
            msg = msg.Substring(index);

            //int index = msg.IndexOf('[');
            //if (index > 0)
            //{
            //    Namespace = msg.Substring(0, index - 1);
            //    msg = msg.Substring(index);
            //}
            var array = JsonDocument.Parse(msg).RootElement.EnumerateArray();
            int i = -1;
            foreach (var item in array)
            {
                i++;
                if (i == 0)
                {
                    Event = item.GetString();
                    JsonElements = new List<JsonElement>();
                }
                else
                {
                    JsonElements.Add(item);
                }
            }
        }

        public string Write()
        {
            var builder = new StringBuilder();
            builder.Append("42");
            if (!string.IsNullOrEmpty(Namespace))
            {
                builder.Append(Namespace).Append(',');
            }
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
