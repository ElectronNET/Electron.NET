namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class HostHookTests
    {
        [IntegrationFact(Skip = "Requires HostHook setup; skipping")]
        public async Task HostHook_call_returns_value()
        {
            var result = await Electron.HostHook.CallAsync<string>("create-excel-file", ".");
            result.Should().NotBeNull();
        }
    }
}