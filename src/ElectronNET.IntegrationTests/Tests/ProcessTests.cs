namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class ProcessTests
    {
        [Fact]
        public async Task Process_info_is_accessible()
        {
            // Use renderer to fetch process info and round-trip
            var execPath = await Electron.WindowManager.CreateWindowAsync(new API.Entities.BrowserWindowOptions { Show = false });
            var result = await execPath.WebContents.ExecuteJavaScriptAsync<string>("process.execPath && process.platform ? 'ok' : 'fail'");
            result.Should().Be("ok");
        }

        [Fact]
        public async Task Process_properties_are_populated()
        {
            var execPath = await Electron.Process.ExecPathAsync;
            execPath.Should().NotBeNullOrWhiteSpace();
            
            var pid = await Electron.Process.PidAsync;
            pid.Should().BeGreaterThan(0);
            
            var platform = await Electron.Process.PlatformAsync;
            platform.Should().NotBeNullOrWhiteSpace();
            
            var argv = await Electron.Process.ArgvAsync;
            argv.Should().NotBeNull();
            argv.Length.Should().BeGreaterThan(0);
            
            var type = await Electron.Process.TypeAsync;
            type.Should().NotBeNullOrWhiteSpace();
            
            var version = await Electron.Process.VersionsAsync;
            version.Should().NotBeNull();
            version.Chrome.Should().NotBeNullOrWhiteSpace();
            version.Electron.Should().NotBeNullOrWhiteSpace();
            
            var defaultApp = await Electron.Process.DefaultAppAsync;
            defaultApp.Should().BeTrue();
            
            var isMainFrame = await Electron.Process.IsMainFrameAsync;
            isMainFrame.Should().BeFalse();
            
            var resourcePath = await Electron.Process.ResourcesPathAsync;
            resourcePath.Should().NotBeNullOrWhiteSpace();

            var upTime = await Electron.Process.UpTimeAsync;
            upTime.Should().BeGreaterThan(0);
            
            var arch = await Electron.Process.ArchAsync;
            arch.Should().NotBeNullOrWhiteSpace();

        }

    }
}