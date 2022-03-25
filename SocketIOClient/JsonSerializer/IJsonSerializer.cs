using System.Collections.Generic;

namespace SocketIOClient.JsonSerializer
{
    public interface IJsonSerializer
    {
        JsonSerializeResult Serialize(object[] data);
        T Deserialize<T>(string json);
        T Deserialize<T>(string json, IList<byte[]> incomingBytes);
    }
}
