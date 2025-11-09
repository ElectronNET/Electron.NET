using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using System.Text.Json;
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
        public int Id { get; internal set; }

        /// <summary>
        /// Render and control web pages.
        /// </summary>
        public WebContents WebContents { get; internal set; }

        /// <summary>
        /// Resizes and moves the view to the supplied bounds relative to the window.
        /// (experimental)
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<Rectangle>();

                Task.Run(() =>
                {
                    BridgeConnector.Socket.On<JsonElement>("browserView-getBounds-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("browserView-getBounds-reply");
                        taskCompletionSource.SetResult(result.Deserialize(ElectronJsonContext.Default.Rectangle));
                    });

                    BridgeConnector.Socket.Emit("browserView-getBounds", Id);
                });

                return taskCompletionSource.Task.GetAwaiter().GetResult();
            }
            set
            {
                BridgeConnector.Socket.Emit("browserView-setBounds", Id, value);
            }
        }

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
        /// (experimental)
        /// </summary>
        /// <param name="options"></param>
        public void SetAutoResize(AutoResizeOptions options)
        {
            BridgeConnector.Socket.Emit("browserView-setAutoResize", Id, options);
        }

        /// <summary>
        /// Color in #aarrggbb or #argb form. The alpha channel is optional.
        /// (experimental)
        /// </summary>
        /// <param name="color">Color in #aarrggbb or #argb form. The alpha channel is optional.</param>
        public void SetBackgroundColor(string color)
        {
            BridgeConnector.Socket.Emit("browserView-setBackgroundColor", Id, color);
        }
    }
}

