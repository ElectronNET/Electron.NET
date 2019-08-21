# Not released

# 5.22.15

ElectronNET.API:

* New Feature: Add BrowserWindow.RemoveMenu() (thanks [hack2root](https://github.com/hack2root))

# Released

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
