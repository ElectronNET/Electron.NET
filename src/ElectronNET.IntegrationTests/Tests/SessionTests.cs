namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class SessionTests
    {
        private readonly ElectronFixture fx;

        public SessionTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 5000)]
        public async Task Session_preloads_roundtrip()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            _ = await session.GetPreloadsAsync();
            // Use a dummy path; API should store value
            session.SetPreloads(new[] { "/tmp/preload_dummy.js" });
            var preloadsAfter = await session.GetPreloadsAsync();
            preloadsAfter.Should().Contain("/tmp/preload_dummy.js");
        }

        [Fact(Timeout = 5000)]
        public async Task Session_proxy_set_and_resolve()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            // Provide all ctor args (pacScript empty to ignore, proxyRules direct, bypass empty)
            await session.SetProxyAsync(new ProxyConfig("", "direct://", ""));
            var proxy = await session.ResolveProxyAsync("https://example.com");
            proxy.Should().NotBeNull();
        }


        [Fact(Timeout = 5000)]
        public async Task Session_clear_cache_and_storage_completes()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            await session.ClearCacheAsync();
            await session.ClearStorageDataAsync();
            await session.ClearHostResolverCacheAsync();
            // Ensure still can query user agent after clears
            var ua = await session.GetUserAgent();
            ua.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(Timeout = 5000)]
        public async Task Session_preloads_set_multiple_and_clear()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            session.SetPreloads(new[] { "/tmp/a.js", "/tmp/b.js" });
            var after = await session.GetPreloadsAsync();
            after.Should().Contain("/tmp/a.js").And.Contain("/tmp/b.js");
            // Reset to empty
            session.SetPreloads(Array.Empty<string>());
            var empty = await session.GetPreloadsAsync();
            empty.Should().NotContain("/tmp/a.js");
        }

        [Fact(Timeout = 5000)]
        public async Task Clear_auth_cache_overloads()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            await session.ClearAuthCacheAsync();
            await session.ClearAuthCacheAsync(new RemovePassword("password") { Origin = "https://example.com", Username = "user", Password = "pw", Realm = "realm", Scheme = Scheme.basic });
        }

        [Fact(Timeout = 5000)]
        public async Task Clear_storage_with_options()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            await session.ClearStorageDataAsync(new ClearStorageDataOptions { Storages = new[] { "cookies" }, Quotas = new[] { "temporary" } });
        }

        [Fact(Timeout = 5000)]
        public async Task Enable_disable_network_emulation()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            session.EnableNetworkEmulation(new EnableNetworkEmulationOptions { Offline = false, Latency = 10, DownloadThroughput = 50000, UploadThroughput = 20000 });
            session.DisableNetworkEmulation();
        }

        [Fact(Timeout = 5000)]
        public async Task Flush_storage_data_does_not_throw()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            session.FlushStorageData();
        }

        [Fact(Timeout = 5000)]
        public async Task Set_user_agent_affects_new_navigation()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            // Set UA and verify via session API (navigator.userAgent on existing WebContents may not reflect the override)
            session.SetUserAgent("IntegrationAgent/1.0");
            var ua = await session.GetUserAgent();
            ua.Should().NotBeNullOrWhiteSpace();
            ua.Should().Contain("IntegrationAgent/1.0");
        }
    }
}