using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API.Extensions;

namespace ElectronNET.API
{
    /// <summary>
    /// Control your application's event lifecycle.
    /// </summary>
    public sealed class App
    {
        /// <summary>
        /// Emitted when all windows have been closed.
        /// <para/>
        /// If you do not subscribe to this event and all windows are closed, the default behavior is to quit
        /// the app; however, if you subscribe, you control whether the app quits or not.If the user pressed
        /// Cmd + Q, or the developer called <see cref="Quit"/>, Electron will first try to close all the windows
        /// and then emit the <see cref="WillQuit"/> event, and in this case the <see cref="WindowAllClosed"/> event
        /// would not be emitted.
        /// </summary>
        public event Action WindowAllClosed
        {
            add
            {
                if (_windowAllClosed == null)
                {
                    BridgeConnector.Socket.On("app-window-all-closed" + GetHashCode(), () =>
                    {
                        if (!Electron.WindowManager.IsQuitOnWindowAllClosed || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            _windowAllClosed();
                        }
                    });

                    BridgeConnector.Socket.Emit("register-app-window-all-closed-event", GetHashCode());
                }
                _windowAllClosed += value;
            }
            remove
            {
                _windowAllClosed -= value;

                if(_windowAllClosed == null)
                    BridgeConnector.Socket.Off("app-window-all-closed" + GetHashCode());
            }
        }

        private event Action _windowAllClosed;

        /// <summary>
        /// Emitted before the application starts closing its windows. 
        /// <para/>
        /// Note: If application quit was initiated by <see cref="AutoUpdater.QuitAndInstall"/> then <see cref="BeforeQuit"/>
        /// is emitted after emitting close event on all windows and closing them.
        /// <para/>
        /// Note: On Windows, this event will not be emitted if the app is closed due to a shutdown/restart of the system or a user logout.
        /// </summary>
        public event Func<QuitEventArgs, Task> BeforeQuit
        {
            add
            {
                if (_beforeQuit == null)
                {
                    BridgeConnector.Socket.On("app-before-quit" + GetHashCode(), async () =>
                    {
                        await _beforeQuit(new QuitEventArgs());

                        if (_preventQuit)
                        {
                            _preventQuit = false;
                        }
                        else
                        {
                            if (_willQuit == null && _quitting == null)
                            {
                                Exit();
                            }
                            else if (_willQuit != null)
                            {
                                await _willQuit(new QuitEventArgs());

                                if (_preventQuit)
                                {
                                    _preventQuit = false;
                                }
                                else
                                {
                                    if (_quitting == null)
                                    {
                                        Exit();
                                    }
                                    else
                                    {
                                        await _quitting();
                                        Exit();
                                    }
                                }
                            }
                            else if (_quitting != null)
                            {
                                await _quitting();
                                Exit();
                            }
                        }
                    });

                    BridgeConnector.Socket.Emit("register-app-before-quit-event", GetHashCode());
                }
                _beforeQuit += value;
            }
            remove
            {
                _beforeQuit -= value;

                if (_beforeQuit == null)
                    BridgeConnector.Socket.Off("app-before-quit" + GetHashCode());
            }
        }

        private event Func<QuitEventArgs, Task> _beforeQuit;

        /// <summary>
        /// Emitted when all windows have been closed and the application will quit.
        /// <para/>
        /// See the description of the <see cref="WindowAllClosed"/> event for the differences between the <see cref="WillQuit"/>
        /// and <see cref="WindowAllClosed"/> events.
        /// <para/>
        /// Note: On Windows, this event will not be emitted if the app is closed due to a shutdown/restart of the system or a user logout.
        /// </summary>
        public event Func<QuitEventArgs, Task> WillQuit
        {
            add
            {
                if (_willQuit == null)
                {
                    BridgeConnector.Socket.On("app-will-quit" + GetHashCode(), async () =>
                    {
                        await _willQuit(new QuitEventArgs());

                        if (_preventQuit)
                        {
                            _preventQuit = false;
                        }
                        else
                        {
                            if (_quitting == null)
                            {
                                Exit();
                            }
                            else
                            {
                                await _quitting();
                                Exit();
                            }
                        }
                    });

                    BridgeConnector.Socket.Emit("register-app-will-quit-event", GetHashCode());
                }
                _willQuit += value;
            }
            remove
            {
                _willQuit -= value;

                if (_willQuit == null)
                    BridgeConnector.Socket.Off("app-will-quit" + GetHashCode());
            }
        }

        private event Func<QuitEventArgs, Task> _willQuit;

        /// <summary>
        /// Emitted when the application is quitting.
        /// <para/>
        /// Note: On Windows, this event will not be emitted if the app is closed due to a shutdown/restart of the system or a user logout.
        /// </summary>
        public event Func<Task> Quitting
        {
            add
            {
                if (_quitting == null)
                {
                    BridgeConnector.Socket.On("app-will-quit" + GetHashCode() + "quitting", async () =>
                    {
                        if(_willQuit == null)
                        {
                            await _quitting();
                            Exit();
                        }
                    });

                    BridgeConnector.Socket.Emit("register-app-will-quit-event", GetHashCode() + "quitting");
                }
                _quitting += value;
            }
            remove
            {
                _quitting -= value;

                if (_quitting == null)
                    BridgeConnector.Socket.Off("app-will-quit" + GetHashCode() + "quitting");
            }
        }

        private event Func<Task> _quitting;

