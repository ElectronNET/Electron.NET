using ElectronNET.API.Entities;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// A WebContentsView can be used to embed additional web content into a BrowserWindow.
    /// It is the successor of <see cref="BrowserView"/>. Add it to a window via
    /// <see cref="BrowserWindow.AddChildView(WebContentsView, int?)"/>.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 41</remarks>
    public class WebContentsView : ApiBase
    {
        /// <summary>
        /// Offset applied to the id of the associated <see cref="API.WebContents"/> so the
        /// host can distinguish it from BrowserWindow (id) and BrowserView (id + 1000) contents.
        /// </summary>
        internal const int WebContentsIdOffset = 1000000;

        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.DashesLowerFirst;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public override int Id { get; protected set; }

        /// <summary>
        /// Render and control web pages.
        /// </summary>
        public WebContents WebContents { get; internal set; }

        internal WebContentsView(int id)
        {
            Id = id;
            WebContents = new WebContents(id + WebContentsIdOffset);
        }

        /// <summary>
        /// The bounds of this view, relative to its parent.
        /// </summary>
        public Task<Rectangle> GetBoundsAsync() => this.InvokeAsync<Rectangle>();

        /// <summary>
        /// Resizes and moves the view to the supplied bounds relative to its parent.
        /// </summary>
        /// <param name="bounds">The new bounds.</param>
        public void SetBounds(Rectangle bounds)
        {
            BridgeConnector.Socket.Emit("webContentsView-setBounds", Id, bounds);
        }

        /// <summary>
        /// Sets the background color of the view.
        /// </summary>
        /// <param name="color">Color in Hex, RGB, ARGB, HSL, HSLA or named CSS color format.</param>
        public void SetBackgroundColor(string color)
        {
            BridgeConnector.Socket.Emit("webContentsView-setBackgroundColor", Id, color);
        }

        /// <summary>
        /// Shows or hides the view.
        /// </summary>
        /// <param name="visible">If false, the view will be hidden from display.</param>
        public void SetVisible(bool visible)
        {
            BridgeConnector.Socket.Emit("webContentsView-setVisible", Id, visible);
        }

        /// <summary>
        /// Sets the border radius of the view in pixels.
        /// Ignored on Electron versions that do not support it.
        /// </summary>
        /// <param name="radius">Border radius size in pixels.</param>
        public void SetBorderRadius(int radius)
        {
            BridgeConnector.Socket.Emit("webContentsView-setBorderRadius", Id, radius);
        }
    }
}
