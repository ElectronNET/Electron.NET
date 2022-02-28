using ElectronNET.API.Entities;
using ElectronNET.API.Hubs;
using Microsoft.AspNetCore.SignalR;
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-window-all-closed-event", GetHashCode());
                }
                _windowAllClosed += value;
            }
            remove
            {
                _windowAllClosed -= value;
            }
        }

        public void TriggerOnWindowAllClosed()
        {
            if (!Electron.WindowManager.IsQuitOnWindowAllClosed || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _windowAllClosed();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-before-quit-event", GetHashCode());
                }
                _beforeQuit += value;
            }
            remove
            {
                _beforeQuit -= value;
            }
        }

        public async Task TriggerOnBeforeQuit(QuitEventArgs quitEventArgs)
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
            _beforeQuit(quitEventArgs);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-will-quit-event", GetHashCode());
                }
                _willQuit += value;
            }
            remove
            {
                _willQuit -= value;
            }
        }

        public async Task TriggerOnWillQuit(QuitEventArgs quitEventArgs)
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
            _willQuit(quitEventArgs);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-will-quit-event", GetHashCode());
                }
                _quitting += value;
            }
            remove
            {
                _quitting -= value;
            }
        }

        public async Task TriggerOnQuitting()
        {
            if (_willQuit == null)
            {
                await _quitting();
                Exit();
            }

            _quitting();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-browser-window-blur-event", GetHashCode());
                }
                _browserWindowBlur += value;
            }
            remove
            {
                _browserWindowBlur -= value;
            }
        }

        public void TriggerOnBrowserWindowBlur()
        {
            _browserWindowBlur();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-browser-window-focus-event", GetHashCode());
                }
                _browserWindowFocus += value;
            }
            remove
            {
                _browserWindowFocus -= value;
            }
        }

        public void TriggerOnBrowserWindowFocus()
        {
            _browserWindowFocus();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-browser-window-created-event", GetHashCode());
                }
                _browserWindowCreated += value;
            }
            remove
            {
                _browserWindowCreated -= value;
            }
        }

        public void TriggerOnBrowserWindowCreated()
        {
            _browserWindowCreated();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-web-contents-created-event", GetHashCode());
                }
                _webContentsCreated += value;
            }
            remove
            {
                _webContentsCreated -= value;
            }
        }

        public void TriggerOnWebContentsCreated()
        {
            _browserWindowCreated();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-accessibility-support-changed-event", GetHashCode());
                }
                _accessibilitySupportChanged += value;
            }
            remove
            {
                _accessibilitySupportChanged -= value;
            }
        }

        public void TriggerOnAccessibilitySupportChanged(bool state)
        {
            _accessibilitySupportChanged(state);
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
        /// Emitted when a MacOS user wants to open a file with the application. The open-file event is usually emitted
        /// when the application is already open and the OS wants to reuse the application to open the file.
        /// open-file is also emitted when a file is dropped onto the dock and the application is not yet running.
        /// <para/>
        /// On Windows, you have to parse the arguments using App.CommandLine to get the filepath.
        /// </summary>
        public event Action<string> OpenFile
        {
            add
            {
                if (_openFile == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-open-file-event", GetHashCode());
                }
                _openFile += value;
            }
            remove
            {
                _openFile -= value;
            }
        }

        public void TriggerOnOpenFile(string openFile)
        {
            _openFile(openFile);
        }

        private event Action<string> _openFile;


        /// <summary>
        /// Emitted when a MacOS user wants to open a URL with the application. Your application's Info.plist file must
        /// define the URL scheme within the CFBundleURLTypes key, and set NSPrincipalClass to AtomApplication.
        /// </summary>
        public event Action<string> OpenUrl
        {
            add
            {
                if (_openUrl == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-app-open-url-event", GetHashCode());
                }
                _openUrl += value;
            }
            remove
            {
                _openUrl -= value;
            }
        }

        public void TriggerOnOpenUrl(string openUrl)
        {
            _openUrl(openUrl);
        }


        private event Action<string> _openUrl;

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
            [Obsolete("Use the asynchronous version NameAsync instead")]
            get
            {
                return NameAsync.Result;
            }
            set
            {
                Electron.SignalrElectron.Clients.All.SendAsync("appSetName", value);
            }
        }

        /// <summary>
        /// A <see cref="string"/> property that indicates the current application's name, which is the name in the
        /// application's package.json file.
        ///
        /// Usually the name field of package.json is a short lowercase name, according to the npm modules spec. You
        /// should usually also specify a productName field, which is your application's full capitalized name, and
        /// which will be preferred over name by Electron.
        /// </summary>
        public Task<string> NameAsync
        {
            get
            {
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    var signalrResult = SignalrSerializeHelper.GetSignalrResultString("appGetName");
                    taskCompletionSource.SetResult(signalrResult.Result);

                    return taskCompletionSource.Task;
                });
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
        public async void Quit()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appQuit");
        }

        /// <summary>
        /// All windows will be closed immediately without asking user and the <see cref="BeforeQuit"/> and <see cref="WillQuit"/>
        /// events will not be emitted.
        /// </summary>
        /// <param name="exitCode">Exits immediately with exitCode. exitCode defaults to 0.</param>
        public async void Exit(int exitCode = 0)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appExit", exitCode);
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
        public async void Relaunch()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appRelaunch");
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
        public async void Relaunch(RelaunchOptions relaunchOptions)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appRelaunch", JObject.FromObject(relaunchOptions, _jsonSerializer));
        }

        /// <summary>
        /// On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses
        /// on the application's first window.
        /// </summary>
        public async void Focus()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appFocus");
        }

        /// <summary>
        /// On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses
        /// on the application's first window.
        /// <para/>
        /// You should seek to use the <see cref="FocusOptions.Steal"/> option as sparingly as possible.
        /// </summary>
        public async void Focus(FocusOptions focusOptions)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appFocus", JObject.FromObject(focusOptions, _jsonSerializer));
        }

        /// <summary>
        /// Hides all application windows without minimizing them.
        /// </summary>
        public async void Hide()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appHide");
        }

        /// <summary>
        /// Shows application windows after they were hidden. Does not automatically focus them.
        /// </summary>
        public async void Show()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appShow");
        }

        /// <summary>
        /// The current application directory.
        /// </summary>
        public async Task<string> GetAppPathAsync(CancellationToken cancellationToken = default)
        {
            return (await SignalrSerializeHelper.GetSignalrResultString("appGetAppPath"));     
        }

        /// <summary>
        /// Sets or creates a directory your app's logs which can then be manipulated with <see cref="GetPathAsync"/>
        /// or <see cref="SetPath"/>.
        /// <para/>
        /// Calling <see cref="SetAppLogsPath"/> without a path parameter will result in this directory being set to
        /// ~/Library/Logs/YourAppName on macOS, and inside the userData directory on Linux and Windows.
        /// </summary>
        /// <param name="path">A custom path for your logs. Must be absolute.</param>
        public async void SetAppLogsPath(string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetAppLogsPath", path);
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
            return (await SignalrSerializeHelper.GetSignalrResultString("appGetPath", pathName.GetDescription()));
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
        public async void SetPath(PathName name, string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetPath", name.GetDescription(), path);
        }

        /// <summary>
        /// The version of the loaded application. If no version is found in the application’s package.json file, 
        /// the version of the current bundle or executable is returned.
        /// </summary>
        /// <returns>The version of the loaded application.</returns>
        public async Task<string> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            return (await SignalrSerializeHelper.GetSignalrResultString("appGetVersion"));
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
            return (await SignalrSerializeHelper.GetSignalrResultString("appGetLocale"));
        }

        /// <summary>
        /// Adds path to the recent documents list. This list is managed by the OS. On Windows you can visit the
        /// list from the task bar, and on macOS you can visit it from dock menu.
        /// </summary>
        /// <param name="path">Path to add.</param>
        public async void AddRecentDocument(string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appAddRecentDocument", path);
        }

        /// <summary>
        /// Clears the recent documents list.
        /// </summary>
        public async void ClearRecentDocuments()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appClearRecentDocuments");
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appSetAsDefaultProtocolClient", protocol, path, args);
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appRemoveAsDefaultProtocolClient", protocol, path, args);
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appIsDefaultProtocolClient", protocol, path, args);
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appSetUserTasks", JArray.FromObject(userTasks, _jsonSerializer));
        }

        /// <summary>
        /// Jump List settings for the application.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Jump List settings.</returns>
        public async Task<JumpListSettings> GetJumpListSettingsAsync(CancellationToken cancellationToken = default)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("appGetJumpListSettings");
            return signalrResult.ToObject<JumpListSettings>();
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
        public async void SetJumpList(JumpListCategory[] categories)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetJumpList", JArray.FromObject(categories, _jsonSerializer));
        }

        // ToDo: Need better option
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
            // ToDo: Check this function - Unsure if it's working as expected
            // Check if second-instance is getting triggereds
            var secondInstanceListener = await SignalrSerializeHelper.GetSignalrResultJArray("appRequestSingleInstanceLock");
            if (secondInstanceListener != null)
            {
                JArray results = secondInstanceListener;
                string[] args = results.First.ToObject<string[]>();
                string workingDirectory = results.Last.ToObject<string>();

                newInstanceOpened(args, workingDirectory);
            }

            return (await SignalrSerializeHelper.GetSignalrResultBool("appRequestSingleInstanceLock"));
        }


        /// <summary>
        /// Releases all locks that were created by makeSingleInstance. This will allow
        /// multiple instances of the application to once again run side by side.
        /// </summary>
        public async void ReleaseSingleInstanceLock()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appReleaseSingleInstanceLock");
        }

        /// <summary>
        /// This method returns whether or not this instance of your app is currently holding the single instance lock.
        /// You can request the lock with <see cref="RequestSingleInstanceLockAsync"/> and release with
        /// <see cref="ReleaseSingleInstanceLock"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> HasSingleInstanceLockAsync(CancellationToken cancellationToken = default)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("appHasSingleInstanceLock");
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
        public async void SetUserActivity(string type, object userInfo, string webpageUrl)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetUserActivity", type, userInfo, webpageUrl);
        }

        /// <summary>
        /// The type of the currently running activity.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> GetCurrentActivityTypeAsync(CancellationToken cancellationToken = default)
        {
            return await SignalrSerializeHelper.GetSignalrResultString("appGetCurrentActivityType");
        }

        /// <summary>
        /// Invalidates the current <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see> user activity.
        /// </summary>
        public async void InvalidateCurrentActivity()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appInvalidateCurrentActivity");
        }

        /// <summary>
        /// Marks the current <see href="https://developer.apple.com/library/ios/documentation/UserExperience/Conceptual/Handoff/HandoffFundamentals/HandoffFundamentals.html">Handoff</see> user activity as inactive without invalidating it.
        /// </summary>
        public async void ResignCurrentActivity()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appResignCurrentActivity");
        }

        /// <summary>
        /// Changes the <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd378459(v=vs.85).aspx">Application User Model ID</see> to id.
        /// </summary>
        /// <param name="id">Model Id.</param>
        public async void SetAppUserModelId(string id)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetAppUserModelId", id);
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
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultString("appImportCertificate", JObject.FromObject(options, _jsonSerializer));
            return int.Parse(signalrResult);
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
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("appGetAppMetrics");
            return signalrResult.ToObject<ProcessMetric[]>();
        }

        /// <summary>
        /// The Graphics Feature Status from chrome://gpu/.
        /// <para/>
        /// Note: This information is only usable after the gpu-info-update event is emitted.
        /// <param name="cancellationToken">The cancellation token.</param>
        /// </summary>
        public async Task<GPUFeatureStatus> GetGpuFeatureStatusAsync(CancellationToken cancellationToken = default)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("appGetGpuFeatureStatus");
            return signalrResult.ToObject<GPUFeatureStatus>();
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appSetBadgeCount", count);
        }

        /// <summary>
        /// The current value displayed in the counter badge.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<int> GetBadgeCountAsync(CancellationToken cancellationToken = default)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultString("appGetBadgeCount");
            return int.Parse(signalrResult);
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
            return await SignalrSerializeHelper.GetSignalrResultBool("appIsUnityRunning");
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
            JObject appGetLoginItemSettingsCompleted;
            if (options == null)
            {
                appGetLoginItemSettingsCompleted = await SignalrSerializeHelper.GetSignalrResultJObject("appGetLoginItemSettings");
            }
            else
            {
                appGetLoginItemSettingsCompleted = await SignalrSerializeHelper.GetSignalrResultJObject("appGetLoginItemSettingsWithOptions", JObject.FromObject(options, _jsonSerializer));
            }

            return appGetLoginItemSettingsCompleted.ToObject<LoginItemSettings>();
        }

        /// <summary>
        /// Set the app's login item settings.
        /// To work with Electron's autoUpdater on Windows, which uses <see href="https://github.com/Squirrel/Squirrel.Windows">Squirrel</see>,
        /// you'll want to set the launch path to Update.exe, and pass arguments that specify your application name.
        /// </summary>
        /// <param name="loginSettings"></param>
        public async void SetLoginItemSettings(LoginSettings loginSettings)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetLoginItemSettings", JObject.FromObject(loginSettings, _jsonSerializer));
        }

        /// <summary>
        /// <see langword="true"/> if Chrome's accessibility support is enabled, <see langword="false"/> otherwise. This API will
        /// return <see langword="true"/> if the use of assistive technologies, such as screen readers, has been detected.
        /// See <see href="chromium.org/developers/design-documents/accessibility">Chromium's accessibility docs</see> for more details.
        /// </summary>
        /// <returns><see langword="true"/> if Chrome’s accessibility support is enabled, <see langword="false"/> otherwise.</returns>
        public async Task<bool> IsAccessibilitySupportEnabledAsync(CancellationToken cancellationToken = default)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("appIsAccessibilitySupportEnabled");
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
        public async void SetAccessibilitySupportEnabled(bool enabled)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetAboutPanelOptions", enabled);
        }

        /// <summary>
        /// Show the app's about panel options. These options can be overridden with
        /// <see cref="SetAboutPanelOptions"/>.
        /// </summary>
        public async void ShowAboutPanel()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appShowAboutPanel");
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
        public async void SetAboutPanelOptions(AboutPanelOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("appSetAboutPanelOptions", JObject.FromObject(options, _jsonSerializer));
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
            [Obsolete("Use the asynchronous version UserAgentFallbackAsync instead")]
            get
            {
                return UserAgentFallbackAsync.Result;
            }
            set
            {
                Electron.SignalrElectron.Clients.All.SendAsync("appSetUserAgentFallback", value);
            }
        }

        /// <summary>
        /// A <see cref="string"/> which is the user agent string Electron will use as a global fallback.
        /// <para/>
        /// This is the user agent that will be used when no user agent is set at the webContents or
        /// session level. It is useful for ensuring that your entire app has the same user agent. Set to a
        /// custom value as early as possible in your app's initialization to ensure that your overridden value
        /// is used.
        /// </summary>
        public Task<string> UserAgentFallbackAsync
        {
            get
            {
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    var signalrResult = SignalrSerializeHelper.GetSignalrResultString("appGetUserAgentFallback");
                    taskCompletionSource.SetResult(signalrResult.Result);

                    return taskCompletionSource.Task;
                });
            }
        }

        internal void PreventQuit()
        {
            _preventQuit = true;
        }

        private bool _preventQuit = false;

        private const string ModuleName = "app";
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="App"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action fn)
            => Events.Instance.On(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="App"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action<object> fn)
            => Events.Instance.On(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="App"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action fn)
            => Events.Instance.Once(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="App"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action<object> fn)
            => Events.Instance.Once(ModuleName, eventName, fn);
    }
}