        /// <summary>
        /// Emitted when a <see cref="BrowserWindow"/> blurred.
        /// </summary>
        public event Action BrowserWindowBlur
        {
            add
            {
                if (_browserWindowBlur == null)
                {
                    BridgeConnector.Socket.On("app-browser-window-blur" + GetHashCode(), () =>
                    {
                        _browserWindowBlur();
                    });

                    BridgeConnector.Socket.Emit("register-app-browser-window-blur-event", GetHashCode());
                }
                _browserWindowBlur += value;
            }
            remove
            {
                _browserWindowBlur -= value;

                if (_browserWindowBlur == null)
                    BridgeConnector.Socket.Off("app-browser-window-blur" + GetHashCode());
            }
        }

        private event Action _browserWindowBlur;

        /// <summary>
        /// Emitted when a <see cref="BrowserWindow"/> gets focused.
        /// </summary>
        public event Action BrowserWindowFocus
        {
            add
            {
                if (_browserWindowFocus == null)
                {
                    BridgeConnector.Socket.On("app-browser-window-focus" + GetHashCode(), () =>
                    {
                        _browserWindowFocus();
                    });

                    BridgeConnector.Socket.Emit("register-app-browser-window-focus-event", GetHashCode());
                }
                _browserWindowFocus += value;
            }
            remove
            {
                _browserWindowFocus -= value;

                if (_browserWindowFocus == null)
                    BridgeConnector.Socket.Off("app-browser-window-focus" + GetHashCode());
            }
        }

        private event Action _browserWindowFocus;

        /// <summary>
        /// Emitted when a new <see cref="BrowserWindow"/> is created.
        /// </summary>
        public event Action BrowserWindowCreated
        {
            add
            {
                if (_browserWindowCreated == null)
                {
                    BridgeConnector.Socket.On("app-browser-window-created" + GetHashCode(), () =>
                    {
                        _browserWindowCreated();
                    });

                    BridgeConnector.Socket.Emit("register-app-browser-window-created-event", GetHashCode());
                }
                _browserWindowCreated += value;
            }
            remove
            {
                _browserWindowCreated -= value;

                if (_browserWindowCreated == null)
                    BridgeConnector.Socket.Off("app-browser-window-created" + GetHashCode());
            }
        }

        private event Action _browserWindowCreated;

        /// <summary>
        /// Emitted when a new <see cref="WebContents"/> is created.
        /// </summary>
        public event Action WebContentsCreated
        {
            add
            {
                if (_webContentsCreated == null)
                {
                    BridgeConnector.Socket.On("app-web-contents-created" + GetHashCode(), () =>
                    {
                        _webContentsCreated();
                    });

                    BridgeConnector.Socket.Emit("register-app-web-contents-created-event", GetHashCode());
                }
                _webContentsCreated += value;
            }
            remove
            {
                _webContentsCreated -= value;

                if (_webContentsCreated == null)
                    BridgeConnector.Socket.Off("app-web-contents-created" + GetHashCode());
            }
        }

        private event Action _webContentsCreated;

        /// <summary>
        /// Emitted when Chrome’s accessibility support changes. This event fires when assistive technologies, such as
        /// screen readers, are enabled or disabled. See https://www.chromium.org/developers/design-documents/accessibility for more details.
        /// </summary>
        /// <returns><see langword="true"/> when Chrome's accessibility support is enabled, <see langword="false"/> otherwise.</returns>
        public event Action<bool> AccessibilitySupportChanged
        {
            add
            {
                if (_accessibilitySupportChanged == null)
                {
                    BridgeConnector.Socket.On("app-accessibility-support-changed" + GetHashCode(), (state) =>
                    {
                        _accessibilitySupportChanged((bool)state);
                    });

                    BridgeConnector.Socket.Emit("register-app-accessibility-support-changed-event", GetHashCode());
                }
                _accessibilitySupportChanged += value;
            }
            remove
            {
                _accessibilitySupportChanged -= value;

                if (_accessibilitySupportChanged == null)
                    BridgeConnector.Socket.Off("app-accessibility-support-changed" + GetHashCode());
            }
        }

        private event Action<bool> _accessibilitySupportChanged;

        /// <summary>
        /// Emitted when the application has finished basic startup.
        /// </summary>
        public event Action Ready 
        {
            add
            {
                if(IsReady)
                {
                    value();
                }

                _ready += value;
            }
            remove
            {
                _ready -= value;
            }
        }

        private event Action _ready;

        /// <summary>
        /// Application host fully started.
        /// </summary>
        public bool IsReady 
        { 
            get { return _isReady; }
            internal set
            {
                _isReady = value;

                if(value)
                {
                    _ready?.Invoke();
                }
            }
        }
        private bool _isReady = false;

        /// <summary>
        /// A <see cref="string"/> property that indicates the current application's name, which is the name in the
        /// application's package.json file.
        ///
        /// Usually the name field of package.json is a short lowercase name, according to the npm modules spec. You
        /// should usually also specify a productName field, which is your application's full capitalized name, and
        /// which will be preferred over name by Electron.
        /// </summary>
        public string Name
        {
            get
            {
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On("appGetNameCompleted", (result) =>
                    {
                        BridgeConnector.Socket.Off("appGetNameCompleted");
                        taskCompletionSource.SetResult((string)result);
                    });

                    BridgeConnector.Socket.Emit("appGetName");

                    return taskCompletionSource.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("appSetName", value);
            }
        }

        internal App() 
        {
            CommandLine = new CommandLine();
        }

        internal static App Instance
        {
            get
            {
                if (_app == null)
                {
                    lock (_syncRoot)
                    {
                        if(_app == null)
                        {
                            _app = new App();
                        }
                    }
                }

                return _app;
            }
        }

