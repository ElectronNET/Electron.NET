# not release

# 0.0.9

ElectronNET.API:

* Better Async handling - thanks @danielmarbach

ElectronNET.CLI:

* More options on the 'build' command, e.g. for a 32bit debug build with electron prune: build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params "--prune=true "
* .NET Core project is now build with Release configuration, but can be adjusted with the new params.
* Be aware: "Breaking" (but because of the alpha status of this project, we won't use SemVer)

# 0.0.8

This version was skipped because we unfortunatly released a pre version of this on NuGet.

# 0.0.7

ElectronNET.CLI:

* Fixed electronize start for macos/linux - thanks @yamachu
* Skip NPM install on start when node_modules directory already exists

# 0.0.6

ElectronNET.CLI:

* nuget packages are now release bits and have the correct assembly verion
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
