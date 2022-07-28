using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketIOClient.Transport
{
    public class Eio4HttpPollingHandler : HttpPollingHandler
    {
        public Eio4HttpPollingHandler(HttpClient httpClient) : base(httpClient) { }

        const char Separator = '\u001E'; //1E 

        public override async Task PostAsync(string uri, IEnumerable<byte[]> bytes, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            foreach (var item in bytes)
            {
                builder.Append('b').Append(Convert.ToBase64String(item)).Append(Separator);
            }
            if (builder.Length == 0)
            {
                return;
            }
            string text = builder.ToString().TrimEnd(Separator);
            await PostAsync(uri, text, cancellationToken);
        }

        protected override void ProduceText(string text)
        {
            string[] items = text.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                if (item[0] == 'b')
                {
                    byte[] bytes = Convert.FromBase64String(item.Substring(1));
                    BytesSubject.OnNext(bytes);
                }
                else
                {
                    TextSubject.OnNext(item);
                }
            }
        }
    }
}
