namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class HostHookTests
    {
        [Fact(Skip = "Requires HostHook setup; skipping")]
        public async Task HostHook_call_returns_value()
        {
            var result = await Electron.HostHook.CallAsync<string>("create-excel-file", ".");
            result.Should().NotBeNull();
        }
    }
}