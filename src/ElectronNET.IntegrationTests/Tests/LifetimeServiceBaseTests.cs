namespace ElectronNET.IntegrationTests.Tests;

using ElectronNET.Runtime.Data;
using ElectronNET.Runtime.Services;

/// <summary>
/// State machine checks for <see cref="LifetimeServiceBase"/>, which the runtime controller and its
/// services (Electron process, socket bridge) build on.
/// No Electron runtime is required for these tests.
/// </summary>
public class LifetimeServiceBaseTests
{
    [Fact]
    public async Task Start_TransitionsToStarting()
    {
        var service = new TestLifetimeService();

        await service.Start();

        service.State.Should().Be(LifetimeState.Starting);
        service.WaitStartedTask.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Stop_TransitionsToStoppingWithoutCompletingStopped()
    {
        var service = new TestLifetimeService();
        await service.Start();

        await service.Stop();

        service.State.Should().Be(LifetimeState.Stopping);
        service.WaitStoppingTask.IsCompleted.Should().BeTrue();
        service.WaitStoppedTask.IsCompleted.Should().BeFalse(
            "reaching Stopped is up to the concrete service once it has actually shut down");
    }

    [Fact]
    public async Task Stopped_BeforeReady_CompletesEveryEarlierWaitTask()
    {
        var service = new TestLifetimeService();
        await service.Start();

        service.Transition(LifetimeState.Stopped);

        service.WaitStartedTask.IsCompleted.Should().BeTrue();
        service.WaitReadyTask.IsCompleted.Should().BeTrue(
            "callers awaiting readiness must not hang when the service stops before it ever became ready");
        service.WaitStoppingTask.IsCompleted.Should().BeTrue();
        service.WaitStoppedTask.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Stopped_RaisesOnlyTheStoppedEvent()
    {
        var service = new TestLifetimeService();
        var events = new List<string>();
        service.Started += (_, _) => events.Add(nameof(service.Started));
        service.Ready += (_, _) => events.Add(nameof(service.Ready));
        service.Stopping += (_, _) => events.Add(nameof(service.Stopping));
        service.Stopped += (_, _) => events.Add(nameof(service.Stopped));
        await service.Start();

        service.Transition(LifetimeState.Stopped);

        events.Should().Equal(nameof(service.Stopped));
    }

    [Fact]
    public async Task Transition_ToTheCurrentState_IsIgnored()
    {
        var service = new TestLifetimeService();
        var stoppedCount = 0;
        service.Stopped += (_, _) => stoppedCount++;
        await service.Start();

        service.Transition(LifetimeState.Stopped);
        service.Transition(LifetimeState.Stopped);

        stoppedCount.Should().Be(1);
    }

    [Fact]
    public async Task Transition_Backwards_Throws()
    {
        var service = new TestLifetimeService();
        await service.Start();
        service.Transition(LifetimeState.Stopped);

        var transition = () => service.Transition(LifetimeState.Stopping);

        transition.Should().Throw<Exception>().WithMessage("Invalid state transition*");
    }

    [Fact]
    public async Task Stop_AfterStopped_Throws()
    {
        var service = new TestLifetimeService();
        await service.Start();
        service.Transition(LifetimeState.Stopped);

        var stop = () => service.Stop();

        await stop.Should().ThrowAsync<Exception>().WithMessage("Invalid state*");
    }

    private sealed class TestLifetimeService : LifetimeServiceBase
    {
        public void Transition(LifetimeState state) => this.TransitionState(state);
    }
}
