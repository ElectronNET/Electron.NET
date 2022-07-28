using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SocketIOClient.JsonSerializer;
using SocketIOClient.Messages;

namespace SocketIOClient.Transport
{
    public class HttpTransport : BaseTransport
    {
        public HttpTransport(HttpClient http,
            IHttpPollingHandler pollingHandler,
            SocketIOOptions options,
            IJsonSerializer jsonSerializer,
            ILogger logger) : base(options, jsonSerializer, logger)
        {
            _http = http;
            _httpPollingHandler = pollingHandler;
            _httpPollingHandler.TextObservable.Subscribe(this);
            _httpPollingHandler.BytesObservable.Subscribe(this);
        }

        string _httpUri;
        CancellationTokenSource _pollingTokenSource;

        readonly HttpClient _http;
        readonly IHttpPollingHandler _httpPollingHandler;

        private void StartPolling(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                int retry = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_httpUri.Contains("&sid="))
                    {
                        await Task.Delay(20);
                        continue;
                    }
                    try
                    {
                        await _httpPollingHandler.GetAsync(_httpUri, CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        retry++;
                        if (retry >= 3)
                        {
                            MessageSubject.OnError(e);
                            break;
                        }
                        await Task.Delay(100 * (int)Math.Pow(2, retry));
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public override async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            // if (_options.ExtraHeaders != null)
            // {
            //     foreach (var item in _options.ExtraHeaders)
            //     {
            //         req.Headers.Add(item.Key, item.Value);
            //     }
            // }

            _httpUri = uri.ToString();
            await _httpPollingHandler.SendAsync(req, new CancellationTokenSource(Options.ConnectionTimeout).Token).ConfigureAwait(false);
            if (_pollingTokenSource != null)
            {
                _pollingTokenSource.Cancel();
            }
            _pollingTokenSource = new CancellationTokenSource();
            StartPolling(_pollingTokenSource.Token);
        }

        public override Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _pollingTokenSource.Cancel();
            if (PingTokenSource != null)
            {
                PingTokenSource.Cancel();
            }
            return Task.CompletedTask;
        }

        public override void AddHeader(string key, string val)
        {
            _http.DefaultRequestHeaders.Add(key, val);
        }

        public override void Dispose()
        {
            base.Dispose();
            _httpPollingHandler.Dispose();
        }

        public override async Task SendAsync(Payload payload, CancellationToken cancellationToken)
        {
            await _httpPollingHandler.PostAsync(_httpUri, payload.Text, cancellationToken);
            if (payload.Bytes != null && payload.Bytes.Count > 0)
            {
                await _httpPollingHandler.PostAsync(_httpUri, payload.Bytes, cancellationToken);
            }
        }

        protected override async Task OpenAsync(OpenedMessage msg)
        {
            //if (!_httpUri.Contains("&sid="))
            //{
            //}
            _httpUri += "&sid=" + msg.Sid;
            await base.OpenAsync(msg);
        }
    }
}
