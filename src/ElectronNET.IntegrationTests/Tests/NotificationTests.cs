namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class NotificationTests
    {
        [Fact]
        public async Task Notification_create_check()
        {
            var tcs = new TaskCompletionSource();

            var options = new NotificationOptions("Notification Title", "Notification test 123");
            options.OnShow = () => tcs.SetResult();

            Electron.Notification.Show(options);

            await Task.WhenAny(tcs.Task, Task.Delay(5_000));

            tcs.Task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task Notification_is_supported_check()
        {
            var supported = await Electron.Notification.IsSupportedAsync();
            supported.Should().BeTrue();
        }
    }
}