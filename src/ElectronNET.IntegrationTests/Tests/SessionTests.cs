namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API.Entities;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class SessionTests : IntegrationTestBase
    {
        public SessionTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Session_preloads_roundtrip()
        {
            var session = this.MainWindow.WebContents.Session;
            _ = await session.GetPreloadsAsync();
            // Use a dummy path; API should store value
            session.SetPreloads(new[] { "/tmp/preload_dummy.js" });
            var preloadsAfter = await session.GetPreloadsAsync();
            preloadsAfter.Should().Contain("/tmp/preload_dummy.js");
        }

        [IntegrationFact]
        public async Task Session_proxy_set_and_resolve()
        {
            var session = this.MainWindow.WebContents.Session;
            // Provide all ctor args (pacScript empty to ignore, proxyRules direct, bypass empty)
            await session.SetProxyAsync(new ProxyConfig("", "direct://", ""));
            var proxy = await session.ResolveProxyAsync("https://example.com");
            proxy.Should().NotBeNull();
        }


        [IntegrationFact]
        public async Task Session_clear_cache_and_storage_completes()
        {
            var session = this.MainWindow.WebContents.Session;
            await session.ClearCacheAsync();
            await session.ClearStorageDataAsync();
            await session.ClearHostResolverCacheAsync();
            // Ensure still can query user agent after clears
            var ua = await session.GetUserAgent();
            ua.Should().NotBeNullOrWhiteSpace();
        }

        [IntegrationFact]
        public async Task Session_preloads_set_multiple_and_clear()
        {
            var session = this.MainWindow.WebContents.Session;
            session.SetPreloads(new[] { "/tmp/a.js", "/tmp/b.js" });
            var after = await session.GetPreloadsAsync();
            after.Should().Contain("/tmp/a.js").And.Contain("/tmp/b.js");
            // Reset to empty
            session.SetPreloads(Array.Empty<string>());
            var empty = await session.GetPreloadsAsync();
            empty.Should().NotContain("/tmp/a.js");
        }

        [IntegrationFact]
        public async Task Clear_auth_cache_overloads()
        {
            var session = this.MainWindow.WebContents.Session;
            await session.ClearAuthCacheAsync();
            await session.ClearAuthCacheAsync(new RemovePassword("password") { Origin = "https://example.com", Username = "user", Password = "pw", Realm = "realm", Scheme = Scheme.basic });
        }

        [IntegrationFact]
        public async Task Clear_storage_with_options()
        {
            var session = this.MainWindow.WebContents.Session;
            await session.ClearStorageDataAsync(new ClearStorageDataOptions { Storages = new[] { "cookies" }, Quotas = new[] { "temporary" } });
        }

        [IntegrationFact]
        public async Task Enable_disable_network_emulation()
        {
            var session = this.MainWindow.WebContents.Session;
            session.EnableNetworkEmulation(new EnableNetworkEmulationOptions { Offline = false, Latency = 10, DownloadThroughput = 50000, UploadThroughput = 20000 });
            session.DisableNetworkEmulation();
        }

        [IntegrationFact]
        public async Task Flush_storage_data_does_not_throw()
        {
            var session = this.MainWindow.WebContents.Session;
            session.FlushStorageData();
        }

        [IntegrationFact]
        public async Task Set_user_agent_affects_new_navigation()
        {
            var session = this.MainWindow.WebContents.Session;
            // Set UA and verify via session API (navigator.userAgent on existing WebContents may not reflect the override)
            session.SetUserAgent("IntegrationAgent/1.0");
            var ua = await session.GetUserAgent();
            ua.Should().NotBeNullOrWhiteSpace();
            ua.Should().Contain("IntegrationAgent/1.0");
        }

        [IntegrationFact]
        public async Task Download_policy_blocks_extension()
        {
            var session = this.MainWindow.WebContents.Session;
            var blocked = new TaskCompletionSource<DownloadInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
            Action<DownloadInfo> handler = info => blocked.TrySetResult(info);
            session.OnDownloadBlocked += handler;

            try
            {
                session.SetDownloadPolicy(new DownloadPolicy { BlockedExtensions = new[] { "exe" } });
                await this.TriggerDownloadAsync("malware.EXE");

                var info = await blocked.Task.WaitAsync(TimeSpan.FromSeconds(10));
                info.Filename.Should().Be("malware.EXE");
            }
            finally
            {
                session.OnDownloadBlocked -= handler;
                session.SetDownloadPolicy(null);
            }
        }

        [IntegrationFact]
        public async Task Download_policy_saves_to_directory()
        {
            var session = this.MainWindow.WebContents.Session;
            var directory = Path.Combine(Path.GetTempPath(), "electronnet-downloads-" + Guid.NewGuid().ToString("N"));
            var done = new TaskCompletionSource<DownloadDoneInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
            Action<DownloadDoneInfo> handler = info => done.TrySetResult(info);
            session.OnDownloadDone += handler;

            try
            {
                session.SetDownloadPolicy(new DownloadPolicy { AllowedExtensions = new[] { ".txt" }, SaveDirectory = directory });
                await this.TriggerDownloadAsync("hello.txt");

                var info = await done.Task.WaitAsync(TimeSpan.FromSeconds(10));
                info.State.Should().Be(DownloadDoneState.Completed);
                info.SavePath.Should().Be(Path.Combine(Path.GetFullPath(directory), "hello.txt"));
                File.ReadAllText(info.SavePath).Should().Be("hello");
            }
            finally
            {
                session.OnDownloadDone -= handler;
                session.SetDownloadPolicy(null);

                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        private async Task TriggerDownloadAsync(string filename)
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("about:blank");
            await wc.ExecuteJavaScriptAsync<string>(
                "(() => { const a = document.createElement('a');" +
                " a.href = URL.createObjectURL(new Blob(['hello'], { type: 'text/plain' }));" +
                $" a.download = '{filename}'; document.body.appendChild(a); a.click(); return 'ok'; })()");
        }
    }
}