        private static App _app;
        private static object _syncRoot = new object();

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Try to close all windows. The <see cref="BeforeQuit"/> event will be emitted first. If all windows are successfully
        /// closed, the <see cref="WillQuit"/> event will be emitted and by default the application will terminate. This method
        /// guarantees that all beforeunload and unload event handlers are correctly executed. It is possible
        /// that a window cancels the quitting by returning <see langword="false"/> in the beforeunload event handler.
        /// </summary>
        public void Quit()
        {
            BridgeConnector.Socket.Emit("appQuit");
        }

        /// <summary>
        /// All windows will be closed immediately without asking user and the <see cref="BeforeQuit"/> and <see cref="WillQuit"/>
        /// events will not be emitted.
        /// </summary>
        /// <param name="exitCode">Exits immediately with exitCode. exitCode defaults to 0.</param>
        public void Exit(int exitCode = 0)
        {
            BridgeConnector.Socket.Emit("appExit", exitCode);
        }

        /// <summary>
        /// Relaunches the app when current instance exits. By default the new instance will use the same working directory
        /// and command line arguments with current instance.
        /// <para/>
        /// Note that this method does not quit the app when executed, you have to call <see cref="Quit"/> or <see cref="Exit"/>
        /// after calling <see cref="Relaunch()"/> to make the app restart.
        /// <para/>
        /// When <see cref="Relaunch()"/> is called for multiple times, multiple instances will be started after current instance
        /// exited.
        /// </summary>
        public void Relaunch()
        {
            BridgeConnector.Socket.Emit("appRelaunch");
        }

        /// <summary>
        /// Relaunches the app when current instance exits. By default the new instance will use the same working directory
        /// and command line arguments with current instance. When <see cref="RelaunchOptions.Args"/> is specified, the
        /// <see cref="RelaunchOptions.Args"/> will be passed as command line arguments instead. When <see cref="RelaunchOptions.ExecPath"/>
        /// is specified, the <see cref="RelaunchOptions.ExecPath"/> will be executed for relaunch instead of current app.
        /// <para/>
        /// Note that this method does not quit the app when executed, you have to call <see cref="Quit"/> or <see cref="Exit"/>
        /// after calling <see cref="Relaunch()"/> to make the app restart.
        /// <para/>
        /// When <see cref="Relaunch()"/> is called for multiple times, multiple instances will be started after current instance
        /// exited.
        /// </summary>
        /// <param name="relaunchOptions">Options for the relaunch.</param>
        public void Relaunch(RelaunchOptions relaunchOptions)
        {
            BridgeConnector.Socket.Emit("appRelaunch", JObject.FromObject(relaunchOptions, _jsonSerializer));
        }

        /// <summary>
        /// On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses
        /// on the application's first window.
        /// </summary>
        public void Focus()
        {
            BridgeConnector.Socket.Emit("appFocus");
        }

        /// <summary>
        /// On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses
        /// on the application's first window.
        /// <para/>
        /// You should seek to use the <see cref="FocusOptions.Steal"/> option as sparingly as possible.
        /// </summary>
        public void Focus(FocusOptions focusOptions)
        {
            BridgeConnector.Socket.Emit("appFocus", JObject.FromObject(focusOptions, _jsonSerializer));
        }

        /// <summary>
        /// Hides all application windows without minimizing them.
        /// </summary>
        public void Hide()
        {
            BridgeConnector.Socket.Emit("appHide");
        }

        /// <summary>
        /// Shows application windows after they were hidden. Does not automatically focus them.
        /// </summary>
        public void Show()
        {
            BridgeConnector.Socket.Emit("appShow");
        }

        /// <summary>
        /// The current application directory.
        /// </summary>
        public async Task<string> GetAppPathAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using(cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetAppPathCompleted", (path) =>
                {
                    BridgeConnector.Socket.Off("appGetAppPathCompleted");
                    taskCompletionSource.SetResult(path.ToString());
                });

