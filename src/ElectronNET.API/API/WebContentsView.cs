using ElectronNET.API.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// A WebContentsView can be used to embed additional web content into a BrowserWindow.
    /// It is the successor of <see cref="BrowserView"/>. Add it to a window via
    /// <see cref="BrowserWindow.AddChildView(WebContentsView, int?)"/>.
    /// </summary>
    /// <remarks>
    /// Up-to-date with Electron API 41.
    /// <para>
    /// Removing a view from a window (<see cref="BrowserWindow.RemoveChildView(WebContentsView)"/>) only
    /// detaches it, so it can be added again later. Call <see cref="DestroyAsync"/> (or <see cref="Dispose"/>)
    /// once the view is no longer needed to close its web contents and release the renderer resources.
    /// </para>
    /// </remarks>
    public class WebContentsView : ApiBase, IDisposable
    {
        private static readonly TimeSpan GetBoundsTimeout = TimeSpan.FromSeconds(1);

        private int destroyed;

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
        /// <remarks>
        /// Every call is correlated with its own response, so concurrent requests for different
        /// views (or for the same view) never share a result.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The view was destroyed.</exception>
        public async Task<Rectangle> GetBoundsAsync()
        {
            if (this.IsDestroyed)
            {
                throw new ObjectDisposedException(nameof(WebContentsView));
            }

            var tcs = new TaskCompletionSource<Rectangle>(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestId = Guid.NewGuid().ToString("N");
            var eventName = "webContentsView-getBounds-completed" + requestId;

            BridgeConnector.Socket.Once<Rectangle>(eventName, bounds => tcs.TrySetResult(bounds));

            try
            {
                await BridgeConnector.Socket.Emit("webContentsView-getBounds", Id, requestId).ConfigureAwait(false);

                using var timeout = new CancellationTokenSource(GetBoundsTimeout);
                using (timeout.Token.Register(() => tcs.TrySetException(new TimeoutException(
                    $"No response after {(long)GetBoundsTimeout.TotalMilliseconds}ms trying to retrieve value webContentsView.GetBoundsAsync()"))))
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                // Make sure a request that timed out (or failed to be sent) does not leave a listener behind.
                if (!tcs.Task.IsCompletedSuccessfully)
                {
                    BridgeConnector.Socket.Off(eventName);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="DestroyAsync"/> (or <see cref="Dispose"/>) was called for this view.
        /// </summary>
        public bool IsDestroyed => Volatile.Read(ref this.destroyed) != 0;

        /// <summary>
        /// Destroys the view: detaches it from any window it is attached to, closes its
        /// <see cref="WebContents"/> and releases it on both the .NET and the Electron side.
        /// </summary>
        /// <remarks>
        /// To only detach a view and keep it for later reuse, use
        /// <see cref="BrowserWindow.RemoveChildView(WebContentsView)"/> instead.
        /// Calling this method more than once has no effect.
        /// </remarks>
        public async Task DestroyAsync()
        {
            if (Interlocked.Exchange(ref this.destroyed, 1) != 0)
            {
                return;
            }

            WindowManager.Instance.RemoveWebContentsView(this);

            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestId = Guid.NewGuid().ToString("N");
            var eventName = "webContentsView-destroy-completed" + requestId;

            BridgeConnector.Socket.Once(eventName, () => tcs.TrySetResult());
            await BridgeConnector.Socket.Emit("webContentsView-destroy", Id, requestId).ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Destroys the view without waiting for Electron to confirm it. See <see cref="DestroyAsync"/>.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.destroyed, 1) != 0)
            {
                return;
            }

            WindowManager.Instance.RemoveWebContentsView(this);
            _ = BridgeConnector.Socket.Emit("webContentsView-destroy", Id, null);
            GC.SuppressFinalize(this);
        }

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
