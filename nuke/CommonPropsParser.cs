using System;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Parses a version from an MSBuild .props file (XML).
/// </summary>
public sealed class CommonPropsParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseNotesParser"/> class.
    /// </summary>
    public CommonPropsParser()
    {
    }

    public Version Parse(string propsPath)
    {
        var doc = XDocument.Load(propsPath);

        var versionElement = doc
            .Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Version");

        if (Version.TryParse(versionElement?.Value.Trim(), out var version))
        {
            version = new Version(version.Major, version.Minor, version.Build);
            return version;
        }

        return null;
    }
}