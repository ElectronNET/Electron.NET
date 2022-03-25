using System;
using System.Collections.Generic;
using System.Text;

namespace SocketIOClient.Messages
{
    public static class MessageFactory
    {
        private static IMessage CreateMessage(MessageType type)
        {
            switch (type)
            {
                case MessageType.Opened:
                    return new OpenedMessage();
                case MessageType.Ping:
                    return new PingMessage();
                case MessageType.Pong:
                    return new PongMessage();
                case MessageType.Connected:
                    return new ConnectedMessage();
                case MessageType.Disconnected:
                    return new DisconnectedMessage();
                case MessageType.EventMessage:
                    return new EventMessage();
                case MessageType.AckMessage:
                    return new ClientAckMessage();
                case MessageType.ErrorMessage:
                    return new ErrorMessage();
                case MessageType.BinaryMessage:
                    return new BinaryMessage();
                case MessageType.BinaryAckMessage:
                    return new ClientBinaryAckMessage();
            }
            return null;
        }

        public static IMessage CreateMessage(int eio, string msg)
        {
            var enums = Enum.GetValues(typeof(MessageType));
            foreach (MessageType item in enums)
            {
                string prefix = ((int)item).ToString();
                if (msg.StartsWith(prefix))
                {
                    IMessage result = CreateMessage(item);
                    if (result != null)
                    {
                        result.Eio = eio;
                        result.Read(msg.Substring(prefix.Length));
                        return result;
                    }
                }
            }
            return null;
        }

        public static OpenedMessage CreateOpenedMessage(string msg)
        {
            var openedMessage = new OpenedMessage();
            if (msg[0] == '0')
            {
                openedMessage.Eio = 4;
                openedMessage.Read(msg.Substring(1));
            }
            else
            {
                openedMessage.Eio = 3;
                int index = msg.IndexOf(':');
                openedMessage.Read(msg.Substring(index + 2));
            }
            return openedMessage;
        }
    }
}
