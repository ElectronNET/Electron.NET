using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.Converter;

/// <summary>
/// Handles the polymorphic shape of releaseNotes coming from electron-builder.
/// Depending on the updater.fullChangelog setting, electron-builder sends:
///   - null                        → when there are no notes
///   - "some string"               → plain string (FullChangelog = false, default)
///   - ["note A", "note B"]        → array of strings (after broken normalize in older TS)
///   - [{ version, note }, ...]    → array of objects (FullChangelog = true)
/// All forms are normalised to ReleaseNoteInfo[] so the C# model stays clean.
/// See: https://github.com/ElectronNET/Electron.NET/issues/1039
/// </summary>
public class ReleaseNotesConverter : JsonConverter<ReleaseNoteInfo[]>
{
    // Ensure the converter is called even when the JSON token is null,
    // so we can return an empty array instead of null.
    public override bool HandleNull => true;

    public override ReleaseNoteInfo[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return Array.Empty<ReleaseNoteInfo>();

            case JsonTokenType.String:
                // Plain string: "Some release notes"
                return new[] { new ReleaseNoteInfo { Note = reader.GetString() } };

            case JsonTokenType.StartArray:
                var list = new List<ReleaseNoteInfo>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        // Array of strings: ["Note A", "Note B"]
                        list.Add(new ReleaseNoteInfo { Note = reader.GetString() });
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        // Array of objects: [{ "version": "1.0", "note": "..." }]
                        using var doc = JsonDocument.ParseValue(ref reader);
                        var entry = new ReleaseNoteInfo();
                        if (doc.RootElement.TryGetProperty("version", out var version))
                            entry.Version = version.GetString();
                        if (doc.RootElement.TryGetProperty("note", out var note))
                            entry.Note = note.GetString();
                        list.Add(entry);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                return list.ToArray();

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when reading releaseNotes.");
        }
    }

    public override void Write(Utf8JsonWriter writer, ReleaseNoteInfo[] value, JsonSerializerOptions options)
    {
        if (value is null || value.Length == 0)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStartObject();
            if (item.Version is not null)
            {
                writer.WriteString("version", item.Version);
            }
            writer.WriteString("note", item.Note);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
