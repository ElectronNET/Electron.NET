using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;

namespace ElectronNET.API
{
    /// <summary>
    /// Render and control web pages.
    /// </summary>
    public class WebContents
    {
        public int Id { get; private set; }

        /// <summary>
        /// Emitted when the renderer process crashes or is killed.
        /// </summary>
        public event Action<bool> OnCrashed
        {
            add
            {
                if (_crashed == null)
                {
                    BridgeConnector.Socket.On("webContents-crashed" + Id, (killed) =>
                    {
                        _crashed((bool)killed);
                    });

                    BridgeConnector.Socket.Emit("register-webContents-crashed", Id);
                }
                _crashed += value;
            }
            remove
            {
                _crashed -= value;
            }
        }

        private event Action<bool> _crashed;

        /// <summary>
        /// Emitted when the navigation is done, i.e. the spinner of the tab has
        /// stopped spinning, and the onload event was dispatched.
        /// </summary>
        public event Action OnDidFinishLoad
        {
            add
            {
                if (_didFinishLoad == null)
                {
                    BridgeConnector.Socket.On("webContents-didFinishLoad" + Id, () =>
                    {
                        _didFinishLoad();
                    });

                    BridgeConnector.Socket.Emit("register-webContents-didFinishLoad", Id);
                }
                _didFinishLoad += value;
            }
            remove
            {
                _didFinishLoad -= value;
            }
        }

        private event Action _didFinishLoad;

        internal WebContents(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Opens the devtools.
        /// </summary>
        public void OpenDevTools()
        {
            BridgeConnector.Socket.Emit("webContentsOpenDevTools", Id);
        }

        /// <summary>
        /// Opens the devtools.
        /// </summary>
        /// <param name="openDevToolsOptions"></param>
        public void OpenDevTools(OpenDevToolsOptions openDevToolsOptions)
        {
            BridgeConnector.Socket.Emit("webContentsOpenDevTools", Id, JObject.FromObject(openDevToolsOptions, _jsonSerializer));
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}