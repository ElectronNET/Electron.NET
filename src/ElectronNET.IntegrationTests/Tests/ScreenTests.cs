namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class ScreenTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ElectronFixture fx;

        public ScreenTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact]
        public async Task Primary_display_has_positive_dimensions()
        {
            var display = await Electron.Screen.GetPrimaryDisplayAsync();
            display.Size.Width.Should().BeGreaterThan(0);
            display.Size.Height.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllDisplays_returns_at_least_one()
        {
            var displays = await Electron.Screen.GetAllDisplaysAsync();
            displays.Should().NotBeNull();
            displays.Length.Should().BeGreaterThan(0);
        }
    }
}