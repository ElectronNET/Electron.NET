namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.InteropServices;
    using ElectronNET.API;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class GlobalShortcutTests
    {
        [IntegrationFact]
        public async Task Can_register_and_unregister()
        {
            var accel = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "Cmd+Alt+G" : "Ctrl+Alt+G";
            var tcs = new TaskCompletionSource<bool>();
            Electron.GlobalShortcut.Register(accel, () => tcs.TrySetResult(true));
            var isRegistered = await Electron.GlobalShortcut.IsRegisteredAsync(accel);
            isRegistered.Should().BeTrue();
            Electron.GlobalShortcut.Unregister(accel);
        }
    }
}