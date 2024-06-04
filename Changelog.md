# 30.0.3

(none)

# 23.6.2

## ElectronNET.API

* Fixed escaping of URL (#735) @cosmo0
* Fixed huge memory waste in IpcMain.Once (#833) @Yuvix25
* Fixed enabling of accessibility support (#798) @dlanorok
* Updated `Display` to current spec (#828) @Yuvix25
* Changed `ZoomFactor` type from `int` to `double` (#754) @Yuvix25
* Added splash screen size config (#822) @NickRimmer
* Added OSX ARM architecture detection (#821) @sajmonr
* Added support for `did-navigate` event (#819) @NickRimmer
* Added support for `will-redirect` event (#819) @NickRimmer
* Added support for `did-fail-load` event (#819) @NickRimmer
* Added support for `did-start-navigation` event (#819) @NickRimmer
* Added support for `did-redirect-navigation` event (#819) @NickRimmer
* Added support for `dom-ready` event (#813) @softworkz

## ElectronNET.CLI

* (none)

## Infrastructure

* Changed build system to NUKE (#757) @FlorianRappl
* Updated target framework for host project (#753) @r-pankevicius
* Fixed typo in README (#800) @franpersanchez

# 23.6.1

## ElectronNET.CLI

* New Feature: Upgrade to .NET 6 support

## ElectronNET.API

* New Feature: Native Electron 23.2.0 support, but not all new API features included (we search contributors)
* New Feature: Upgrade to .NET 6 support
* New Feature: Changed Web-Socket .NET Library to [SocketIOClient](https://github.com/doghappy/socket.io-client-csharp)
* Breaking Changes: We removed deprecated API events/methods from ElectronNET.API [(More Details)](https://www.electronjs.org/docs/latest/breaking-changes)

# 13.5.1

ElectronNET.CLI:

* New Feature: Using exit code instead of seek for the term 'error' (thanks [TSrgy](https://github.com/TSrgy)) [\#562](https://github.com/ElectronNET/Electron.NET/pull/562)
* Fixed bug: Allow for property overrides to be passed in (thanks [danatcofo](https://github.com/danatcofo)) [\#531](https://github.com/ElectronNET/Electron.NET/pull/531)
Use `/p:propertyName=value` or `/property:propertyName=value` to pass in property overrides. This is equivalent to the `-p:` option documented here: [https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)
* Fixed bug: Add ability to pass an argument for "Version" for both the "dotnet publish" and "electron-builder" commands (thanks [tub5](https://github.com/tub5)) [\#546](https://github.com/ElectronNET/Electron.NET/pull/546)
* Fixed bug: Version flag not persisting with the referenced executable (thanks [tub5](https://github.com/tub5)) [\#585](https://github.com/ElectronNET/Electron.NET/pull/585)
* Fixed bug: Changes PublishSingleFile default to false for NET5 compatibility (thanks [cristiangiagante](https://github.com/cristiangiagante)) [\#570](https://github.com/ElectronNET/Electron.NET/pull/570)

ElectronNET.API:

* New Feature: Native Electron 13.1.5 support, but not all new features (we search contributors)
* Breaking API Changes (from native Electron 13.1.5):
   - `Shell.MoveItemToTrashAsync` renamed with `Shell.TrashItemAsync`
   - The deprecated extension APIs have been removed: `BrowserWindow.GetAllExtensionsAsync()`, `BrowserWindow.RemoveExtension()`, `BrowserWindow.AddExtensionAsync()`. Use the session APIs instead: `Session.GetAllExtensionsAsync()`, `Session.RemoveExtension()`, `Session.LoadExtensionAsync()`.
* New Feature: singleInstance handle command line arguments [\#520](https://github.com/ElectronNET/Electron.NET/issues/520)
* New Feature: Add WebContents [insertCSS](https://www.electronjs.org/docs/api/web-contents#contentsinsertcsscss-options) functionality (thanks [nfichter](https://github.com/nfichter)) [\#559](https://github.com/ElectronNET/Electron.NET/pull/559)
* New Feature: Allow IpcMain to send IPC messages to BrowserViews (thanks [nfichter](https://github.com/nfichter)) [\#560](https://github.com/ElectronNET/Electron.NET/pull/560)
* New Feature: Add support for proxies that require basic username/password authentication (thanks [nfichter](https://github.com/nfichter)) [\#561](https://github.com/ElectronNET/Electron.NET/pull/561)
* New Feature: Add PostData to LoadURLOptions to allow http-posts in LoadURL calls (thanks [Funkrusha](https://github.com/Funkrusha)) [\#547](https://github.com/ElectronNET/Electron.NET/pull/547)
* Fixed bug: Fix splash screen interaction causing crashes, ghost dragging, and resizable behavior #540 (thanks [MiniguyBrendan](https://github.com/MiniguyBrendan)) [\#540](https://github.com/ElectronNET/Electron.NET/pull/540)
* Fixed bug: Vibrancy serialization fix (thanks [tantumalice](https://github.com/tantumalice)) [\#573](https://github.com/ElectronNET/Electron.NET/pull/573)

# 11.5.1

ElectronNET.CLI:

* New Feature: Added new build and start commandline options for single exe (thanks [nathanwienand](https://github.com/nathanwienand)) [\#506](https://github.com/ElectronNET/Electron.NET/pull/506)
* New Feature: Set a description of the app in `electron.manifest.json` (thanks [BurtsevC](https://github.com/BurtsevC)) [\#433](https://github.com/ElectronNET/Electron.NET/pull/433)
* New Feature: Set a target for the start command (thanks [gabecook](https://github.com/gabecook)) [\#463](https://github.com/ElectronNET/Electron.NET/pull/463)
* New Feature: `electronize init` support for F# projects (thanks [kojo12228](https://github.com/kojo12228)) [\#457](https://github.com/ElectronNET/Electron.NET/pull/457)
* New Feature: Linux support for the buildAll.sh (thanks [duncanawoods](https://github.com/duncanawoods)) [\#465](https://github.com/ElectronNET/Electron.NET/pull/465)
* Fixed bug: ERR_UNKNOWN_URL_SCHEME by intercepting file:// protocol (thanks [duncanawoods](https://github.com/duncanawoods)) [\#467](https://github.com/ElectronNET/Electron.NET/pull/467)

ElectronNET.API:

* New Feature: Native Electron 11.1.1 support, but not all new features (we search contributors)
* Breaking API Changes (from native Electron 11.0): - Removed: BrowserView.{destroy, fromId, fromWebContents, getAllViews} and id property of BrowserView
* New Feature: Upgrade to .NET 5 (thanks [scottkuhl](https://github.com/scottkuhl)) [\#509](https://github.com/ElectronNET/Electron.NET/pull/509)
* New Feature: Extension Method for adding the Electron static class members to the standard MS DI Containers, this is a QOL issue only. `services.AddElectron()` (thanks [danatcofo](https://github.com/danatcofo )) [\#528](https://github.com/ElectronNET/Electron.NET/pull/528)
* New Feature: SetMenu completed for the Dock (MacOS) (thanks [danatcofo](https://github.com/danatcofo )) [\#528](https://github.com/ElectronNET/Electron.NET/pull/528)

Example for the Dock Menu

`Electron.Dock.SetMenu(new [] {
   new MenuItem {
      Label = "Dock Menu Item",
      Click = () => {
         // do something
      }
   },
});`

Example for consuming the activate event (MacOs only)

`Electron.App.On("activate", obj => {
   var hasWindows = (bool)obj;
   // do something
});`

* New Feature: On and Once implementations for the App and Tray to cover the plethora of events that are not mapped explicitly in those two modules. (thanks [danatcofo](https://github.com/danatcofo )) [\#528](https://github.com/ElectronNET/Electron.NET/pull/528)
* New Feature: Adding the `EnableRemoteModule` property to the WebPreferences object. As of Electron 10, this property defaulted to false and without it exposed you can't use the remote module within a window. (thanks [danatcofo](https://github.com/danatcofo )) [\#528](https://github.com/ElectronNET/Electron.NET/pull/528)
* New Feature: Adding a configurable default electron port. (thanks [aarong-av](https://github.com/aarong-av)) [\#505](https://github.com/ElectronNET/Electron.NET/pull/505)
* New Feature: Added support for launching the application with a file on MacOS (thanks [dlitty](https://github.com/dlitty)) [\#478](https://github.com/ElectronNET/Electron.NET/pull/478)
* Improved: Avoid Blocking Calls in App and AutoUpdater (thanks [freosc](https://github.com/freosc)) [\#474](https://github.com/ElectronNET/Electron.NET/pull/474)
* Fixed bug: Maintain references between socket.io connection events (thanks [danatcofo](https://github.com/danatcofo )) [\#468](https://github.com/ElectronNET/Electron.NET/pull/486)
* Fixed bug: Set default WebPreferences.DefaultFontSize (thanks [duncanawoods](https://github.com/duncanawoods)) [\#468](https://github.com/ElectronNET/Electron.NET/pull/468)

# 9.31.2

* Electron-Builder fixed for Windows builds.

# 9.31.1

ElectronNET.CLI:

* New Feature: Added config parameter (thanks [konstantingross](https://github.com/konstantingross)) [\#409](https://github.com/ElectronNET/Electron.NET/pull/409)
* New Feature: Set the configuration environment with the electron.manifest.json file.
* Fixed bug: Custom user path removed and replaced by the correct directory with VS macro (When ElectronNET.CLI is the Startup Project, press F5 (Debug) and the ElectronNET.WebApp starts correctly without error!) (thanks [konstantingross](https://github.com/konstantingross)) [\#409](https://github.com/ElectronNET/Electron.NET/pull/409)

ElectronNET.API:

* New Feature: Native Electron 9.0.3 support, but not all new features (we search contributors)
* New Feature: PowerMonitor API Support (thanks [gustavo-lara-molina](https://github.com/gustavo-lara-molina)) [\#399](https://github.com/ElectronNET/Electron.NET/pull/399) [\#423](https://github.com/ElectronNET/Electron.NET/pull/423)
* New Feature: NativeTheme API Support (thanks [konstantingross](https://github.com/konstantingross)) [\#402](https://github.com/ElectronNET/Electron.NET/pull/402)
* New Feature: Cookie API Support (thanks [freosc](https://github.com/freosc)) [\#413](https://github.com/ElectronNET/Electron.NET/pull/413)
* Changed Feature: Removed dock methods from App API and moved to Dock API (thanks [konstantingross](https://github.com/konstantingross)) [\#422](https://github.com/ElectronNET/Electron.NET/pull/422)
* App-Api Enhancement: MenuItems with Submenus need an submenu type workaround [\#412](https://github.com/ElectronNET/Electron.NET/issues/412)
* App-Api Enhancement: Added UserAgentFallback (thanks [Mandrakia](https://github.com/Mandrakia)) [\#406](https://github.com/ElectronNET/Electron.NET/pull/406)
* App-Api Enhancement: Summaries rewritten, new App.IsReady / App.HasSingleInstanceLock property, App.Ready event, App.Focus with force parameter method, many parameters changes (thanks [konstantingross](https://github.com/konstantingross)) [\#415](https://github.com/ElectronNET/Electron.NET/pull/415) [\#422](https://github.com/ElectronNET/Electron.NET/pull/422)
* App-Api Enhancement: New App.IsReady property and App.Ready event (thanks [konstantingross](https://github.com/konstantingross)) [\#415](https://github.com/ElectronNET/Electron.NET/pull/415)
* Shell-Api Enhancement: API fixes for Electron 9.0.0 / Added missing parameters / Summaries rewritten (thanks [konstantingross](https://github.com/konstantingross)) [\#417](https://github.com/ElectronNET/Electron.NET/pull/417) [\#418](https://github.com/ElectronNET/Electron.NET/pull/418)
* Notification-Api Enhancement: Added missing properties in Notifications (thanks [konstantingross](https://github.com/konstantingross)) [\#410](https://github.com/ElectronNET/Electron.NET/pull/410)
* BrowserWindows-Api Enhancement: Add missing API call for SetProgressBar options (thanks [konstantingross](https://github.com/konstantingross)) [\#416](https://github.com/ElectronNET/Electron.NET/pull/416)
* BrowserWindow Enhancement: Add BrowserWindow.GetNativeWindowHandle() (thanks [kdlslyv](https://github.com/kdlslyv)) [\#429](https://github.com/ElectronNET/Electron.NET/pull/429)
* HostHook-Api Enhancement: HostHook.CallAsync should use TaskCompletionSource.SetException instead of throwing exception (thanks [Fre V](https://github.com/freosc)) [\#430](https://github.com/ElectronNET/Electron.NET/pull/430)
* MacOS Enhancement: Application exit logic (thanks [dafergu2](https://github.com/dafergu2)) [\#405](https://github.com/ElectronNET/Electron.NET/pull/405)
* Fixed bug: ElectronNET.API.Entities.WebPreferences.ContextIsolation [DefaultValue(true)] [\#411](https://github.com/ElectronNET/Electron.NET/issues/411)

ElectronNET.WebApp (internal use):
* Improvement debugging and testing new API calls (without install ElectronNET.CLI) (thanks [konstantingross](https://github.com/konstantingross)) [\#425](https://github.com/ElectronNET/Electron.NET/pull/425)
* Fixed bug: Cannot find modules in ElectronHostHook (thanks [konstantingross](https://github.com/konstantingross)) [\#425](https://github.com/ElectronNET/Electron.NET/pull/425)

Thank you for donation [Phil Seeman](https://github.com/mpnow) ❤

# 8.31.2

ElectronNET.CLI:
* New Feature: Deactivate PublishReadyToRun for build or start [\#395](https://github.com/ElectronNET/Electron.NET/issues/395)

   `electronize build /target win /PublishReadyToRun false`
   `electronize start /PublishReadyToRun false`
* Fixed bug: Application window doesn't open after packaging  [\#387](https://github.com/ElectronNET/Electron.NET/issues/387)

ElectronNET.API:

* New Feature: NativeImage Support (thanks [ThrDev](https://github.com/ThrDev)) [\#394](https://github.com/ElectronNET/Electron.NET/pull/394)
* New Feature: Update menu items for context menu and system tray on-the-fly. [\#270](https://github.com/ElectronNET/Electron.NET/pull/270)


# 8.31.1

ElectronNET.CLI:
* New Feature: Set a name and author of the app in `electron.manifest.json` [\#348](https://github.com/ElectronNET/Electron.NET/issues/348#issuecomment-615977950) [\#310](https://github.com/ElectronNET/Electron.NET/issues/310#issuecomment-617361086)
* New Feature: Live reload (thanks [syedadeel2](https://github.com/syedadeel2)) [\#390](https://github.com/ElectronNET/Electron.NET/pull/390)
`electronize start /watch`
* New Feature: Every new window will created with an clear cache [\#273](https://github.com/ElectronNET/Electron.NET/issues/273)
`electronize start /clear-cache`

ElectronNET.API:

* New Feature: Native Electron 8.2.3 support, but not all new features (we search contributors)
* New Feature: We incease the startup time for ~25-36% [\#356](https://github.com/ElectronNET/Electron.NET/issues/356)
* New Feature: Added print capability (thanks [x-xx-o](https://github.com/x-xx-o)) [\#355](https://github.com/ElectronNET/Electron.NET/pull/355)
* New Feature: BrowserView API [\#371](https://github.com/ElectronNET/Electron.NET/issues/371)
* Changed App.GetNameAsync and App.SetNameAsync to the App.Name Property [\#350](https://github.com/ElectronNET/Electron.NET/issues/350)
* Fixed bug: Splash Screen disappearing on click [\#357](https://github.com/ElectronNET/Electron.NET/issues/357)
* Fixed bug: Start MenuRole enum at 1 (thanks [jjuback](https://github.com/jjuback)) [\#369](https://github.com/ElectronNET/Electron.NET/pull/369)
* Fixed bug: BridgeConnector not connected (spam console) [\#347](https://github.com/ElectronNET/Electron.NET/issues/347)
* Fixed bug: BrowserWindowOptions is not setting Width and Height properly [\#373](https://github.com/ElectronNET/Electron.NET/issues/373)
* Fixed bug: IpcMain.Once(string) is not one time use, is not removing listener [\#366](https://github.com/ElectronNET/Electron.NET/issues/366)
* Fixed bug: IpcMain.RemoveAllListeners(string) is not removing the listeners [\#365](https://github.com/ElectronNET/Electron.NET/issues/365)
* Fixed bug: GetLoginItemSettingsAsync does not work [\#352](https://github.com/ElectronNET/Electron.NET/issues/352)
* Fixed bug: Using OnReadyToShow to display the main window in Blazor does not seem to work with Show set to false [\#361](https://github.com/ElectronNET/Electron.NET/issues/361)
* Fixed bug: Unable to disable WebSecurity along with NodeIntegration enabled [\#389](https://github.com/ElectronNET/Electron.NET/issues/389)

# 7.30.2

ElectronNET.CLI:

* New Feature: Different manifest file support [\#340](https://github.com/ElectronNET/Electron.NET/issues/340)
  * Create a additional manifest file: `electronize init /manifest test`
  * Start the app with your additional manifest file: `electronize start /manifest electron.manifest.test.json`
  * Build the app with your additional manifest file: `electronize build /target win /manifest electron.manifest.test.json`.

* New Feature: Command Line support [\#337](https://github.com/ElectronNET/Electron.NET/issues/337)
  * You can start the app with: `electronize start /args --dog=woof --test=true`
  * Or as binary: `myapp.exe /args --dog=woof --test=true`
* Fixed bug: Start process with listen port 8000 error. [\#308](https://github.com/ElectronNET/Electron.NET/issues/308) (thanks [thecodejedi](https://github.com/thecodejedi))
* Fixed bug: `electronize build` with no arguments would throw a `KeyNotFoundException`. (thanks [jamiebrynes7](https://github.com/jamiebrynes7))

ElectronNET.API:

* New Feature: Electron 7.1.2 support, but not all new features (we search contributors) [\#341](https://github.com/ElectronNET/Electron.NET/issues/341)
* New Feature: Electron.App.CommandLine API [\#337](https://github.com/ElectronNET/Electron.NET/issues/337)
* New Feature: Support of BrowserWindow.AddExtension, BrowserWindow.RemoveExtension, BrowserWindow.GetExtensions (thanks [Daddoon](https://github.com/Daddoon))

Thank you for donation [robertmclaws](https://github.com/robertmclaws) ❤

# 5.30.1

ElectronNET.CLI:

* Move to .NET Core 3.0
* Use npm npx instead of global installations (thanks [jimbuck](https://github.com/jimbuck))

ElectronNET.API:

* Move to .NET Core 3.0
* New Feature: Add BrowserWindow.RemoveMenu() (thanks [hack2root](https://github.com/hack2root))

Thanks to [MaherJendoubi](https://github.com/MaherJendoubi), [kant2002](https://github.com/kant2002), [raz-canva](https://github.com/raz-canva) and [Daddoon](https://github.com/Daddoon) to give .NET Core 3.0 feedback!
# 5.22.14

ElectronNET.CLI:

* Fixed bug: Build fails with latest electron-builder version [\#288](https://github.com/ElectronNET/Electron.NET/issues/288)

ElectronNET.API:

* New Feature: Full support for Auto Updater [(Based on electron-updater - Version 4.0.6)](https://www.electron.build/auto-update)
* New Feature: Support for set a custom static Port to ASP.NET Backend [\#155](https://github.com/ElectronNET/Electron.NET/issues/155)
* Fixed bug: Electron tray icon TypeError ([Electron original issue](https://github.com/electron/electron/issues/7657)) (thanks [Tum4ik](https://github.com/Tum4ik))
* Fixed bug: Wrong tray icon path in the application built via `electronize build` command (thanks [Tum4ik](https://github.com/Tum4ik))
* Fixed bug: fix async issue where same port is considered open [\#261](https://github.com/ElectronNET/Electron.NET/issues/261) (thanks [netpoetica](https://github.com/netpoetica))

ElectronNET.WebApp:

* Fixed usage of the `Electron.Tray.Show` according fixed bugs in the ElectronNET.CLI (thanks [Tum4ik](https://github.com/Tum4ik))

# 5.22.13

ElectronNET.API:

* Fixed bug: Menu Item visibility [\#257](https://github.com/ElectronNET/Electron.NET/issues/257)
* Fixed bug: electron.manifest.json - singleInstance not working [\#258](https://github.com/ElectronNET/Electron.NET/issues/258)
* Fixed security issue: ASP.NET Core process is now bound to 127.0.0.1 instead of the broader localhost [\#258](https://github.com/ElectronNET/Electron.NET/pull/266)

# 5.22.12

ElectronNET.CLI:

* New Feature: Changed from **electron packager** to [**electron builder**](https://www.electron.build/)
* New Feature: 'add hosthook' command for add a ElectronHostHook-Directory
* Fixed bug: 'Unexpected firewall warnings' [\#181](https://github.com/ElectronNET/Electron.NET/issues/181)
* Fixed bug: 'found 8 vulnerabilities (1 low, 5 moderate, 2 high)' [\#199](https://github.com/ElectronNET/Electron.NET/pull/199)
* Merged pull request: Call electronize from the Path instead of via dotnet in launchSettings.json [\#243](https://github.com/ElectronNET/Electron.NET/pull/243) (thanks [grbd](https://github.com/grbd))

ElectronNET.API:

* New Feature: Electron 5.0.1 support, but not all new features
* New Feature: Auto Updater [(Based on electron-updater)](https://www.electron.build/auto-update)
* New Feature: Splashscreen-Support
* New Feature: HostHook-API for execute own TypeScript/JavaScript code on native Electron Main-Process
* New Feature: Session-API functions
* Fixed bug: Node process running after stopping app [\#96](https://github.com/ElectronNET/Electron.NET/issues/96)
* Fixed bug: 'X and Y options to not work on Windows 10' [\#193](https://github.com/ElectronNET/Electron.NET/issues/193)
* Fixed bug: Unable to clear cache [\#66](https://github.com/ElectronNET/Electron.NET/issues/66)
* Merged pull request: Fix BrowserWindow::SetMenu [\#231](https://github.com/ElectronNET/Electron.NET/pull/231) thanks (thanks [CodeKenpachi](https://github.com/CodeKenpachi))
* Merged pull request: FIX application hangs after socket reconnect [\#233](https://github.com/ElectronNET/Electron.NET/pull/233) (thanks [pedromrpinto](https://github.com/pedromrpinto))
* Merged pull request: Reduce chance of detecting false positives when scanning subprocesses for errors. [\#236](https://github.com/ElectronNET/Electron.NET/pull/236) (thanks [BorisTheBrave](https://github.com/BorisTheBrave))
* Merged pull request: Updates the C# API to accept floating point as in JS. [\#240](https://github.com/ElectronNET/Electron.NET/pull/240) (thanks [BorisTheBrave](https://github.com/BorisTheBrave))
* Merged pull request: buildReleaseNuGetPackages should leave you in the same directory you …  [\#241](https://github.com/ElectronNET/Electron.NET/pull/241) (thanks [BorisTheBrave](https://github.com/BorisTheBrave))

ElectronNET.WebApp:

* Implemented a sample for the new HostHook-API
* Fixed bug: 'Electron.NET API Demo: unable to copy code?' [\#247](https://github.com/ElectronNET/Electron.NET/issues/247)

# 0.0.11

ElectronNET.CLI:

* Invoke 'npm install' without --prod flag to install needed devDependencies as well.
* Enable SourceLink
* NuGet Package License Information updated (deprecation of licenseUrl)

ElectronNET.API:

* Documentation added for WebContents.GetUrl()
* Enable SourceLink
* NuGet Package License Information updated (deprecation of licenseUrl)

# 0.0.10

ElectronNET.API:

* manifestJsonFilePath fixed (thanks @smack0007)
* Use Electron release 3.0.0 and updated packages (thanks @deinok)
* fixes for Socket interaction (thanks @mojinxun)
* Fixing SingleInstances (thanks @yaofeng)
* Enhance WebContent.GetUrl (thanks @ru-sh)

ElectronNET.CLI:

* Show Resultcode for better debugging when using Build/Start Command
* ElectronNET.CLI is now a global dotnet tool

# 0.0.9

ElectronNET.API:

* Better Async handling - thanks @danielmarbach

ElectronNET.CLI:

* More options on the 'build' command, e.g. for a 32bit debug build with electron prune: build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params "--prune=true "
* .NET Core project is now built with Release configuration but can be adjusted with the new params.
* Be aware: "Breaking" (but because of the alpha status of this project, we won't use SemVer)

# 0.0.8

This version was skipped because we unfortunately  released a pre-version of this on NuGet.

# 0.0.7

ElectronNET.CLI:

* Fixed electronize start for macos/linux - thanks @yamachu
* Skip NPM install on start when node_modules directory already exists

# 0.0.6

ElectronNET.CLI:

* nuget packages are now release bits and have the correct assembly version
* Version command
* better devCleanup.cmd
* Better Platform Support Issue - thanks to @Petermarcu
* Start Command should now work on OSX/Linux - thanks to @r105m

ElectronNET.API:

* Thread-Safe methods - thanks to @yeskunall

# 0.0.5

ElectronNET.API:

* The last nuget package didn't contain the actual webpreferences settings with defaults - hopefully now.

# 0.0.4

ElectronNET.CLI:

* dotnet electronize start fixed

ElectronNET.API:

* WebPreferences settings with default values

# 0.0.3

ElectronNET.CLI:
* Init with Debug profile
* Build for all platforms (well... for newest OSX/Linux/Windows)

ElectronNET.API:
* Moar XML documentation
* Hybrid support (e.g. running as normal website and electron app)
* Event bugfixing

# 0.0.2

ElectronNET.CLI:
* Added Init to Help page
* Added XML documentation to NuGet output
* Maybe fixed for https://github.com/GregorBiswanger/Electron.NET/issues/2

ElectronNET.API:
* Add XML documentation to NuGet output
* Implemented Notification-, Dialog- & Tray-API

# 0.0.1

* init everything and basic functionality
