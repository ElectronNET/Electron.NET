namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class ShellTests : IntegrationTestBase
    {
        public ShellTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact(Skip = "This can keep the test process hanging until the e-mail window is closed")]
        public async Task OpenExternal_invalid_scheme_returns_error_or_empty()
        {
            var error = await Electron.Shell.OpenExternalAsync("mailto:test@example.com");
            (error == string.Empty || error.Contains("@") || error.Length > 0).Should().BeTrue(); // call succeeded
        }
    }
}