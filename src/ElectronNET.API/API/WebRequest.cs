using Newtonsoft.Json.Linq;
using System;

namespace ElectronNET.API.Entities
{
    public class OnBeforeRequestDetails
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public int? WebContentsId { get; set; }
        // Ensure all necessary properties are included as per Electron documentation
    }

    public class WebRequestFilter
    {
        public string[] Urls { get; set; }
    }

    public class WebRequest
    {
        public int Id { get; private set; }

        internal WebRequest(int id)
        {
            Id = id;
        }

        private event Action<OnBeforeRequestDetails, Action<object>> _onBeforeRequest;

        public void OnBeforeRequest(WebRequestFilter filter, Action<OnBeforeRequestDetails, Action<object>> listener)
        {
            if (_onBeforeRequest == null)
            {
                BridgeConnector.Socket.On($"webContents-session-webRequest-onBeforeRequest{Id}",
                    (args) =>
                        {
                            ////var details = ((JObject)args[0]).ToObject<OnBeforeRequestDetails>();
                            ////var callback = args.Length > 1 ? (Action<object>)((response) => { BridgeConnector.Socket.Emit($"webContents-session-webRequest-onBeforeRequest-response{Id}", response); }) : null;
                            var details = ((JObject)args).ToObject<OnBeforeRequestDetails>();
                            var callback =  (Action<object>)((response) => { BridgeConnector.Socket.Emit($"webContents-session-webRequest-onBeforeRequest-response{Id}", response); });

                            _onBeforeRequest?.Invoke(details, callback);
                        });

                BridgeConnector.Socket.Emit("register-webContents-session-webRequest-onBeforeRequest", Id, JObject.FromObject(filter));
            }

            _onBeforeRequest += listener;
        }

        public void RemoveListener(Action<OnBeforeRequestDetails, Action<object>> listener)
        {
            _onBeforeRequest -= listener;
            if (_onBeforeRequest == null)
            {
                BridgeConnector.Socket.Off($"webContents-session-webRequest-onBeforeRequest{Id}");
            }
        }
    }
}