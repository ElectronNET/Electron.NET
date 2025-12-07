namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using System;
    using System.IO;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class AppTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ElectronFixture fx;

        public AppTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [IntegrationFact]
        public async Task Can_get_app_path()
        {
            var path = await Electron.App.GetAppPathAsync();
            path.Should().NotBeNullOrWhiteSpace();
            Directory.Exists(path).Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Can_get_version_and_locale()
        {
            var version = await Electron.App.GetVersionAsync();
            version.Should().NotBeNullOrWhiteSpace();
            var locale = await Electron.App.GetLocaleAsync();
            locale.Should().NotBeNullOrWhiteSpace();
        }

        [IntegrationFact]
        public async Task Can_get_special_paths()
        {
            var userData = await Electron.App.GetPathAsync(PathName.UserData);
            userData.Should().NotBeNullOrWhiteSpace();
            Directory.Exists(Path.GetDirectoryName(userData) ?? userData).Should().BeTrue();

            var temp = await Electron.App.GetPathAsync(PathName.Temp);
            temp.Should().NotBeNullOrWhiteSpace();
            Directory.Exists(temp).Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Can_get_app_metrics()
        {
            var metrics = await Electron.App.GetAppMetricsAsync();
            metrics.Should().NotBeNull();
            metrics.Length.Should().BeGreaterThan(0);
        }

        [IntegrationFact]
        public async Task Can_get_gpu_feature_status()
        {
            var status = await Electron.App.GetGpuFeatureStatusAsync();
            status.Should().NotBeNull();
        }

        [IntegrationFact]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Windows")]
        public async Task Can_get_login_item_settings()
        {
            var settings = await Electron.App.GetLoginItemSettingsAsync();
            settings.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task CommandLine_append_and_query_switch()
        {
            var switchName = "integration-switch";
            Electron.App.CommandLine.AppendSwitch(switchName, "value123");
            (await Electron.App.CommandLine.HasSwitchAsync(switchName)).Should().BeTrue();
            (await Electron.App.CommandLine.GetSwitchValueAsync(switchName)).Should().Be("value123");
        }

        [IntegrationFact]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Windows")]
        public async Task Accessibility_support_toggle()
        {
            Electron.App.SetAccessibilitySupportEnabled(true);
            var enabled = await Electron.App.IsAccessibilitySupportEnabledAsync();
            enabled.Should().BeTrue(); // API responded
            Electron.App.SetAccessibilitySupportEnabled(false);
        }

        [IntegrationFact]
        public async Task UserAgentFallback_roundtrip()
        {
            var original = await Electron.App.UserAgentFallbackAsync;
            Electron.App.UserAgentFallback = "ElectronIntegrationTest/1.0";
            var updated = await Electron.App.UserAgentFallbackAsync;
            updated.Should().Be("ElectronIntegrationTest/1.0");
            Electron.App.UserAgentFallback = original; // restore
        }

        [IntegrationFact]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        public async Task BadgeCount_set_and_reset_where_supported()
        {
            await Electron.App.SetBadgeCountAsync(2);
            var count = await Electron.App.GetBadgeCountAsync();
            // Some platforms may always return0; just ensure call didn't throw and is non-negative
            count.Should().BeGreaterThanOrEqualTo(0);
            await Electron.App.SetBadgeCountAsync(0);
        }

        [IntegrationFact]
        public async Task App_metrics_have_cpu_info()
        {
            var metrics = await Electron.App.GetAppMetricsAsync();
            metrics[0].Cpu.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task App_gpu_feature_status_has_some_fields()
        {
            var status = await Electron.App.GetGpuFeatureStatusAsync();
            status.Should().NotBeNull();
            status.Webgl.Should().NotBeNull();
            status.VideoDecode.Should().NotBeNull();
        }
    }
}
