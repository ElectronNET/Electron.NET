namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.InteropServices;
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class NotificationTests
    {
        [IntegrationFact]
        public async Task Notification_create_check()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "Always returns false. Might need full-blown desktop environment");

            var tcs = new TaskCompletionSource();

            var options = new NotificationOptions("Notification Title", "Notification test 123");
            options.OnShow = () => tcs.SetResult();

            await Task.Delay(500.ms());

            Electron.Notification.Show(options);

            await Task.WhenAny(tcs.Task, Task.Delay(5.seconds()));

            tcs.Task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Notification_is_supported_check()
        {
            Skip.If(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), "Always returns false. Might need full-blown desktop environment");

            var supported = await Electron.Notification.IsSupportedAsync();
            supported.Should().BeTrue();
        }
    }
}