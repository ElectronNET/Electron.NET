﻿using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ElectronNET.API;

/// <summary>
/// 
/// </summary>
public sealed class WindowManager
{
    private static WindowManager _windowManager;
    private static readonly object SyncRoot = new();

    internal WindowManager() { }

    internal static WindowManager Instance
    {
        get
        {
            if (_windowManager == null)
            {
                lock (SyncRoot)
                {
                    _windowManager ??= new WindowManager();
                }
            }

            return _windowManager;
        }
    }

    /// <summary>
    /// Quit when all windows are closed. (Default is true)
    /// </summary>
    /// <value>
    ///   <c>true</c> if [quit window all closed]; otherwise, <c>false</c>.
    /// </value>
    public bool IsQuitOnWindowAllClosed
    {
        get => _isQuitOnWindowAllClosed;
        set
        {
            BridgeConnector.Socket.Emit("quit-app-window-all-closed-event", value);
            _isQuitOnWindowAllClosed = value;
        }
    }
    private bool _isQuitOnWindowAllClosed = true;

    /// <summary>
    /// Gets the browser windows.
    /// </summary>
    /// <value>
    /// The browser windows.
    /// </value>
    public IReadOnlyCollection<BrowserWindow> BrowserWindows => _browserWindows.AsReadOnly();

    private readonly List<BrowserWindow> _browserWindows = new();

    /// <summary>
    /// Gets the browser views.
    /// </summary>
    /// <value>
    /// The browser view.
    /// </value>
    public IReadOnlyCollection<BrowserView> BrowserViews => _browserViews.AsReadOnly();

    private readonly List<BrowserView> _browserViews = new();

    /// <summary>
    /// Creates the window asynchronous.
    /// </summary>
    /// <param name="loadUrl">The load URL.</param>
    /// <returns></returns>
    public async Task<BrowserWindow> CreateWindowAsync(string loadUrl = "http://localhost")
    {
        return await CreateWindowAsync(new BrowserWindowOptions(), loadUrl);
    }

    /// <summary>
    /// Creates the window asynchronous.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="loadUrl">The load URL.</param>
    /// <returns></returns>
    public async Task<BrowserWindow> CreateWindowAsync(BrowserWindowOptions options, string loadUrl = "http://localhost")
    {
        var taskCompletionSource = new TaskCompletionSource<BrowserWindow>();

        BridgeConnector.Socket.On("BrowserWindowCreated", (id) =>
        {
            BridgeConnector.Socket.Off("BrowserWindowCreated");

            var browserWindowId = int.Parse(id.ToString()!);

            var browserWindow = new BrowserWindow(browserWindowId);
            _browserWindows.Add(browserWindow);

            taskCompletionSource.SetResult(browserWindow);
        });

        BridgeConnector.Socket.On<object>("BrowserWindowClosed", (ids) =>
        {
            BridgeConnector.Socket.Off("BrowserWindowClosed");

            var browserWindowIds = ((JArray)ids).ToObject<int[]>();

            for (int index = 0; index < _browserWindows.Count; index++)
            {
                if (!browserWindowIds.Contains(_browserWindows[index].Id))
                {
                    _browserWindows.RemoveAt(index);
                }
            }
        });

        if (loadUrl.ToUpper() == "HTTP://LOCALHOST")
        {
            loadUrl = $"{loadUrl}:{BridgeSettings.WebPort}";
        }

        // Workaround Windows 10 / Electron Bug
        // https://github.com/electron/electron/issues/4045
        if (IsWindows10())
        {
            options.Width += 14;
            options.Height += 7;
        }

        if (options.X == -1 && options.Y == -1)
        {
            options.X = 0;
            options.Y = 0;

            await BridgeConnector.Socket.Emit("createBrowserWindow", JObject.FromObject(options, _jsonSerializer), loadUrl);
        }
        else
        {
            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (IsWindows10())
            {
                options.X -= 7;
            }

            var ownjsonSerializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            await BridgeConnector.Socket.Emit("createBrowserWindow", JObject.FromObject(options, ownjsonSerializer), loadUrl);
        }

        return await taskCompletionSource.Task;
    }

    private bool IsWindows10()
    {
        return RuntimeInformation.OSDescription.Contains("Windows 10");
    }

    /// <summary>
    /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
    /// It is like a child window, except that it is positioned relative to its owning window. 
    /// It is meant to be an alternative to the webview tag.
    /// </summary>
    /// <returns></returns>
    public Task<BrowserView> CreateBrowserViewAsync()
    {
        return CreateBrowserViewAsync(new BrowserViewConstructorOptions());
    }

    /// <summary>
    /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
    /// It is like a child window, except that it is positioned relative to its owning window. 
    /// It is meant to be an alternative to the webview tag.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task<BrowserView> CreateBrowserViewAsync(BrowserViewConstructorOptions options)
    {
        var taskCompletionSource = new TaskCompletionSource<BrowserView>();

        BridgeConnector.Socket.On("BrowserViewCreated", (id) =>
        {
            BridgeConnector.Socket.Off("BrowserViewCreated");

            string browserViewId = id.ToString();
            BrowserView browserView = new BrowserView(int.Parse(browserViewId));

            _browserViews.Add(browserView);

            taskCompletionSource.SetResult(browserView);
        });

        var ownjsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        await BridgeConnector.Socket.Emit("createBrowserView", JObject.FromObject(options, ownjsonSerializer));

        return await taskCompletionSource.Task;
    }

    private readonly JsonSerializer _jsonSerializer = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };
}