using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
    /// It is like a child window, except that it is positioned relative to its owning window. 
    /// It is meant to be an alternative to the webview tag.
    /// </summary>
    public class BrowserView
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; internal set; }

        /// <summary>
        /// Render and control web pages.
        /// </summary>
        public WebContents WebContents { get; internal set; }

        /// <summary>
        /// Whether the view is destroyed.
        /// </summary>
        public Task<bool> IsDestroyedAsync
        {
            get
            {
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("browserView-isDestroyed-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("browserView-isDestroyed-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("browserView-isDestroyed", Id);

                    return taskCompletionSource.Task;
                });
            }
        }

        /// <summary>
        /// Resizes and moves the view to the supplied bounds relative to the window.
        /// 
        /// (experimental)
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return Task.Run<Rectangle>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<Rectangle>();

                    BridgeConnector.Socket.On("browserView-getBounds-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("browserView-getBounds-reply");
                        taskCompletionSource.SetResult((Rectangle)result);
                    });

                    BridgeConnector.Socket.Emit("browserView-getBounds", Id);

                    return taskCompletionSource.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("browserView-setBounds", Id, JObject.FromObject(value, _jsonSerializer));
            }
        }

        internal Action<BrowserView> Destroyed;

        /// <summary>
        /// BrowserView
        /// </summary>
        internal BrowserView(int id) 
        {
            Id = id;

            // Workaround: increase the Id so as not to conflict with BrowserWindow id
            // the backend detect about the value an BrowserView
            WebContents = new WebContents(id + 1000);
        }

        /// <summary>
        /// Force closing the view, the `unload` and `beforeunload` events won't be emitted
        /// for the web page.After you're done with a view, call this function in order to
        /// free memory and other resources as soon as possible.
        /// </summary>
        public void Destroy()
        {
            BridgeConnector.Socket.Emit("browserView-destroy", Id);

            Destroyed?.Invoke(this);
        }

        /// <summary>
        /// (experimental)
        /// </summary>
        /// <param name="options"></param>
        public void SetAutoResize(AutoResizeOptions options)
        {
            BridgeConnector.Socket.Emit("browserView-setAutoResize", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Color in #aarrggbb or #argb form. The alpha channel is optional.
        /// 
        /// (experimental)
        /// </summary>
        /// <param name="color">Color in #aarrggbb or #argb form. The alpha channel is optional.</param>
        public void SetBackgroundColor(string color)
        {
            BridgeConnector.Socket.Emit("browserView-setBackgroundColor", Id, color);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
