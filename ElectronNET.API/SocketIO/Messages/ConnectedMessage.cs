using System;
using SocketIOClient.Transport;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SocketIOClient.Messages
{
    public class ConnectedMessage : IMessage
    {
        public MessageType Type => MessageType.Connected;

        public string Namespace { get; set; }

        public string Sid { get; set; }

        public List<byte[]> OutgoingBytes { get; set; }

        public List<byte[]> IncomingBytes { get; set; }

        public int BinaryCount { get; }

        public int Eio { get; set; }

        public TransportProtocol Protocol { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Query { get; set; }
        public string AuthJsonStr { get; set; }

        public void Read(string msg)
        {
            if (Eio == 3)
            {
                Eio3Read(msg);
            }
            else
            {
                Eio4Read(msg);
            }
        }

        public string Write()
        {
            if (Eio == 3)
            {
                return Eio3Write();
            }
            return Eio4Write();
        }

        public void Eio4Read(string msg)
        {
            int index = msg.IndexOf('{');
            if (index > 0)
            {
                Namespace = msg.Substring(0, index - 1);
                msg = msg.Substring(index);
            }
            else
            {
                Namespace = string.Empty;
            }
            Sid = JsonDocument.Parse(msg).RootElement.GetProperty("sid").GetString();
        }

        public string Eio4Write()
        {
            var builder = new StringBuilder("40");
            if (!string.IsNullOrEmpty(Namespace))
            {
                builder.Append(Namespace).Append(',');
            }
            builder.Append(AuthJsonStr);
            return builder.ToString();
        }

        public void Eio3Read(string msg)
        {
            if (msg.Length >= 2)
            {
                int startIndex = msg.IndexOf('/');
                if (startIndex == -1)
                {
                    return;
                }
                int endIndex = msg.IndexOf('?', startIndex);
                if (endIndex == -1)
                {
                    endIndex = msg.IndexOf(',', startIndex);
                }
                if (endIndex == -1)
                {
                    endIndex = msg.Length;
                }
                Namespace = msg.Substring(startIndex, endIndex);
            }
        }

        public string Eio3Write()
        {
            if (string.IsNullOrEmpty(Namespace))
            {
                return string.Empty;
            }
            var builder = new StringBuilder("40");
            builder.Append(Namespace);
            if (Query != null)
            {
                int i = -1;
                foreach (var item in Query)
                {
                    i++;
                    if (i == 0)
                    {
                        builder.Append('?');
                    }
                    else
                    {
                        builder.Append('&');
                    }
                    builder.Append(item.Key).Append('=').Append(item.Value);
                }
            }
            builder.Append(',');
            return builder.ToString();
        }
    }
}
