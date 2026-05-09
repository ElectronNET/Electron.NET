namespace ElectronNET.IntegrationTests.Tests
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using ElectronNET.API.Entities;

    /// <summary>
    /// Unit tests for UpdateInfo JSON deserialization.
    /// Tests the fix for issue #1039: releaseNotes arrives as string or string[] from electron-builder
    /// when FullChangelog is false (default), but the C# model expects ReleaseNoteInfo[].
    /// No Electron runtime is required for these tests.
    /// </summary>
    public class UpdateInfoSerializationTests
    {
        // camelCase + ignore null — mirrors ElectronJson.Options used in production
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // electron-builder sends a plain string when FullChangelog = false (default)
        [Fact]
        public void Deserialize_WithStringReleaseNotes_ShouldConvertToSingleEntry()
        {
            var json = """{"version":"1.2.3","releaseNotes":"Some release notes"}""";

            var result = JsonSerializer.Deserialize<UpdateInfo>(json, Options);

            result.Should().NotBeNull();
            result.ReleaseNotes.Should().HaveCount(1);
            result.ReleaseNotes[0].Note.Should().Be("Some release notes");
        }

        // After the (incorrect) TypeScript normalize: string → ["string"] which is an array of strings,
        // not an array of ReleaseNoteInfo objects. The C# model must handle this too.
        [Fact]
        public void Deserialize_WithArrayOfStringReleaseNotes_ShouldConvertToEntries()
        {
            var json = """{"version":"1.2.3","releaseNotes":["Note A","Note B"]}""";

            var result = JsonSerializer.Deserialize<UpdateInfo>(json, Options);

            result.Should().NotBeNull();
            result.ReleaseNotes.Should().HaveCount(2);
            result.ReleaseNotes[0].Note.Should().Be("Note A");
            result.ReleaseNotes[1].Note.Should().Be("Note B");
        }

        // When FullChangelog = true, electron-builder sends proper objects; this must keep working.
        [Fact]
        public void Deserialize_WithProperReleaseNoteObjects_ShouldDeserializeNormally()
        {
            var json = """{"version":"1.2.3","releaseNotes":[{"version":"1.2.3","note":"Proper note"}]}""";

            var result = JsonSerializer.Deserialize<UpdateInfo>(json, Options);

            result.Should().NotBeNull();
            result.ReleaseNotes.Should().HaveCount(1);
            result.ReleaseNotes[0].Version.Should().Be("1.2.3");
            result.ReleaseNotes[0].Note.Should().Be("Proper note");
        }

        // Null releaseNotes should result in an empty array (matching the default value).
        [Fact]
        public void Deserialize_WithNullReleaseNotes_ShouldReturnEmptyArray()
        {
            var json = """{"version":"1.2.3","releaseNotes":null}""";

            var result = JsonSerializer.Deserialize<UpdateInfo>(json, Options);

            result.Should().NotBeNull();
            result.ReleaseNotes.Should().NotBeNull();
            result.ReleaseNotes.Should().BeEmpty();
        }

        // Absent releaseNotes field should keep the default empty array.
        [Fact]
        public void Deserialize_WithMissingReleaseNotes_ShouldReturnEmptyArray()
        {
            var json = """{"version":"1.2.3"}""";

            var result = JsonSerializer.Deserialize<UpdateInfo>(json, Options);

            result.Should().NotBeNull();
            result.ReleaseNotes.Should().NotBeNull();
            result.ReleaseNotes.Should().BeEmpty();
        }
    }
}
