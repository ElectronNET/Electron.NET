# Electron.App

Control your application's event lifecycle.

## Overview

The `Electron.App` API provides comprehensive control over your application's lifecycle, including startup, shutdown, window management, and system integration. It handles application-level events and provides methods for managing the overall application state.

## Properties

#### 📋 `CommandLine`
A `CommandLine` object that allows you to read and manipulate the command line arguments that Chromium uses.

#### 📋 `IsReady`
Application host fully started.

#### 📋 `Name`
A string property that indicates the current application's name, which is the name in the application's package.json file.

Usually the name field of package.json is a short lowercase name, according to the npm modules spec. You should usually also specify a productName field, which is your application's full capitalized name, and which will be preferred over name by Electron.

#### 📋 `NameAsync`
A `Task<string>` property that indicates the current application's name, which is the name in the application's package.json file.

Usually the name field of package.json is a short lowercase name, according to the npm modules spec. You should usually also specify a productName field, which is your application's full capitalized name, and which will be preferred over name by Electron.

#### 📋 `UserAgentFallback`
A string which is the user agent string Electron will use as a global fallback.

This is the user agent that will be used when no user agent is set at the webContents or session level. It is useful for ensuring that your entire app has the same user agent. Set to a custom value as early as possible in your app's initialization to ensure that your overridden value is used.

#### 📋 `UserAgentFallbackAsync`
A `Task<string>` which is the user agent string Electron will use as a global fallback.

This is the user agent that will be used when no user agent is set at the webContents or session level. It is useful for ensuring that your entire app has the same user agent. Set to a custom value as early as possible in your app's initialization to ensure that your overridden value is used.

## Methods

#### 🧊 `void AddRecentDocument(string path)`
Adds path to the recent documents list. This list is managed by the OS. On Windows you can visit the list from the task bar, and on macOS you can visit it from dock menu.

#### 🧊 `void ClearRecentDocuments()`
Clears the recent documents list.

#### 🧊 `void Exit(int exitCode = 0)`
All windows will be closed immediately without asking user and the BeforeQuit and WillQuit events will not be emitted.

**Parameters:**
- `exitCode` - Exits immediately with exitCode. exitCode defaults to 0.

#### 🧊 `void Focus()`
On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses on the application's first window.

#### 🧊 `void Focus(FocusOptions focusOptions)`
On Linux, focuses on the first visible window. On macOS, makes the application the active app. On Windows, focuses on the application's first window.

You should seek to use the `FocusOptions.Steal` option as sparingly as possible.

**Parameters:**
- `focusOptions` - Focus options

#### 🧊 `Task<ProcessMetric[]> GetAppMetricsAsync(CancellationToken cancellationToken = default)`
Memory and cpu usage statistics of all the processes associated with the app.

**Returns:**

Array of ProcessMetric objects that correspond to memory and cpu usage statistics of all the processes associated with the app.

#### 🧊 `Task<string> GetAppPathAsync(CancellationToken cancellationToken = default)`
The current application directory.

**Returns:**

The current application directory.

#### 🧊 `Task<int> GetBadgeCountAsync(CancellationToken cancellationToken = default)`
The current value displayed in the counter badge.

**Returns:**

The current value displayed in the counter badge.

#### 🧊 `Task<string> GetCurrentActivityTypeAsync(CancellationToken cancellationToken = default)`
The type of the currently running activity.

**Returns:**

The type of the currently running activity.

#### 🧊 `Task<GPUFeatureStatus> GetGpuFeatureStatusAsync(CancellationToken cancellationToken = default)`
The Graphics Feature Status from chrome://gpu/.

Note: This information is only usable after the gpu-info-update event is emitted.

**Returns:**

The Graphics Feature Status from chrome://gpu/.

#### 🧊 `Task<JumpListSettings> GetJumpListSettingsAsync(CancellationToken cancellationToken = default)`
Jump List settings for the application.

**Returns:**

Jump List settings.

