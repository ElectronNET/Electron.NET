using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Manage Browser Windows and Views
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Quit when all windows are closed. (Default is true)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [quit window all closed]; otherwise, <c>false</c>.
        /// </value>
        bool IsQuitOnWindowAllClosed { get; set; }

        /// <summary>
        /// Gets the browser windows.
        /// </summary>
        /// <value>
        /// The browser windows.
        /// </value>
        IReadOnlyCollection<BrowserWindow> BrowserWindows { get; }

        /// <summary>
        /// Gets the browser views.
        /// </summary>
        /// <value>
        /// The browser view.
        /// </value>
        IReadOnlyCollection<BrowserView> BrowserViews { get; }

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost");

        /// <summary>
        /// Creates the window asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="loadUrl">The load URL.</param>
        /// <returns></returns>
        Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost");

        /// <summary>
        /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
        /// It is like a child window, except that it is positioned relative to its owning window. 
        /// It is meant to be an alternative to the webview tag.
        /// </summary>
        /// <returns></returns>
        Task<BrowserView> CreateBrowserViewAsync();

        /// <summary>
        /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
        /// It is like a child window, except that it is positioned relative to its owning window. 
        /// It is meant to be an alternative to the webview tag.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<BrowserView> CreateBrowserViewAsync(BrowserViewConstructorOptions options);
    }
}