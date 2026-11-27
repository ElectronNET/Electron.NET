namespace ElectronNET.IntegrationTests.Tests;

using System.Net;
using System.Net.Sockets;
using System.Reflection;
using ElectronNET.Runtime.Data;
using ElectronNET.Runtime.Services.SocketBridge;

/// <summary>
/// Lifecycle checks for <see cref="SocketBridgeService"/> when the bridge never connects.
/// This happens when Electron exits before the bridge connects, e.g. a second instance that loses the
/// single-instance lock: the runtime controller stops the bridge and waits for it to reach Stopped.
/// No Electron runtime is required for these tests.
/// </summary>
public class SocketBridgeServiceLifecycleTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    public SocketBridgeServiceLifecycleTests()
    {
        // SocketBridgeService reads ElectronNetRuntime, whose static initializer gathers the build info from the
        // test assembly under testhost (same as ElectronFixture); it has to be registered before first access.
        AppDomain.CurrentDomain.SetData("ElectronTestAssembly", Assembly.GetExecutingAssembly());
    }

    [Fact]
    public async Task Stop_WhenNeverConnected_TransitionsToStopped()
    {
        var bridge = new SocketBridgeService(GetClosedLoopbackPort(), "token");

        await bridge.Start();
        await bridge.Stop().WaitAsync(Timeout);

        bridge.State.Should().Be(LifetimeState.Stopped);
        bridge.WaitStoppingTask.IsCompleted.Should().BeTrue();
        bridge.WaitStoppedTask.IsCompleted.Should().BeTrue(
            "a socket that never connected never raises BridgeDisconnected, so Stop() itself must complete the lifecycle");
    }

    [Fact]
    public async Task Stop_WhenNeverConnected_RaisesStoppingThenStoppedOnce()
    {
        var bridge = new SocketBridgeService(GetClosedLoopbackPort(), "token");
        var events = new List<string>();
        bridge.Stopping += (_, _) => { lock (events) events.Add(nameof(bridge.Stopping)); };
        bridge.Stopped += (_, _) => { lock (events) events.Add(nameof(bridge.Stopped)); };
        bridge.Ready += (_, _) => { lock (events) events.Add(nameof(bridge.Ready)); };

        await bridge.Start();
        await bridge.Stop().WaitAsync(Timeout);

        lock (events)
        {
            events.Should().Equal(nameof(bridge.Stopping), nameof(bridge.Stopped));
        }
    }

    [Fact]
    public async Task Stop_WhenNeverConnected_IsNotRevivedByThePendingConnectAttempt()
    {
        var bridge = new SocketBridgeService(GetClosedLoopbackPort(), "token");

        await bridge.Start();
        await bridge.Stop().WaitAsync(Timeout);

        // The connect attempt started by Start() runs in the background; once it gives up it must not move the
        // service out of Stopped (backward transitions would throw on the thread pool).
        await Task.Delay(TimeSpan.FromSeconds(1));
        bridge.State.Should().Be(LifetimeState.Stopped);
    }

    private static int GetClosedLoopbackPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
