# Electron.PowerMonitor

Monitor system power events like sleep, wake, and battery status.

## Overview

The `Electron.PowerMonitor` API provides access to system power events and state changes. This includes monitoring when the system is going to sleep, waking up, or changing power sources.

## Events

#### ⚡ `OnAC`
Emitted when the system changes to AC power.

#### ⚡ `OnBattery`
Emitted when system changes to battery power.

#### ⚡ `OnLockScreen`
Emitted when the system is about to lock the screen.

#### ⚡ `OnResume`
Emitted when system is resuming.

#### ⚡ `OnShutdown`
Emitted when the system is about to reboot or shut down.

#### ⚡ `OnSuspend`
Emitted when the system is suspending.

#### ⚡ `OnUnLockScreen`
Emitted when the system is about to unlock the screen.

## Usage Examples

### Basic Power Event Monitoring

```csharp
// Monitor system sleep/wake
Electron.PowerMonitor.OnSuspend += () =>
{
    Console.WriteLine("System going to sleep");
    // Save application state
    SaveApplicationState();
};

Electron.PowerMonitor.OnResume += () =>
{
    Console.WriteLine("System waking up");
    // Restore application state
    RestoreApplicationState();
};
```

### Screen Lock/Unlock Monitoring

```csharp
// Handle screen lock events
Electron.PowerMonitor.OnLockScreen += () =>
{
    Console.WriteLine("Screen locking");
    // Pause real-time operations
    PauseRealTimeOperations();
};

Electron.PowerMonitor.OnUnLockScreen += () =>
{
    Console.WriteLine("Screen unlocking");
    // Resume real-time operations
    ResumeRealTimeOperations();
};
```

### Power Source Changes

```csharp
// Monitor power source changes
Electron.PowerMonitor.OnAC += () =>
{
    Console.WriteLine("Switched to AC power");
    // Adjust power-intensive operations
    EnablePowerIntensiveFeatures();
};

Electron.PowerMonitor.OnBattery += () =>
{
    Console.WriteLine("Switched to battery power");
    // Reduce power consumption
    EnablePowerSavingMode();
};
```

### System Shutdown Handling

```csharp
// Handle system shutdown
Electron.PowerMonitor.OnShutdown += () =>
{
    Console.WriteLine("System shutting down");
    // Save critical data and exit gracefully
    SaveAndExit();
};
```

### Application State Management

```csharp
private bool isSuspended = false;

public void InitializePowerMonitoring()
{
    // Track suspension state
    Electron.PowerMonitor.OnSuspend += () =>
    {
        isSuspended = true;
        OnSystemSleep();
    };

    Electron.PowerMonitor.OnResume += () =>
    {
        isSuspended = false;
        OnSystemWake();
    };

    // Handle screen lock for security
    Electron.PowerMonitor.OnLockScreen += () =>
    {
        OnScreenLocked();
    };
}

private void OnSystemSleep()
{
    // Pause network operations
    PauseNetworkOperations();

    // Save unsaved work
    AutoSaveDocuments();

    // Reduce resource usage
    MinimizeResourceUsage();
}

private void OnSystemWake()
{
    // Resume network operations
    ResumeNetworkOperations();

    // Check for updates
    CheckForUpdates();

    // Restore full functionality
    RestoreFullFunctionality();
}

private void OnScreenLocked()
{
    // Hide sensitive information
    HideSensitiveData();

    // Pause real-time features
    PauseRealTimeFeatures();
}
```

### Battery Status Monitoring

```csharp
// Monitor battery status changes
Electron.PowerMonitor.OnAC += () =>
{
    Console.WriteLine("Plugged in - full performance mode");
    EnableFullPerformanceMode();
};

Electron.PowerMonitor.OnBattery += () =>
{
    Console.WriteLine("On battery - power saving mode");
    EnablePowerSavingMode();
};
```

## Related APIs

- [Electron.App](App.md) - Application lifecycle events
- [Electron.Notification](Notification.md) - Notify users about power events

## Additional Resources

- [Electron PowerMonitor Documentation](https://electronjs.org/docs/api/power-monitor) - Official Electron power monitor API
