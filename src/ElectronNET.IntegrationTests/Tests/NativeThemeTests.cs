namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class NativeThemeTests
    {
        [Fact]
        public async Task ThemeSource_roundtrip()
        {
            // Capture initial
            _ = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            // Force light
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Light);
            var useDarkAfterLight = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            // Force dark
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            var useDarkAfterDark = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            // Restore system
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);
            // Assertions are tolerant (platform dependent)
            useDarkAfterLight.Should().BeFalse("forcing Light should result in light colors");
            useDarkAfterDark.Should().BeTrue("forcing Dark should result in dark colors");
        }

        [Fact]
        public async Task Updated_event_fires_on_change()
        {
            var fired = false;
            Electron.NativeTheme.Updated += () => fired = true;
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            await Task.Delay(400);
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Light);
            for (int i = 0; i < 10 && !fired; i++)
            {
                await Task.Delay(100);
            }

            fired.Should().BeTrue();
        }
    }
}