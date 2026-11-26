namespace ElectronNET.IntegrationTests.Tests
{
    using System.Text.Json;
    using ElectronNET.API.Entities;
    using ElectronNET.API.Serialization;

    // Vibrancy is macOS-only but serialization itself is platform-independent so the warning can be disabled for these tests
    #pragma warning disable CA1416

    public class EnumSerializationTests
    {
        private static readonly JsonSerializerOptions Options = ElectronJson.Options;

        [Theory]
        [InlineData(Vibrancy.underWindow, "\"under-window\"")]
        [InlineData(Vibrancy.underPage, "\"under-page\"")]
        [InlineData(Vibrancy.appearanceBased, "\"appearance-based\"")]
        [InlineData(Vibrancy.fullscreenUi, "\"fullscreen-ui\"")]
        [InlineData(Vibrancy.sidebar, "\"sidebar\"")]
        public void Serialize_Vibrancy_ShouldUseEnumMemberValue(Vibrancy value, string expected)
        {
            JsonSerializer.Serialize(value, Options).Should().Be(expected);
        }

        [Fact]
        public void Serialize_TitleBarStyleDefault_ShouldUseJsonPropertyName()
        {
            JsonSerializer.Serialize(TitleBarStyle.defaultStyle, Options).Should().Be("\"default\"");
        }

        [Fact]
        public void Serialize_UnannotatedMember_ShouldBeCamelCase()
        {
            JsonSerializer.Serialize(TitleBarStyle.hiddenInset, Options).Should().Be("\"hiddenInset\"");
            JsonSerializer.Serialize(TitleBarStyle.customButtonsOnHover, Options).Should().Be("\"customButtonsOnHover\"");
        }

        [Fact]
        public void Serialize_BrowserWindowOptions_ShouldWriteAnnotatedEnumNames()
        {
            var options = new BrowserWindowOptions { Vibrancy = Vibrancy.underWindow };

            var json = JsonSerializer.Serialize(options, Options);

            json.Should().Contain("\"vibrancy\":\"under-window\"");
            json.Should().Contain("\"titleBarStyle\":\"default\"");
        }

        [Fact]
        public void Deserialize_CookieChangedCause_ShouldRoundTripJsonPropertyName()
        {
            JsonSerializer.Deserialize<CookieChangedCause>("\"expired_overwrite\"", Options).Should().Be(CookieChangedCause.expiredOverwrite);
            JsonSerializer.Serialize(CookieChangedCause.expiredOverwrite, Options).Should().Be("\"expired_overwrite\"");
        }

        [Theory]
        [InlineData("\"under-window\"")]
        [InlineData("\"underWindow\"")]
        [InlineData("\"UnderWindow\"")]
        public void Deserialize_Vibrancy_ShouldAcceptAnnotatedAndMemberNames(string json)
        {
            JsonSerializer.Deserialize<Vibrancy>(json, Options).Should().Be(Vibrancy.underWindow);
        }

        [Fact]
        public void Deserialize_IntegerValue_ShouldBeAccepted()
        {
            JsonSerializer.Deserialize<Vibrancy>(((int)Vibrancy.underWindow).ToString(), Options).Should().Be(Vibrancy.underWindow);
        }

        [Fact]
        public void Deserialize_UnknownName_ShouldThrow()
        {
            var act = () => JsonSerializer.Deserialize<Vibrancy>("\"not-a-vibrancy\"", Options);

            act.Should().Throw<JsonException>();
        }

        [Fact]
        public void NullableEnum_ShouldRoundTrip()
        {
            JsonSerializer.Serialize<Vibrancy?>(Vibrancy.underWindow, Options).Should().Be("\"under-window\"");
            JsonSerializer.Serialize<Vibrancy?>(null, Options).Should().Be("null");
            JsonSerializer.Deserialize<Vibrancy?>("\"under-window\"", Options).Should().Be(Vibrancy.underWindow);
            JsonSerializer.Deserialize<Vibrancy?>("null", Options).Should().BeNull();
        }
    }
}
