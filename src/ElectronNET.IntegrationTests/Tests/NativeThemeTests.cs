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
            var themeSourceLight = await Electron.NativeTheme.GetThemeSourceAsync();
            // Force dark
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            var useDarkAfterDark = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            var themeSourceDark = await Electron.NativeTheme.GetThemeSourceAsync();
            // Restore system
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);
            var themeSourceSystem = await Electron.NativeTheme.GetThemeSourceAsync();
            // Assertions are tolerant (platform dependent)
            useDarkAfterLight.Should().BeFalse("forcing Light should result in light colors");
            useDarkAfterDark.Should().BeTrue("forcing Dark should result in dark colors");
            themeSourceLight.Should().Be(ThemeSourceMode.Light);
            themeSourceDark.Should().Be(ThemeSourceMode.Dark);
            themeSourceSystem.Should().Be(ThemeSourceMode.System);
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
        
        [Fact]
        public async Task Should_use_high_contrast_colors_check()
        {
            var metrics = await Electron.NativeTheme.ShouldUseHighContrastColorsAsync();
            metrics.Should().Be(false);
        }
        
        [Fact]
        public async Task Should_use_inverted_colors_check()
        {
            var metrics = await Electron.NativeTheme.ShouldUseInvertedColorSchemeAsync();
            metrics.Should().Be(false);
        }
    }
}