                BridgeConnector.Socket.Emit("appGetAppPath");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }            
        }

        /// <summary>
        /// Sets or creates a directory your app's logs which can then be manipulated with <see cref="GetPathAsync"/>
        /// or <see cref="SetPath"/>.
        /// <para/>
        /// Calling <see cref="SetAppLogsPath"/> without a path parameter will result in this directory being set to
        /// ~/Library/Logs/YourAppName on macOS, and inside the userData directory on Linux and Windows.
        /// </summary>
        /// <param name="path">A custom path for your logs. Must be absolute.</param>
        public void SetAppLogsPath(string path)
        {
            BridgeConnector.Socket.Emit("appSetAppLogsPath", path);
        }

        /// <summary>
        /// The path to a special directory. If <see cref="GetPathAsync"/> is called without called
        /// <see cref="SetAppLogsPath"/> being called first, a default directory will be created equivalent
        /// to calling <see cref="SetAppLogsPath"/> without a path parameter.
        /// </summary>
        /// <param name="pathName">Special directory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A path to a special directory or file associated with name.</returns>
        public async Task<string> GetPathAsync(PathName pathName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using(cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetPathCompleted", (path) =>
                {
                    BridgeConnector.Socket.Off("appGetPathCompleted");

                    taskCompletionSource.SetResult(path.ToString());
                });

                BridgeConnector.Socket.Emit("appGetPath", pathName.GetDescription());

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }  
        }

        /// <summary>
        /// Overrides the path to a special directory or file associated with name. If the path specifies a directory
        /// that does not exist, an Error is thrown. In that case, the directory should be created with fs.mkdirSync or similar.
        /// <para/>
        /// You can only override paths of a name defined in <see cref="GetPathAsync"/>.
        /// <para/>
        /// By default, web pages' cookies and caches will be stored under the <see cref="PathName.UserData"/> directory. If you
        /// want to change this location, you have to override the <see cref="PathName.UserData"/> path before the <see cref="Ready"/>
        /// event of the <see cref="App"/> module is emitted.
        /// <param name="name">Special directory.</param>
        /// <param name="path">New path to a special directory.</param>
        /// </summary>
        public void SetPath(PathName name, string path)
        {
            BridgeConnector.Socket.Emit("appSetPath", name.GetDescription(), path);
        }

        /// <summary>
        /// The version of the loaded application. If no version is found in the application’s package.json file, 
        /// the version of the current bundle or executable is returned.
        /// </summary>
        /// <returns>The version of the loaded application.</returns>
        public async Task<string> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using(cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetVersionCompleted", (version) =>
                {
                    BridgeConnector.Socket.Off("appGetVersionCompleted");
                    taskCompletionSource.SetResult(version.ToString());
                });

                BridgeConnector.Socket.Emit("appGetVersion");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The current application locale. Possible return values are documented <see href="https://www.electronjs.org/docs/api/locales">here</see>.
        /// <para/>
        /// Note: When distributing your packaged app, you have to also ship the locales folder.
        /// <para/>
        /// Note: On Windows, you have to call it after the <see cref="Ready"/> events gets emitted.
        /// </summary>
        /// <returns>The current application locale.</returns>
        public async Task<string> GetLocaleAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetLocaleCompleted", (local) =>
                {
                    BridgeConnector.Socket.Off("appGetLocaleCompleted");
                    taskCompletionSource.SetResult(local.ToString());
                });

                BridgeConnector.Socket.Emit("appGetLocale");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds path to the recent documents list. This list is managed by the OS. On Windows you can visit the
        /// list from the task bar, and on macOS you can visit it from dock menu.
        /// </summary>
        /// <param name="path">Path to add.</param>
        public void AddRecentDocument(string path)
        {
            BridgeConnector.Socket.Emit("appAddRecentDocument", path);
        }

        /// <summary>
        /// Clears the recent documents list.
        /// </summary>
        public void ClearRecentDocuments()
        {
            BridgeConnector.Socket.Emit("appClearRecentDocuments");
        }

        /// <summary>
        /// Sets the current executable as the default handler for a protocol (aka URI scheme). It allows you to
        /// integrate your app deeper into the operating system. Once registered, all links with your-protocol://
        /// will be opened with the current executable. The whole link, including protocol, will be passed to your
        /// application as a parameter.
        /// <para/>
        /// Note: On macOS, you can only register protocols that have been added to your app's info.plist, which
        /// cannot be modified at runtime. However, you can change the file during build time via
        /// <see href="https://www.electronforge.io/">Electron Forge</see>,
        /// <see href="https://github.com/electron/electron-packager">Electron Packager</see>, or by editing info.plist
        /// with a text editor. Please refer to
        /// <see href="https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-102207-TPXREF115">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// Note: In a Windows Store environment (when packaged as an appx) this API will return true for all calls but
        /// the registry key it sets won't be accessible by other applications. In order to register your Windows Store
        /// application as a default protocol handler you <see href="https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-uap-protocol">must declare the protocol in your manifest</see>.
        /// <para/>
        /// The API uses the Windows Registry and LSSetDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">
        /// The name of your protocol, without ://. For example, if you want your app to handle electron:// links,
        /// call this method with electron as the parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> SetAsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)
        {
            return await SetAsDefaultProtocolClientAsync(protocol, null, null, cancellationToken);
        }

        /// <summary>
        /// Sets the current executable as the default handler for a protocol (aka URI scheme). It allows you to
        /// integrate your app deeper into the operating system. Once registered, all links with your-protocol://
        /// will be opened with the current executable. The whole link, including protocol, will be passed to your
        /// application as a parameter.
        /// <para/>
        /// Note: On macOS, you can only register protocols that have been added to your app's info.plist, which
        /// cannot be modified at runtime. However, you can change the file during build time via
        /// <see href="https://www.electronforge.io/">Electron Forge</see>,
        /// <see href="https://github.com/electron/electron-packager">Electron Packager</see>, or by editing info.plist
        /// with a text editor. Please refer to
        /// <see href="https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-102207-TPXREF115">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// Note: In a Windows Store environment (when packaged as an appx) this API will return true for all calls but
        /// the registry key it sets won't be accessible by other applications. In order to register your Windows Store
        /// application as a default protocol handler you <see href="https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-uap-protocol">must declare the protocol in your manifest</see>.
        /// <para/>
        /// The API uses the Windows Registry and LSSetDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">
        /// The name of your protocol, without ://. For example, if you want your app to handle electron:// links,
        /// call this method with electron as the parameter.</param>
        /// <param name="path">The path to the Electron executable. Defaults to process.execPath</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> SetAsDefaultProtocolClientAsync(string protocol, string path, CancellationToken cancellationToken = default)
        {
            return await SetAsDefaultProtocolClientAsync(protocol, path, null, cancellationToken);
        }

        /// <summary>
        /// Sets the current executable as the default handler for a protocol (aka URI scheme). It allows you to
        /// integrate your app deeper into the operating system. Once registered, all links with your-protocol://
        /// will be opened with the current executable. The whole link, including protocol, will be passed to your
        /// application as a parameter.
        /// <para/>
        /// Note: On macOS, you can only register protocols that have been added to your app's info.plist, which
        /// cannot be modified at runtime. However, you can change the file during build time via
        /// <see href="https://www.electronforge.io/">Electron Forge</see>,
        /// <see href="https://github.com/electron/electron-packager">Electron Packager</see>, or by editing info.plist
        /// with a text editor. Please refer to
        /// <see href="https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-102207-TPXREF115">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// Note: In a Windows Store environment (when packaged as an appx) this API will return true for all calls but
        /// the registry key it sets won't be accessible by other applications. In order to register your Windows Store
        /// application as a default protocol handler you <see href="https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-uap-protocol">must declare the protocol in your manifest</see>.
        /// <para/>
        /// The API uses the Windows Registry and LSSetDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">
        /// The name of your protocol, without ://. For example, if you want your app to handle electron:// links,
        /// call this method with electron as the parameter.</param>
        /// <param name="path">The path to the Electron executable. Defaults to process.execPath</param>
        /// <param name="args">Arguments passed to the executable. Defaults to an empty array.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> SetAsDefaultProtocolClientAsync(string protocol, string path, string[] args, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appSetAsDefaultProtocolClientCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appSetAsDefaultProtocolClientCompleted");
                    taskCompletionSource.SetResult((bool) success);
                });

                BridgeConnector.Socket.Emit("appSetAsDefaultProtocolClient", protocol, path, args);

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This method checks if the current executable as the default handler for a protocol (aka URI scheme).
        /// If so, it will remove the app as the default handler.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)
        {
            return await RemoveAsDefaultProtocolClientAsync(protocol, null, null, cancellationToken);
        }

        /// <summary>
        /// This method checks if the current executable as the default handler for a protocol (aka URI scheme).
        /// If so, it will remove the app as the default handler.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="path">Defaults to process.execPath.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, string path, CancellationToken cancellationToken = default)
        {
            return await RemoveAsDefaultProtocolClientAsync(protocol, path, null, cancellationToken);
        }

        /// <summary>
        /// This method checks if the current executable as the default handler for a protocol (aka URI scheme).
        /// If so, it will remove the app as the default handler.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="path">Defaults to process.execPath.</param>
        /// <param name="args">Defaults to an empty array.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, string path, string[] args, CancellationToken cancellationToken = default)
        {           
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appRemoveAsDefaultProtocolClientCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appRemoveAsDefaultProtocolClientCompleted");
                    taskCompletionSource.SetResult((bool) success);
                });

                BridgeConnector.Socket.Emit("appRemoveAsDefaultProtocolClient", protocol, path, args);

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This method checks if the current executable is the default handler for a protocol (aka URI scheme).
        /// <para/>
        /// Note: On macOS, you can use this method to check if the app has been registered as the default protocol
        /// handler for a protocol. You can also verify this by checking ~/Library/Preferences/com.apple.LaunchServices.plist
        /// on the macOS machine. Please refer to <see href="https://developer.apple.com/library/mac/documentation/Carbon/Reference/LaunchServicesReference/#//apple_ref/c/func/LSCopyDefaultHandlerForURLScheme">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// The API uses the Windows Registry and LSCopyDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the current executable is the default handler for a protocol (aka URI scheme).</returns>
        public async Task<bool> IsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)
        {
            return await IsDefaultProtocolClientAsync(protocol, null, null, cancellationToken);
        }

        /// <summary>
        /// This method checks if the current executable is the default handler for a protocol (aka URI scheme).
        /// <para/>
        /// Note: On macOS, you can use this method to check if the app has been registered as the default protocol
        /// handler for a protocol. You can also verify this by checking ~/Library/Preferences/com.apple.LaunchServices.plist
        /// on the macOS machine. Please refer to <see href="https://developer.apple.com/library/mac/documentation/Carbon/Reference/LaunchServicesReference/#//apple_ref/c/func/LSCopyDefaultHandlerForURLScheme">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// The API uses the Windows Registry and LSCopyDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="path">Defaults to process.execPath.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the current executable is the default handler for a protocol (aka URI scheme).</returns>
        public async Task<bool> IsDefaultProtocolClientAsync(string protocol, string path, CancellationToken cancellationToken = default)
        {
            return await IsDefaultProtocolClientAsync(protocol, path, null, cancellationToken);
        }

        /// <summary>
        /// This method checks if the current executable is the default handler for a protocol (aka URI scheme).
        /// <para/>
        /// Note: On macOS, you can use this method to check if the app has been registered as the default protocol
        /// handler for a protocol. You can also verify this by checking ~/Library/Preferences/com.apple.LaunchServices.plist
        /// on the macOS machine. Please refer to <see href="https://developer.apple.com/library/mac/documentation/Carbon/Reference/LaunchServicesReference/#//apple_ref/c/func/LSCopyDefaultHandlerForURLScheme">Apple's documentation</see>
        /// for details.
        /// <para/>
        /// The API uses the Windows Registry and LSCopyDefaultHandlerForURLScheme internally.
        /// </summary>
        /// <param name="protocol">The name of your protocol, without ://.</param>
        /// <param name="path">Defaults to process.execPath.</param>
        /// <param name="args">Defaults to an empty array.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the current executable is the default handler for a protocol (aka URI scheme).</returns>
        public async Task<bool> IsDefaultProtocolClientAsync(string protocol, string path, string[] args, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appIsDefaultProtocolClientCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appIsDefaultProtocolClientCompleted");
                    taskCompletionSource.SetResult((bool) success);
                });

                BridgeConnector.Socket.Emit("appIsDefaultProtocolClient", protocol, path, args);

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds tasks to the <see cref="UserTask"/> category of the JumpList on Windows.
        /// <para/>
        /// Note: If you'd like to customize the Jump List even more use <see cref="SetJumpList"/> instead.
        /// </summary>
        /// <param name="userTasks">Array of <see cref="UserTask"/> objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> SetUserTasksAsync(UserTask[] userTasks, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appSetUserTasksCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appSetUserTasksCompleted");
                    taskCompletionSource.SetResult((bool) success);
                });

                BridgeConnector.Socket.Emit("appSetUserTasks", JArray.FromObject(userTasks, _jsonSerializer));

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Jump List settings for the application.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Jump List settings.</returns>
        public async Task<JumpListSettings> GetJumpListSettingsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<JumpListSettings>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetJumpListSettingsCompleted", (jumpListSettings) =>
                {
                    BridgeConnector.Socket.Off("appGetJumpListSettingsCompleted");
                    taskCompletionSource.SetResult(JObject.Parse(jumpListSettings.ToString()).ToObject<JumpListSettings>());
                });

                BridgeConnector.Socket.Emit("appGetJumpListSettings");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets or removes a custom Jump List for the application. If categories is null the previously set custom
        /// Jump List (if any) will be replaced by the standard Jump List for the app (managed by Windows).
        /// <para/>
        /// Note: If a <see cref="JumpListCategory"/> object has neither the <see cref="JumpListCategory.Type"/> nor
        /// the <see cref="JumpListCategory.Name"/> property set then its <see cref="JumpListCategory.Type"/> is assumed
        /// to be <see cref="JumpListCategoryType.tasks"/>. If the <see cref="JumpListCategory.Name"/> property is set but
        /// the <see cref="JumpListCategory.Type"/> property is omitted then the <see cref="JumpListCategory.Type"/> is
        /// assumed to be <see cref="JumpListCategoryType.custom"/>.
        /// <para/>
        /// Note: Users can remove items from custom categories, and Windows will not allow a removed item to be added
        /// back into a custom category until after the next successful call to <see cref="SetJumpList"/>. Any attempt
        /// to re-add a removed item to a custom category earlier than that will result in the entire custom category being
        /// omitted from the Jump List. The list of removed items can be obtained using <see cref="GetJumpListSettingsAsync"/>.
        /// </summary>
        /// <param name="categories">Array of <see cref="JumpListCategory"/> objects.</param>
        public void SetJumpList(JumpListCategory[] categories)
        {
            BridgeConnector.Socket.Emit("appSetJumpList", JArray.FromObject(categories, _jsonSerializer));
        }

        /// <summary>
        /// The return value of this method indicates whether or not this instance of your application successfully obtained
        /// the lock. If it failed to obtain the lock, you can assume that another instance of your application is already
        /// running with the lock and exit immediately.
        /// <para/>
        /// I.e.This method returns <see langword="true"/> if your process is the primary instance of your application and your
        /// app should continue loading. It returns <see langword="false"/> if your process should immediately quit as it has
        /// sent its parameters to another instance that has already acquired the lock.
        /// <para/>
        /// On macOS, the system enforces single instance automatically when users try to open a second instance of your app
        /// in Finder, and the open-file and open-url events will be emitted for that.However when users start your app in
        /// command line, the system's single instance mechanism will be bypassed, and you have to use this method to ensure
        /// single instance.
        /// </summary>
        /// <param name="newInstanceOpened">Lambda with an array of the second instance’s command line arguments.
        /// The second parameter is the working directory path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This method returns false if your process is the primary instance of the application and your app
        /// should continue loading. And returns true if your process has sent its parameters to another instance, and
        /// you should immediately quit.
        /// </returns>
        public async Task<bool> RequestSingleInstanceLockAsync(Action<string[], string> newInstanceOpened, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appRequestSingleInstanceLockCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appRequestSingleInstanceLockCompleted");
                    taskCompletionSource.SetResult((bool)success);
                });

                BridgeConnector.Socket.Off("secondInstance");
                BridgeConnector.Socket.On("secondInstance", (result) =>
                {
                    JArray results = (JArray)result;
                    string[] args = results.First.ToObject<string[]>();
                    string workingDirectory = results.Last.ToObject<string>();

                    newInstanceOpened(args, workingDirectory);
                });

                BridgeConnector.Socket.Emit("appRequestSingleInstanceLock");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Releases all locks that were created by makeSingleInstance. This will allow
        /// multiple instances of the application to once again run side by side.
        /// </summary>
        public void ReleaseSingleInstanceLock()
        {
            BridgeConnector.Socket.Emit("appReleaseSingleInstanceLock");
        }

        /// <summary>
        /// This method returns whether or not this instance of your app is currently holding the single instance lock.
        /// You can request the lock with <see cref="RequestSingleInstanceLockAsync"/> and release with
        /// <see cref="ReleaseSingleInstanceLock"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> HasSingleInstanceLockAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appHasSingleInstanceLockCompleted", (hasLock) =>
                {
                    BridgeConnector.Socket.Off("appHasSingleInstanceLockCompleted");
                    taskCompletionSource.SetResult((bool) hasLock);
                });

                BridgeConnector.Socket.Emit("appHasSingleInstanceLock");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates an NSUserActivity and sets it as the current activity. The activity is
        /// eligible for <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see>
        /// to another device afterward.
        /// </summary>
        /// <param name="type">Uniquely identifies the activity. Maps to <see href="https://developer.apple.com/library/ios/documentation/Foundation/Reference/NSUserActivity_Class/index.html#//apple_ref/occ/instp/NSUserActivity/activityType">NSUserActivity.activityType</see>.</param>
        /// <param name="userInfo">App-specific state to store for use by another device.</param>
        public void SetUserActivity(string type, object userInfo)
        {
            SetUserActivity(type, userInfo, null);
        }

        /// <summary>
        /// Creates an NSUserActivity and sets it as the current activity. The activity is
        /// eligible for <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see>
        /// to another device afterward.
        /// </summary>
        /// <param name="type">
        /// Uniquely identifies the activity. Maps to <see href="https://developer.apple.com/library/ios/documentation/Foundation/Reference/NSUserActivity_Class/index.html#//apple_ref/occ/instp/NSUserActivity/activityType">NSUserActivity.activityType</see>.
        /// </param>
        /// <param name="userInfo">App-specific state to store for use by another device.</param>
        /// <param name="webpageUrl">
        /// The webpage to load in a browser if no suitable app is installed on the resuming device. The scheme must be http or https.
        /// </param>
        public void SetUserActivity(string type, object userInfo, string webpageUrl)
        {
            BridgeConnector.Socket.Emit("appSetUserActivity", type, userInfo, webpageUrl);
        }

        /// <summary>
        /// The type of the currently running activity.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> GetCurrentActivityTypeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetCurrentActivityTypeCompleted", (activityType) =>
                {
                    BridgeConnector.Socket.Off("appGetCurrentActivityTypeCompleted");
                    taskCompletionSource.SetResult(activityType.ToString());
                });

                BridgeConnector.Socket.Emit("appGetCurrentActivityType");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Invalidates the current <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see> user activity.
        /// </summary>
        public void InvalidateCurrentActivity()
        {
            BridgeConnector.Socket.Emit("appInvalidateCurrentActivity");
        }

        /// <summary>
        /// Marks the current <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see> user activity as inactive without invalidating it.
        /// </summary>
        public void ResignCurrentActivity()
        {
            BridgeConnector.Socket.Emit("appResignCurrentActivity");
        }

        /// <summary>
        /// Changes the <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd378459(v=vs.85).aspx">Application User Model ID</see> to id.
        /// </summary>
        /// <param name="id">Model Id.</param>
        public void SetAppUserModelId(string id)
        {
            BridgeConnector.Socket.Emit("appSetAppUserModelId", id);
        }

        /// TODO: Check new parameter which is a function [App.ImportCertificate]
        /// <summary>
        /// Imports the certificate in pkcs12 format into the platform certificate store.
        /// callback is called with the result of import operation, a value of 0 indicates
        /// success while any other value indicates failure according to chromium net_error_list.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Result of import. Value of 0 indicates success.</returns>
        public async Task<int> ImportCertificateAsync(ImportCertificateOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<int>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appImportCertificateCompleted", (result) =>
                {
                    BridgeConnector.Socket.Off("appImportCertificateCompleted");
                    taskCompletionSource.SetResult((int) result);
                });

                BridgeConnector.Socket.Emit("appImportCertificate", JObject.FromObject(options, _jsonSerializer));

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Memory and cpu usage statistics of all the processes associated with the app.
        /// </summary>
        /// <returns>
        /// Array of ProcessMetric objects that correspond to memory and cpu usage
        /// statistics of all the processes associated with the app.
        /// <param name="cancellationToken">The cancellation token.</param>
        /// </returns>
        public async Task<ProcessMetric[]> GetAppMetricsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<ProcessMetric[]>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetAppMetricsCompleted", (result) =>
                {
                    BridgeConnector.Socket.Off("appGetAppMetricsCompleted");
                    var processMetrics = ((JArray)result).ToObject<ProcessMetric[]>();

                    taskCompletionSource.SetResult(processMetrics);
                });

                BridgeConnector.Socket.Emit("appGetAppMetrics");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The Graphics Feature Status from chrome://gpu/.
        /// <para/>
        /// Note: This information is only usable after the gpu-info-update event is emitted.
        /// <param name="cancellationToken">The cancellation token.</param>
        /// </summary>
        public async Task<GPUFeatureStatus> GetGpuFeatureStatusAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<GPUFeatureStatus>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetGpuFeatureStatusCompleted", (result) =>
                {
                    BridgeConnector.Socket.Off("appGetGpuFeatureStatusCompleted");
                    var gpuFeatureStatus = ((JObject)result).ToObject<GPUFeatureStatus>();

                    taskCompletionSource.SetResult(gpuFeatureStatus);
                });

                BridgeConnector.Socket.Emit("appGetGpuFeatureStatus");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets the counter badge for current app. Setting the count to 0 will hide the badge.
        /// On macOS it shows on the dock icon. On Linux it only works for Unity launcher.
        /// <para/>
        /// Note: Unity launcher requires the existence of a .desktop file to work, for more
        /// information please read <see href="https://www.electronjs.org/docs/tutorial/desktop-environment-integration#unity-launcher">Desktop Environment Integration</see>.
        /// </summary>
        /// <param name="count">Counter badge.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the call succeeded.</returns>
        public async Task<bool> SetBadgeCountAsync(int count, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appSetBadgeCountCompleted", (success) =>
                {
                    BridgeConnector.Socket.Off("appSetBadgeCountCompleted");
                    taskCompletionSource.SetResult((bool) success);
                });

                BridgeConnector.Socket.Emit("appSetBadgeCount", count);

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The current value displayed in the counter badge.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<int> GetBadgeCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<int>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetBadgeCountCompleted", (count) =>
                {
                    BridgeConnector.Socket.Off("appGetBadgeCountCompleted");
                    taskCompletionSource.SetResult((int)count);
                });

                BridgeConnector.Socket.Emit("appGetBadgeCount");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// A <see cref="CommandLine"/> object that allows you to read and manipulate the command line arguments that Chromium uses.
        /// </summary>
        public CommandLine CommandLine { get; internal set; }

        /// <summary>
        /// Whether the current desktop environment is Unity launcher.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> IsUnityRunningAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appIsUnityRunningCompleted", (isUnityRunning) =>
                {
                    BridgeConnector.Socket.Off("appIsUnityRunningCompleted");
                    taskCompletionSource.SetResult((bool)isUnityRunning);
                });

                BridgeConnector.Socket.Emit("appIsUnityRunning");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// If you provided path and args options to <see cref="SetLoginItemSettings"/> then you need to pass the same
        /// arguments here for <see cref="LoginItemSettings.OpenAtLogin"/> to be set correctly.
        /// </summary>
        public async Task<LoginItemSettings> GetLoginItemSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await GetLoginItemSettingsAsync(null, cancellationToken);
        }

        /// <summary>
        /// If you provided path and args options to <see cref="SetLoginItemSettings"/> then you need to pass the same
        /// arguments here for <see cref="LoginItemSettings.OpenAtLogin"/> to be set correctly.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<LoginItemSettings> GetLoginItemSettingsAsync(LoginItemSettingsOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<LoginItemSettings>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appGetLoginItemSettingsCompleted", (loginItemSettings) =>
                {
                    BridgeConnector.Socket.Off("appGetLoginItemSettingsCompleted");

                    var result = ((JObject) loginItemSettings).ToObject<LoginItemSettings>();

                    taskCompletionSource.SetResult(result);
                });

                if (options == null)
                {
                    BridgeConnector.Socket.Emit("appGetLoginItemSettings");
                }
                else
                {
                    BridgeConnector.Socket.Emit("appGetLoginItemSettings", JObject.FromObject(options, _jsonSerializer));
                }

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Set the app's login item settings.
        /// To work with Electron's autoUpdater on Windows, which uses <see href="https://github.com/Squirrel/Squirrel.Windows">Squirrel</see>,
        /// you'll want to set the launch path to Update.exe, and pass arguments that specify your application name.
        /// </summary>
        /// <param name="loginSettings"></param>
        public void SetLoginItemSettings(LoginSettings loginSettings)
        {
            BridgeConnector.Socket.Emit("appSetLoginItemSettings", JObject.FromObject(loginSettings, _jsonSerializer));
        }

        /// <summary>
        /// <see langword="true"/> if Chrome's accessibility support is enabled, <see langword="false"/> otherwise. This API will
        /// return <see langword="true"/> if the use of assistive technologies, such as screen readers, has been detected.
        /// See <see href="chromium.org/developers/design-documents/accessibility">Chromium's accessibility docs</see> for more details.
        /// </summary>
        /// <returns><see langword="true"/> if Chrome’s accessibility support is enabled, <see langword="false"/> otherwise.</returns>
        public async Task<bool> IsAccessibilitySupportEnabledAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appIsAccessibilitySupportEnabledCompleted", (isAccessibilitySupportEnabled) =>
                {
                    BridgeConnector.Socket.Off("appIsAccessibilitySupportEnabledCompleted");
                    taskCompletionSource.SetResult((bool)isAccessibilitySupportEnabled);
                });

                BridgeConnector.Socket.Emit("appIsAccessibilitySupportEnabled");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Manually enables Chrome's accessibility support, allowing to expose accessibility switch to users in application settings.
        /// See <see href="chromium.org/developers/design-documents/accessibility">Chromium's accessibility docs</see> for more details.
        /// Disabled (<see langword="false"/>) by default.
        /// <para/>
        /// This API must be called after the <see cref="Ready"/> event is emitted.
        /// <para/>
        /// Note: Rendering accessibility tree can significantly affect the performance of your app. It should not be enabled by default.
        /// </summary>
        /// <param name="enabled">Enable or disable <see href="https://developers.google.com/web/fundamentals/accessibility/semantics-builtin/the-accessibility-tree">accessibility tree</see> rendering.</param>
        public void SetAccessibilitySupportEnabled(bool enabled)
        {
            BridgeConnector.Socket.Emit("appSetAboutPanelOptions", enabled);
        }

        /// <summary>
        /// Show the app's about panel options. These options can be overridden with
        /// <see cref="SetAboutPanelOptions"/>.
        /// </summary>
        public void ShowAboutPanel()
        {
            BridgeConnector.Socket.Emit("appShowAboutPanel");
        }

        /// <summary>
        /// Set the about panel options. This will override the values defined in the app's .plist file on macOS. See the
        /// <see href="https://developer.apple.com/reference/appkit/nsapplication/1428479-orderfrontstandardaboutpanelwith?language=objc">Apple docs</see>
        /// for more details. On Linux, values must be set in order to be shown; there are no defaults.
        /// <para/>
        /// If you do not set credits but still wish to surface them in your app, AppKit will look for a file named "Credits.html",
        /// "Credits.rtf", and "Credits.rtfd", in that order, in the bundle returned by the NSBundle class method main. The first file
        /// found is used, and if none is found, the info area is left blank. See Apple
        /// <see href="https://developer.apple.com/documentation/appkit/nsaboutpaneloptioncredits?language=objc">documentation</see> for more information.
        /// </summary>
        /// <param name="options">About panel options.</param>
        public void SetAboutPanelOptions(AboutPanelOptions options)
        {
            BridgeConnector.Socket.Emit("appSetAboutPanelOptions", JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// A <see cref="string"/> which is the user agent string Electron will use as a global fallback.
        /// <para/>
        /// This is the user agent that will be used when no user agent is set at the webContents or
        /// session level. It is useful for ensuring that your entire app has the same user agent. Set to a
        /// custom value as early as possible in your app's initialization to ensure that your overridden value
        /// is used.
        /// </summary>
        public string UserAgentFallback
        {
            get
            {
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On("appGetUserAgentFallbackCompleted", (result) =>
                    {
                        BridgeConnector.Socket.Off("appGetUserAgentFallbackCompleted");
                        taskCompletionSource.SetResult((string)result);
                    });

                    BridgeConnector.Socket.Emit("appGetUserAgentFallback");

                    return taskCompletionSource.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("appSetUserAgentFallback", value);
            }
        }

        internal void PreventQuit()
        {
            _preventQuit = true;
        }

        private bool _preventQuit = false;
    }
}