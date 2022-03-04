using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ElectronNET.API
{
    public sealed class DesktopCapturer
    {
        private static readonly object _syncRoot = new();
        private static DesktopCapturer _desktopCapturer;

        internal DesktopCapturer() { }

        internal static DesktopCapturer Instance
        {
            get
            {
                if (_desktopCapturer == null)
                {
                    lock (_syncRoot)
                    {
                        if (_desktopCapturer == null)
                        {
                            _desktopCapturer = new DesktopCapturer();
                        }
                    }
                }

                return _desktopCapturer;
            }
        }

        public async Task<DesktopCapturerSource[]> GetSourcesAsync(SourcesOption option) 
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("desktop-capturer-get-sources", JObject.FromObject(option, _jsonSerializer));
            return ((JObject)signalrResult).ToObject<DesktopCapturerSource[]>();
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}