#### 🧊 `Task<string> GetLocaleAsync(CancellationToken cancellationToken = default)`
The current application locale. Possible return values are documented [here](https://www.electronjs.org/docs/api/locales).

Note: When distributing your packaged app, you have to also ship the locales folder.

Note: On Windows, you have to call it after the Ready events gets emitted.

**Returns:**

The current application locale.

#### 🧊 `Task<LoginItemSettings> GetLoginItemSettingsAsync(CancellationToken cancellationToken = default)`
If you provided path and args options to SetLoginItemSettings then you need to pass the same arguments here for LoginItemSettings.OpenAtLogin to be set correctly.

**Returns:**

Login item settings.

#### 🧊 `Task<string> GetPathAsync(PathName pathName, CancellationToken cancellationToken = default)`
The path to a special directory. If GetPathAsync is called without called SetAppLogsPath being called first, a default directory will be created equivalent to calling SetAppLogsPath without a path parameter.

**Parameters:**
- `pathName` - Special directory.

**Returns:**

A path to a special directory or file associated with name.

#### 🧊 `Task<string> GetVersionAsync(CancellationToken cancellationToken = default)`
The version of the loaded application. If no version is found in the application's package.json file, the version of the current bundle or executable is returned.

**Returns:**

The version of the loaded application.

#### 🧊 `Task<bool> HasSingleInstanceLockAsync(CancellationToken cancellationToken = default)`
This method returns whether or not this instance of your app is currently holding the single instance lock. You can request the lock with RequestSingleInstanceLockAsync and release with ReleaseSingleInstanceLock.

**Returns:**

Whether this instance of your app is currently holding the single instance lock.

#### 🧊 `void Hide()`
Hides all application windows without minimizing them.

#### 🧊 `Task<int> ImportCertificateAsync(ImportCertificateOptions options, CancellationToken cancellationToken = default)`
Imports the certificate in pkcs12 format into the platform certificate store. callback is called with the result of import operation, a value of 0 indicates success while any other value indicates failure according to chromium net_error_list.

**Parameters:**
- `options` - Import certificate options
- `cancellationToken` - The cancellation token

**Returns:**

Result of import. Value of 0 indicates success.

#### 🧊 `void InvalidateCurrentActivity()`
Invalidates the current Handoff user activity.

#### 🧊 `Task<bool> IsAccessibilitySupportEnabledAsync(CancellationToken cancellationToken = default)`
true if Chrome's accessibility support is enabled, false otherwise. This API will return true if the use of assistive technologies, such as screen readers, has been detected. See Chromium's accessibility docs for more details.

**Returns:**

true if Chrome's accessibility support is enabled, false otherwise.

#### 🧊 `Task<bool> IsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)`
This method checks if the current executable is the default handler for a protocol (aka URI scheme).

Note: On macOS, you can use this method to check if the app has been registered as the default protocol handler for a protocol. You can also verify this by checking ~/Library/Preferences/com.apple.LaunchServices.plist on the macOS machine. Please refer to Apple's documentation for details.

The API uses the Windows Registry and LSCopyDefaultHandlerForURLScheme internally.

**Parameters:**
- `protocol` - The name of your protocol, without ://
- `cancellationToken` - The cancellation token

**Returns:**

Whether the current executable is the default handler for a protocol (aka URI scheme).

#### 🧊 `Task<bool> IsUnityRunningAsync(CancellationToken cancellationToken = default)`
Whether the current desktop environment is Unity launcher.

**Returns:**

Whether the current desktop environment is Unity launcher.

#### 🧊 `void Quit()`
Try to close all windows. The BeforeQuit event will be emitted first. If all windows are successfully closed, the WillQuit event will be emitted and by default the application will terminate. This method guarantees that all beforeunload and unload event handlers are correctly executed. It is possible that a window cancels the quitting by returning false in the beforeunload event handler.

#### 🧊 `void ReleaseSingleInstanceLock()`
Releases all locks that were created by makeSingleInstance. This will allow multiple instances of the application to once again run side by side.

#### 🧊 `void Relaunch()`
Relaunches the app when current instance exits. By default the new instance will use the same working directory and command line arguments with current instance.

Note that this method does not quit the app when executed, you have to call Quit or Exit after calling Relaunch() to make the app restart.

When Relaunch() is called for multiple times, multiple instances will be started after current instance exited.

#### 🧊 `void Relaunch(RelaunchOptions relaunchOptions)`
Relaunches the app when current instance exits. By default the new instance will use the same working directory and command line arguments with current instance. When RelaunchOptions.Args is specified, the RelaunchOptions.Args will be passed as command line arguments instead. When RelaunchOptions.ExecPath is specified, the RelaunchOptions.ExecPath will be executed for relaunch instead of current app.

Note that this method does not quit the app when executed, you have to call Quit or Exit after calling Relaunch() to make the app restart.

When Relaunch() is called for multiple times, multiple instances will be started after current instance exited.

**Parameters:**
- `relaunchOptions` - Options for the relaunch

#### 🧊 `Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)`
This method checks if the current executable as the default handler for a protocol (aka URI scheme). If so, it will remove the app as the default handler.

**Parameters:**
- `protocol` - The name of your protocol, without ://
- `cancellationToken` - The cancellation token

**Returns:**

Whether the call succeeded.

#### 🧊 `Task<bool> RequestSingleInstanceLockAsync(Action<string[], string> newInstanceOpened, CancellationToken cancellationToken = default)`
The return value of this method indicates whether or not this instance of your application successfully obtained the lock. If it failed to obtain the lock, you can assume that another instance of your application is already running with the lock and exit immediately.

I.e.This method returns true if your process is the primary instance of your application and your app should continue loading. It returns false if your process should immediately quit as it has sent its parameters to another instance that has already acquired the lock.

On macOS, the system enforces single instance automatically when users try to open a second instance of your app in Finder, and the open-file and open-url events will be emitted for that.However when users start your app in command line, the system's single instance mechanism will be bypassed, and you have to use this method to ensure single instance.

**Parameters:**
- `newInstanceOpened` - Lambda with an array of the second instance's command line arguments. The second parameter is the working directory path.
- `cancellationToken` - The cancellation token

**Returns:**

This method returns false if your process is the primary instance of the application and your app should continue loading. And returns true if your process has sent its parameters to another instance, and you should immediately quit.

#### 🧊 `void ResignCurrentActivity()`
Marks the current Handoff user activity as inactive without invalidating it.

#### 🧊 `void SetAccessibilitySupportEnabled(bool enabled)`
Manually enables Chrome's accessibility support, allowing to expose accessibility switch to users in application settings. See Chromium's accessibility docs for more details. Disabled (false) by default.

This API must be called after the Ready event is emitted.

Note: Rendering accessibility tree can significantly affect the performance of your app. It should not be enabled by default.

**Parameters:**
- `enabled` - Enable or disable accessibility tree rendering

#### 🧊 `void SetAppLogsPath(string path)`
Sets or creates a directory your app's logs which can then be manipulated with GetPathAsync or SetPath.

Calling SetAppLogsPath without a path parameter will result in this directory being set to ~/Library/Logs/YourAppName on macOS, and inside the userData directory on Linux and Windows.

**Parameters:**
- `path` - A custom path for your logs. Must be absolute

#### 🧊 `void SetAppUserModelId(string id)`
Changes the Application User Model ID to id.

**Parameters:**
- `id` - Model Id

#### 🧊 `Task<bool> SetAsDefaultProtocolClientAsync(string protocol, CancellationToken cancellationToken = default)`
Sets the current executable as the default handler for a protocol (aka URI scheme). It allows you to integrate your app deeper into the operating system. Once registered, all links with your-protocol:// will be opened with the current executable. The whole link, including protocol, will be passed to your application as a parameter.

Note: On macOS, you can only register protocols that have been added to your app's info.plist, which cannot be modified at runtime. However, you can change the file during build time via Electron Forge, Electron Packager, or by editing info.plist with a text editor. Please refer to Apple's documentation for details.

Note: In a Windows Store environment (when packaged as an appx) this API will return true for all calls but the registry key it sets won't be accessible by other applications. In order to register your Windows Store application as a default protocol handler you must declare the protocol in your manifest.

The API uses the Windows Registry and LSSetDefaultHandlerForURLScheme internally.

**Parameters:**
- `protocol` - The name of your protocol, without ://. For example, if you want your app to handle electron:// links, call this method with electron as the parameter.
- `cancellationToken` - The cancellation token

**Returns:**

Whether the call succeeded.

#### 🧊 `Task<bool> SetBadgeCountAsync(int count, CancellationToken cancellationToken = default)`
Sets the counter badge for current app. Setting the count to 0 will hide the badge. On macOS it shows on the dock icon. On Linux it only works for Unity launcher.

Note: Unity launcher requires the existence of a .desktop file to work, for more information please read Desktop Environment Integration.

**Parameters:**
- `count` - Counter badge
- `cancellationToken` - The cancellation token

**Returns:**

Whether the call succeeded.

#### 🧊 `void SetJumpList(JumpListCategory[] categories)`
Sets or removes a custom Jump List for the application. If categories is null the previously set custom Jump List (if any) will be replaced by the standard Jump List for the app (managed by Windows).

Note: If a JumpListCategory object has neither the Type nor the Name property set then its Type is assumed to be tasks. If the Name property is set but the Type property is omitted then the Type is assumed to be custom.

Note: Users can remove items from custom categories, and Windows will not allow a removed item to be added back into a custom category until after the next successful call to SetJumpList. Any attempt to re-add a removed item to a custom category earlier than that will result in the entire custom category being omitted from the Jump List. The list of removed items can be obtained using GetJumpListSettingsAsync.

**Parameters:**
- `categories` - Array of JumpListCategory objects

#### 🧊 `void SetLoginItemSettings(LoginSettings loginSettings)`
Set the app's login item settings.

To work with Electron's autoUpdater on Windows, which uses Squirrel, you'll want to set the launch path to Update.exe, and pass arguments that specify your application name.

**Parameters:**
- `loginSettings` - Login settings

#### 🧊 `void SetPath(PathName name, string path)`
Overrides the path to a special directory or file associated with name. If the path specifies a directory that does not exist, an Error is thrown. In that case, the directory should be created with fs.mkdirSync or similar.

You can only override paths of a name defined in GetPathAsync.

By default, web pages' cookies and caches will be stored under the PathName.UserData directory. If you want to change this location, you have to override the PathName.UserData path before the Ready event of the App module is emitted.

**Parameters:**
- `name` - Special directory name
- `path` - New path to a special directory

#### 🧊 `void SetUserActivity(string type, object userInfo)`
Creates an NSUserActivity and sets it as the current activity. The activity is eligible for Handoff to another device afterward.

**Parameters:**
- `type` - Uniquely identifies the activity. Maps to NSUserActivity.activityType.
- `userInfo` - App-specific state to store for use by another device

#### 🧊 `Task<bool> SetUserTasksAsync(UserTask[] userTasks, CancellationToken cancellationToken = default)`
Adds tasks to the UserTask category of the JumpList on Windows.

Note: If you'd like to customize the Jump List even more use SetJumpList instead.

**Parameters:**
- `userTasks` - Array of UserTask objects
- `cancellationToken` - The cancellation token

**Returns:**

Whether the call succeeded.

#### 🧊 `void Show()`
Shows application windows after they were hidden. Does not automatically focus them.

#### 🧊 `void ShowAboutPanel()`
Show the app's about panel options. These options can be overridden with SetAboutPanelOptions.

## Events

#### ⚡ `AccessibilitySupportChanged`
Emitted when Chrome's accessibility support changes. This event fires when assistive technologies, such as screen readers, are enabled or disabled. See https://www.chromium.org/developers/design-documents/accessibility for more details.

#### ⚡ `BrowserWindowBlur`
Emitted when a BrowserWindow blurred.

#### ⚡ `BrowserWindowCreated`
Emitted when a new BrowserWindow is created.

#### ⚡ `BrowserWindowFocus`
Emitted when a BrowserWindow gets focused.

#### ⚡ `OpenFile`
Emitted when a macOS user wants to open a file with the application. The open-file event is usually emitted when the application is already open and the OS wants to reuse the application to open the file. open-file is also emitted when a file is dropped onto the dock and the application is not yet running.

On Windows, you have to parse the arguments using App.CommandLine to get the filepath.

#### ⚡ `OpenUrl`
Emitted when a macOS user wants to open a URL with the application. Your application's Info.plist file must define the URL scheme within the CFBundleURLTypes key, and set NSPrincipalClass to AtomApplication.

#### ⚡ `Quitting`
Emitted when the application is quitting.

Note: On Windows, this event will not be emitted if the app is closed due to a shutdown/restart of the system or a user logout.

#### ⚡ `Ready`
Emitted when the application has finished basic startup.

#### ⚡ `WebContentsCreated`
Emitted when a new WebContents is created.

#### ⚡ `WillQuit`
Emitted when all windows have been closed and the application will quit.

See the description of the WindowAllClosed event for the differences between the WillQuit and WindowAllClosed events.

Note: On Windows, this event will not be emitted if the app is closed due to a shutdown/restart of the system or a user logout.

#### ⚡ `WindowAllClosed`
Emitted when all windows have been closed.

If you do not subscribe to this event and all windows are closed, the default behavior is to quit the app; however, if you subscribe, you control whether the app quits or not.If the user pressed Cmd + Q, or the developer called Quit, Electron will first try to close all the windows and then emit the WillQuit event, and in this case the WindowAllClosed event would not be emitted.

## Usage Examples

### Application Lifecycle

```csharp
// Handle app startup
Electron.App.Ready += () =>
{
    Console.WriteLine("App is ready!");
};

// Handle window management
Electron.App.WindowAllClosed += () =>
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        Electron.App.Quit();
    }
};

// Prevent quit
Electron.App.BeforeQuit += async (args) =>
{
    var result = await Electron.Dialog.ShowMessageBoxAsync("Do you want to quit?");
    if (result.Response == 1) // Cancel
    {
        args.PreventDefault = true;
    }
};
```

### Protocol Handling

```csharp
// Register custom protocol
var success = await Electron.App.SetAsDefaultProtocolClientAsync("myapp");

// Check if registered
var isDefault = await Electron.App.IsDefaultProtocolClientAsync("myapp");
```

### Jump List (Windows)

```csharp
// Set user tasks
await Electron.App.SetUserTasksAsync(new[]
{
    new UserTask
    {
        Program = "myapp.exe",
        Arguments = "--new-document",
        Title = "New Document",
        Description = "Create a new document"
    }
});
```

### Application Information

```csharp
// Get app information
var appPath = await Electron.App.GetAppPathAsync();
var version = await Electron.App.GetVersionAsync();
var locale = await Electron.App.GetLocaleAsync();

// Set app name
await Electron.App.NameAsync; // Get current name
Electron.App.Name = "My Custom App Name";
```

### Badge Count (macOS/Linux)

```csharp
// Set badge count
await Electron.App.SetBadgeCountAsync(5);

// Get current badge count
var count = await Electron.App.GetBadgeCountAsync();
```

## Related APIs

- [Electron.WindowManager](WindowManager.md) - Window creation and management
- [Electron.Dialog](Dialog.md) - User interaction dialogs
- [Electron.Menu](Menu.md) - Application menus

## Additional Resources

- [Electron App Documentation](https://electronjs.org/docs/api/app) - Official Electron app API
- [Startup Methods](../Using/Startup-Methods.md) - Different application startup modes
