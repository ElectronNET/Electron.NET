namespace ElectronNET.IntegrationTests.Tests
{
    using API;
    using System.Threading.Tasks;

    [Collection("ElectronCollection")]
    public class AutoUpdaterTests
    {
        private readonly ElectronFixture fx;
        public AutoUpdaterTests(ElectronFixture fx)
        {
            this.fx = fx;
        }
        
        [Fact]
        public void AutoDownload_check()
        {
            Electron.AutoUpdater.AutoDownload = false;
            var test1 =  Electron.AutoUpdater.AutoDownload;
            Electron.AutoUpdater.AutoDownload = true;
            var test2 =  Electron.AutoUpdater.AutoDownload;
            test1.Should().BeFalse();
            test2.Should().BeTrue();
        }

        [Fact]
        public void AutoInstallOnAppQuit_check()
        {
            Electron.AutoUpdater.AutoInstallOnAppQuit = false;
            var test1 =  Electron.AutoUpdater.AutoInstallOnAppQuit;
            Electron.AutoUpdater.AutoInstallOnAppQuit = true;
            var test2 =  Electron.AutoUpdater.AutoInstallOnAppQuit;
            test1.Should().BeFalse();
            test2.Should().BeTrue();
        }

        [Fact]
        public void AllowPrerelease_check()
        {
            Electron.AutoUpdater.AllowPrerelease = false;
            var test1 =  Electron.AutoUpdater.AllowPrerelease;
            Electron.AutoUpdater.AllowPrerelease = true;
            var test2 =  Electron.AutoUpdater.AllowPrerelease;
            test1.Should().BeFalse();
            test2.Should().BeTrue();
        }

        [Fact]
        public void FullChangelog_check()
        {
            Electron.AutoUpdater.FullChangelog = false;
            var test1 =  Electron.AutoUpdater.FullChangelog;
            Electron.AutoUpdater.FullChangelog = true;
            var test2 =  Electron.AutoUpdater.FullChangelog;
            test1.Should().BeFalse();
            test2.Should().BeTrue();
        }

        [Fact]
        public void AllowDowngrade_check()
        {
            Electron.AutoUpdater.AllowDowngrade = false;
            var test1 =  Electron.AutoUpdater.AllowDowngrade;
            Electron.AutoUpdater.AllowDowngrade = true;
            var test2 =  Electron.AutoUpdater.AllowDowngrade;
            test1.Should().BeFalse();
            test2.Should().BeTrue();
        }

        [Fact]
        public void UpdateConfigPath_check()
        {
            var test1 =  Electron.AutoUpdater.UpdateConfigPath;
            test1.Should().Be(string.Empty);
        }
        
        [Fact]
        public async Task CurrentVersionAsync_check()
        {
            var semver = await Electron.AutoUpdater.CurrentVersionAsync;
            semver.Should().NotBeNull();
            semver.Major.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ChannelAsync_check()
        {
            var test = await Electron.AutoUpdater.ChannelAsync;
            test.Should().Be(string.Empty);
            Electron.AutoUpdater.SetChannel = "beta";
            test = await Electron.AutoUpdater.ChannelAsync;
            test.Should().Be("beta");
        }

        [Fact]
        public async Task RequestHeadersAsync_check()
        {
            var headers = new Dictionary<string, string>
            {
                { "key1", "value1" },
            };
            var test = await Electron.AutoUpdater.RequestHeadersAsync;
            test.Should().BeNull();
            Electron.AutoUpdater.RequestHeaders = headers;
            test = await Electron.AutoUpdater.RequestHeadersAsync;
            test.Should().NotBeNull();
            test.Count.Should().Be(1);
            test["key1"].Should().Be("value1");
        }
        
        [Fact]
        public async Task CheckForUpdatesAsync_check()
        {
            var test = await Electron.AutoUpdater.CheckForUpdatesAsync();
            test.Should().BeNull();
        }
        
        [Fact]
        public async Task CheckForUpdatesAndNotifyAsync_check()
        {
            var test = await Electron.AutoUpdater.CheckForUpdatesAsync();
            test.Should().BeNull();
        }
        
        [Fact]
        public async Task GetFeedURLAsync_check()
        {
            var test = await Electron.AutoUpdater.GetFeedURLAsync();
            test.Should().Contain("Deprecated");
        }
    }    
}
