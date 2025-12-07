namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.Versioning;
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class NativeThemeTests
    {
        [IntegrationFact]
        public async Task ThemeSource_roundtrip()
        {
            // Capture initial
            _ = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            // Force light
            await Task.Delay(50.ms());
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Light);
            await Task.Delay(500.ms());
            var useDarkAfterLight = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            var themeSourceLight = await Electron.NativeTheme.GetThemeSourceAsync();
            // Force dark
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            await Task.Delay(500.ms());
            var useDarkAfterDark = await Electron.NativeTheme.ShouldUseDarkColorsAsync();
            var themeSourceDark = await Electron.NativeTheme.GetThemeSourceAsync();
            // Restore system
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.System);
            await Task.Delay(500.ms());
            var themeSourceSystem = await Electron.NativeTheme.GetThemeSourceAsync();
            // Assertions are tolerant (platform dependent)
            useDarkAfterLight.Should().BeFalse("forcing Light should result in light colors");
            useDarkAfterDark.Should().BeTrue("forcing Dark should result in dark colors");
            themeSourceLight.Should().Be(ThemeSourceMode.Light);
            themeSourceDark.Should().Be(ThemeSourceMode.Dark);
            themeSourceSystem.Should().Be(ThemeSourceMode.System);
        }

        [IntegrationFact]
        public async Task Updated_event_fires_on_change()
        {
            var fired = false;
            Electron.NativeTheme.Updated += () => fired = true;
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            await Task.Delay(400.ms());
            Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Light);
            for (int i = 0; i < 10 && !fired; i++)
            {
                await Task.Delay(100.ms());
            }

            fired.Should().BeTrue();
        }

        [IntegrationFact]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Windows")]
        public async Task Should_use_high_contrast_colors_check()
        {
            var metrics = await Electron.NativeTheme.ShouldUseHighContrastColorsAsync();
            metrics.Should().Be(false);
        }

        [IntegrationFact]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Windows")]
        public async Task Should_use_inverted_colors_check()
        {
            var metrics = await Electron.NativeTheme.ShouldUseInvertedColorSchemeAsync();
            metrics.Should().Be(false);
        }
    }
}
