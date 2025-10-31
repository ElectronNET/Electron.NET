# Electron.IpcMain

Communicate asynchronously from the main process to renderer processes.

## Overview

The `Electron.IpcMain` API provides inter-process communication between the main process and renderer processes. It allows you to send messages, listen for events, and handle communication between different parts of your Electron application.

## Methods

#### 🧊 `Task On(string channel, Action<object> listener)`
Listens to channel, when a new message arrives listener would be called with listener(event, args...).

**Parameters:**
- `channel` - Channel name to listen on
- `listener` - Callback method to handle incoming messages

#### 🧊 `void OnSync(string channel, Func<object, object> listener)`
Send a message to the renderer process synchronously via channel. Note: Sending a synchronous message will block the whole renderer process.

**Parameters:**
- `channel` - Channel name to listen on
- `listener` - Synchronous callback method

#### 🧊 `void Once(string channel, Action<object> listener)`
Adds a one time listener method for the event. This listener is invoked only the next time a message is sent to channel, after which it is removed.

**Parameters:**
- `channel` - Channel name to listen on
- `listener` - Callback method to handle the message once

#### 🧊 `void RemoveAllListeners(string channel)`
Removes all listeners of the specified channel.

**Parameters:**
- `channel` - Channel name to remove listeners from

#### 🧊 `void Send(BrowserView browserView, string channel, params object[] data)`
Send a message to the BrowserView renderer process asynchronously via channel.

**Parameters:**
- `browserView` - Target browser view
- `channel` - Channel name to send on
- `data` - Arguments to send

#### 🧊 `void Send(BrowserWindow browserWindow, string channel, params object[] data)`
Send a message to the renderer process asynchronously via channel.

**Parameters:**
- `browserWindow` - Target browser window
- `channel` - Channel name to send on
- `data` - Arguments to send

## Usage Examples

### Basic Message Handling

```csharp
// Listen for messages from renderer
await Electron.IpcMain.On("request-data", (args) =>
{
    Console.WriteLine($"Received request: {args}");
    // Process the request and send response
});

// Send response back to renderer
Electron.IpcMain.Send(mainWindow, "data-response", processedData);
```

### Synchronous Communication

```csharp
// Handle synchronous requests
Electron.IpcMain.OnSync("get-user-info", (request) =>
{
    var userId = request.ToString();
    var userInfo = GetUserInfo(userId);
    return userInfo;
});
```

### One-time Event Handling

```csharp
// Handle initialization request once
Electron.IpcMain.Once("app-initialized", (args) =>
{
    Console.WriteLine("App initialized, setting up...");
    InitializeApplication();
});
```

### Complex Data Exchange

```csharp
// Send complex data to renderer
var appData = new
{
    Version = "1.0.0",
    Features = new[] { "feature1", "feature2" },
    Settings = new { Theme = "dark", Language = "en" }
};

Electron.IpcMain.Send(mainWindow, "app-config", appData);

// Listen for settings updates
await Electron.IpcMain.On("update-settings", (settings) =>
{
    var config = JsonConvert.DeserializeObject<AppSettings>(settings.ToString());
    ApplySettings(config);
});
```

### Multi-Window Communication

```csharp
// Send message to specific window
var settingsWindow = await Electron.WindowManager.CreateWindowAsync();
Electron.IpcMain.Send(settingsWindow, "show-settings", currentSettings);

// Broadcast to all windows
foreach (var window in Electron.WindowManager.BrowserWindows)
{
    Electron.IpcMain.Send(window, "notification", message);
}
```

### Error Handling

```csharp
// Handle IPC errors gracefully
await Electron.IpcMain.On("risky-operation", async (args) =>
{
    try
    {
        var result = await PerformRiskyOperation(args);
        Electron.IpcMain.Send(mainWindow, "operation-success", result);
    }
    catch (Exception ex)
    {
        Electron.IpcMain.Send(mainWindow, "operation-error", ex.Message);
    }
});
```

### Integration with Host Hooks

```csharp
// Use with custom host functionality
await Electron.IpcMain.On("execute-host-function", async (args) =>
{
    var functionName = args.ToString();
    var result = await Electron.HostHook.CallAsync<string>(functionName);

    Electron.IpcMain.Send(mainWindow, "function-result", result);
});
```

## Related APIs

- [Electron.HostHook](HostHook.md) - Execute custom JavaScript functions
- [Electron.WindowManager](WindowManager.md) - Target specific windows for communication
- [Electron.WebContents](WebContents.md) - Send messages to web content

## Additional Resources

- [Electron IPC Documentation](https://electronjs.org/docs/api/ipc-main) - Official Electron IPC API
