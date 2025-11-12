using ElectronNET.API.Entities;

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

        [Fact]
        public async Task GetCursorScreenPoint_check()
        {
            var point = await Electron.Screen.GetCursorScreenPointAsync();
            point.Should().NotBeNull();
            point.X.Should().BeGreaterThanOrEqualTo(0);
            point.Y.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetMenuBarWorkArea_check()
        {
            var area = await Electron.Screen.GetMenuBarWorkAreaAsync();
            area.Should().NotBeNull();
            area.X.Should().BeGreaterThanOrEqualTo(0);
            area.Y.Should().BeGreaterThanOrEqualTo(0);
            area.Height.Should().BeGreaterThan(0);
            area.Width.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetDisplayNearestPoint_check()
        {
            var point = new Point
            {
                X = 100,
                Y = 100
            };
            var display = await Electron.Screen.GetDisplayNearestPointAsync(point);
            display.Should().NotBeNull();
            display.Size.Width.Should().BeGreaterThan(0);
            display.Size.Height.Should().BeGreaterThan(0);
        }
        [Fact]
        public async Task GetDisplayMatching_check()
        {
            var rectangle = new Rectangle
            {
                X = 100,
                Y = 100,
                Width = 100,
                Height = 100
            };
            var display = await Electron.Screen.GetDisplayMatchingAsync(rectangle);
            display.Should().NotBeNull();
            display.Size.Width.Should().BeGreaterThan(0);
            display.Size.Height.Should().BeGreaterThan(0);
        }
    }
}