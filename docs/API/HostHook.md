# Electron.HostHook

Execute native JavaScript/TypeScript code from the host process.

## Overview

The `Electron.HostHook` API allows you to execute native JavaScript/TypeScript code from the host process. This enables advanced integration scenarios where you need to run custom JavaScript code or access Node.js APIs directly.

## Methods

#### 🧊 `void Call(string socketEventName, params dynamic[] arguments)`
Execute native JavaScript/TypeScript code synchronously.

**Parameters:**
- `socketEventName` - Socket name registered on the host
- `arguments` - Optional parameters

#### 🧊 `Task<T> CallAsync<T>(string socketEventName, params dynamic[] arguments)`
Execute native JavaScript/TypeScript code asynchronously with type-safe return values.

**Parameters:**
- `T` - Expected return type
- `socketEventName` - Socket name registered on the host
- `arguments` - Optional parameters

**Returns:**

Task<T> with the result from the executed host code.

## Usage Examples

### Basic Host Hook Execution

```csharp
// Execute simple JavaScript function
Electron.HostHook.Call("myFunction", "parameter1", 42);

// Execute with callback-style result
var result = await Electron.HostHook.CallAsync<string>("getUserName", userId);
Console.WriteLine($"User name: {result}");
```

### Advanced Integration

```csharp
// Call custom Electron API
var fileContent = await Electron.HostHook.CallAsync<string>("readFile", "config.json");
Console.WriteLine($"Config: {fileContent}");

// Execute with multiple parameters
var processedData = await Electron.HostHook.CallAsync<object[]>("processData", rawData, options);

// Call with complex objects
var settings = new { theme = "dark", language = "en" };
var updatedSettings = await Electron.HostHook.CallAsync<object>("updateSettings", settings);
```

### Error Handling

```csharp
try
{
    // Execute host function with error handling
    var result = await Electron.HostHook.CallAsync<string>("riskyOperation", inputData);
    Console.WriteLine($"Success: {result}");
}
catch (Exception ex)
{
    // Handle execution errors
    Console.WriteLine($"Host hook error: {ex.Message}");
    Electron.Dialog.ShowErrorBox("Operation Failed", "Could not execute host function.");
}
```

### Type-Safe Operations

```csharp
// Strongly typed return values
var userInfo = await Electron.HostHook.CallAsync<UserInfo>("getUserInfo", userId);
Console.WriteLine($"User: {userInfo.Name}, Email: {userInfo.Email}");

// Array results
var fileList = await Electron.HostHook.CallAsync<string[]>("listFiles", directoryPath);
foreach (var file in fileList)
{
    Console.WriteLine($"File: {file}");
}

// Complex object results
var systemStats = await Electron.HostHook.CallAsync<SystemStatistics>("getSystemStats");
Console.WriteLine($"CPU: {systemStats.CpuUsage}%, Memory: {systemStats.MemoryUsage}%");
```

### Custom ElectronHostHook Setup

```csharp
// In your ElectronHostHook/index.ts
import { app } from 'electron';

export function getAppVersion(): string {
    return app.getVersion();
}

export async function readConfigFile(): Promise<string> {
    const fs = require('fs').promises;
    return await fs.readFile('config.json', 'utf8');
}

export function customNotification(message: string): void {
    // Custom notification logic
    console.log(`Custom notification: ${message}`);
}
```

### Integration with .NET Code

```csharp
// Use host hook in your application logic
public async Task<string> GetApplicationVersion()
{
    return await Electron.HostHook.CallAsync<string>("getAppVersion");
}

public async Task LoadConfiguration()
{
    try
    {
        var config = await Electron.HostHook.CallAsync<ConfigObject>("readConfigFile");
        ApplyConfiguration(config);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to load config: {ex.Message}");
        UseDefaultConfiguration();
    }
}

public void ShowCustomNotification(string message)
{
    Electron.HostHook.Call("customNotification", message);
}
```

## Related APIs

- [Electron.IpcMain](IpcMain.md) - Inter-process communication
- [Electron.App](App.md) - Application lifecycle events
- [Electron.WebContents](WebContents.md) - Web content integration

## Additional Resources

- [Host Hook Documentation](../Core/Advanced-Migration-Topics.md) - Setting up custom host hooks
