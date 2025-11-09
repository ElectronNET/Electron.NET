namespace ElectronNET.IntegrationTests.Tests
{
    [Collection("ElectronCollection")]
    public class CookiesTests
    {
        private readonly ElectronFixture fx;
        public CookiesTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Skip = "Cookie set/get requires navigation to domain; skipping until test harness serves page")]
        public async Task Cookie_set_get_remove_sequence()
        {
            var session = this.fx.MainWindow.WebContents.Session;
            var changed = false;
            session.Cookies.OnChanged += (cookie, cause, removed) => changed = true;
            // Navigate to example.com so cookie domain matches
            await this.fx.MainWindow.WebContents.LoadURLAsync("https://example.com");
            // Set via renderer for now
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("document.cookie='integration_cookie=1;path=/';");
            await Task.Delay(500);
            changed.Should().BeTrue();
        }
    }
}