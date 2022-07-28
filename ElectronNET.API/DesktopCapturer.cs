using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Newtonsoft.Json;

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
            return await BridgeConnector.OnResult<DesktopCapturerSource[]>("desktop-capturer-get-sources", "desktop-capturer-get-sources-result", option);
        }
    }
}