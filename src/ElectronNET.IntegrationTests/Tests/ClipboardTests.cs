namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class ClipboardTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ElectronFixture fx;

        public ClipboardTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 5000)]
        public async Task Clipboard_text_roundtrip()
        {
            var text = $"Hello Electron {Guid.NewGuid()}";
            Electron.Clipboard.WriteText(text);
            var read = await Electron.Clipboard.ReadTextAsync();
            read.Should().Be(text);
        }

        [Fact(Timeout = 5000)]
        public async Task Available_formats_contains_text_after_write()
        {
            var text = "FormatsTest";
            Electron.Clipboard.WriteText(text);
            var formats = await Electron.Clipboard.AvailableFormatsAsync();
            formats.Should().Contain(f => f.Contains("text") || f.Contains("TEXT") || f.Contains("plain"));
        }

        [Fact(Timeout = 5000)]
        public async Task Bookmark_write_and_read()
        {
            var url = "https://electron-test.com";
            Electron.Clipboard.WriteBookmark("TitleTest", url);
            var bookmark = await Electron.Clipboard.ReadBookmarkAsync();
            bookmark.Url.Should().Be(url);
        }
    }
}