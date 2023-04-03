// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Class for representing semantic versions.
/// </summary>
/// <remarks>
/// Original from Cake build tool source:
/// https://github.com/cake-build/cake/blob/9828d7b246d332054896e52ba56983822feb3f05/src/Cake.Common/SemanticVersion.cs
/// </remarks>
public class SemVersion : IComparable, IComparable<SemVersion>, IEquatable<SemVersion>
{
    /// <summary>
    /// Gets the default version of a SemanticVersion.
    /// </summary>
    public static SemVersion Zero { get; } = new SemVersion(0, 0, 0, null, null, "0.0.0");

    /// <summary>
    /// Regex property for parsing a semantic version number.
    /// </summary>
    public static readonly Regex SemVerRegex =
        new Regex(
            @"(?<Major>0|(?:[1-9]\d*))(?:\.(?<Minor>0|(?:[1-9]\d*))(?:\.(?<Patch>0|(?:[1-9]\d*)))?(?:\-(?<PreRelease>[0-9A-Z\.-]+))?(?:\+(?<Meta>[0-9A-Z\.-]+))?)?",
            RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the major number of the version.
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// Gets the minor number of the version.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    /// Gets the patch number of the version.
    /// </summary>
    public int Patch { get; }

    /// <summary>
    /// Gets the prerelease of the version.
    /// </summary>
    public string PreRelease { get; }

    /// <summary>
    /// Gets the meta of the version.
    /// </summary>
    public string Meta { get; }

    /// <summary>
    /// Gets a value indicating whether semantic version is a prerelease or not.
    /// </summary>
    public bool IsPreRelease { get; }

    /// <summary>
    /// Gets a value indicating whether semantic version has meta or not.
    /// </summary>
    public bool HasMeta { get; }

    /// <summary>
    /// Gets the VersionString of the semantic version.
    /// </summary>
    public string VersionString { get; }

    /// <summary>
    /// Gets the AssemblyVersion of the semantic version.
    /// </summary>
    public Version AssemblyVersion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemVersion"/> class.
    /// </summary>
    /// <param name="major">Major number.</param>
    /// <param name="minor">Minor number.</param>
    /// <param name="patch">Patch number.</param>
    /// <param name="preRelease">Prerelease string.</param>
    /// <param name="meta">Meta string.</param>
    public SemVersion(int major, int minor, int patch, string preRelease = null, string meta = null) : this(major,
        minor, patch, preRelease, meta, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemVersion"/> class.
    /// </summary>
    /// <param name="major">Major number.</param>
    /// <param name="minor">Minor number.</param>
    /// <param name="patch">Patch number.</param>
    /// <param name="preRelease">Prerelease string.</param>
    /// <param name="meta">Meta string.</param>
    /// <param name="versionString">The complete version number.</param>
    public SemVersion(int major, int minor, int patch, string preRelease, string meta, string versionString)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        AssemblyVersion = new Version(major, minor, patch);
        IsPreRelease = !string.IsNullOrEmpty(preRelease);
        HasMeta = !string.IsNullOrEmpty(meta);
        PreRelease = IsPreRelease ? preRelease : null;
        Meta = HasMeta ? meta : null;

        if (!string.IsNullOrEmpty(versionString))
        {
            VersionString = versionString;
        }
        else
        {
            var sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Major, Minor, Patch);

            if (IsPreRelease)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "-{0}", PreRelease);
            }

            if (HasMeta)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "+{0}", Meta);
            }

            VersionString = sb.ToString();
        }
    }

    /// <summary>
    /// Method which tries to parse a semantic version string.
    /// </summary>
    /// <param name="version">the version that should be parsed.</param>
    /// <param name="semVersion">the out parameter the parsed version should be stored in.</param>
    /// <returns>Returns a boolean indicating if the parse was successful.</returns>
    public static bool TryParse(string version,
        out SemVersion semVersion)
    {
        semVersion = Zero;

        if (string.IsNullOrEmpty(version))
        {
            return false;
        }

        var match = SemVerRegex.Match(version);
        if (!match.Success)
        {
            return false;
        }

        if (!int.TryParse(
                match.Groups["Major"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var major) ||
            !int.TryParse(
                match.Groups["Minor"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var minor) ||
            !int.TryParse(
                match.Groups["Patch"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var patch))
        {
            return false;
        }

        semVersion = new SemVersion(
            major,
            minor,
            patch,
            match.Groups["PreRelease"]?.Value,
            match.Groups["Meta"]?.Value,
            version);

        return true;
    }

    /// <summary>
    /// Checks if two SemVersion objects are equal.
    /// </summary>
    /// <param name="other">the other SemVersion want to test equality to.</param>
    /// <returns>A boolean indicating whether the objecst we're equal or not.</returns>
    public bool Equals(SemVersion other)
    {
        return other is object
               && Major == other.Major
               && Minor == other.Minor
               && Patch == other.Patch
               && string.Equals(PreRelease, other.PreRelease, StringComparison.OrdinalIgnoreCase)
               && string.Equals(Meta, other.Meta, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Compares to SemVersion objects to and another.
    /// </summary>
    /// <param name="other">The SemVersion object we compare with.</param>
    /// <returns>Return 0 if the objects are identical, 1 if the version is newer and -1 if the version is older.</returns>
    public int CompareTo(SemVersion other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Equals(other))
        {
            return 0;
        }

        if (Major > other.Major)
        {
            return 1;
        }

        if (Major < other.Major)
        {
            return -1;
        }

        if (Minor > other.Minor)
        {
            return 1;
        }

        if (Minor < other.Minor)
        {
            return -1;
        }

        if (Patch > other.Patch)
        {
            return 1;
        }

        if (Patch < other.Patch)
        {
            return -1;
        }

        if (IsPreRelease != other.IsPreRelease)
        {
            return other.IsPreRelease ? 1 : -1;
        }

        switch (StringComparer.InvariantCultureIgnoreCase.Compare(PreRelease, other.PreRelease))
        {
            case 1:
                return 1;

            case -1:
                return -1;

            default:
            {
                return (string.IsNullOrEmpty(Meta) != string.IsNullOrEmpty(other.Meta))
                    ? string.IsNullOrEmpty(Meta) ? 1 : -1
                    : StringComparer.InvariantCultureIgnoreCase.Compare(Meta, other.Meta);
            }
        }
    }

    /// <summary>
    /// Compares to SemVersion objects to and another.
    /// </summary>
    /// <param name="obj">The object we compare with.</param>
    /// <returns>Return 0 if the objects are identical, 1 if the version is newer and -1 if the version is older.</returns>
    public int CompareTo(object obj)
    {
        return (obj is SemVersion semVersion)
            ? CompareTo(semVersion)
            : -1;
    }

    /// <summary>
    /// Equals-method for the SemVersion class.
    /// </summary>
    /// <param name="obj">the other SemVersion want to test equality to.</param>
    /// <returns>A boolean indicating whether the objecst we're equal or not.</returns>
    public override bool Equals(object obj)
    {
        return (obj is SemVersion semVersion)
               && Equals(semVersion);
    }

    /// <summary>
    /// Method for getting the hashcode of the SemVersion object.
    /// </summary>
    /// <returns>The hashcode of the SemVersion object.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Major;
            hashCode = (hashCode * 397) ^ Minor;
            hashCode = (hashCode * 397) ^ Patch;
            hashCode = (hashCode * 397) ^
                       (PreRelease != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(PreRelease) : 0);
            hashCode = (hashCode * 397) ^ (Meta != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Meta) : 0);
            return hashCode;
        }
    }

    /// <summary>
    /// Returns the string representation of an SemVersion object.
    /// </summary>
    /// <returns>The string representation of the object.</returns>
    public override string ToString()
    {
        int[] verParts = { Major, Minor, Patch };
        string ver = string.Join(".", verParts);
        return $"{ver}{(IsPreRelease ? "-" : string.Empty)}{PreRelease}{Meta}";
    }

    /// <summary>
    /// The greater than-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was greater than operand2.</returns>
    public static bool operator >(SemVersion operand1, SemVersion operand2)
        => operand1 is { } && operand1.CompareTo(operand2) == 1;

    /// <summary>
    /// The less than-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was less than operand2.</returns>
    public static bool operator <(SemVersion operand1, SemVersion operand2)
        => operand1 is { }
            ? operand1.CompareTo(operand2) == -1
            : operand2 is { };

    /// <summary>
    /// The greater than or equal to-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was greater than or equal to operand2.</returns>
    public static bool operator >=(SemVersion operand1, SemVersion operand2)
        => operand1 is { }
            ? operand1.CompareTo(operand2) >= 0
            : operand2 is null;

    /// <summary>
    /// The lesser than or equal to-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was lesser than or equal to operand2.</returns>
    public static bool operator <=(SemVersion operand1, SemVersion operand2)
        => operand1 is null || operand1.CompareTo(operand2) <= 0;

    /// <summary>
    /// The equal to-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was equal to operand2.</returns>
    public static bool operator ==(SemVersion operand1, SemVersion operand2)
        => operand1?.Equals(operand2) ?? operand2 is null;

    /// <summary>
    /// The not equal to-operator for the SemVersion class.
    /// </summary>
    /// <param name="operand1">first SemVersion.</param>
    /// <param name="operand2">second. SemVersion.</param>
    /// <returns>A value indicating if the operand1 was not equal to operand2.</returns>
    public static bool operator !=(SemVersion operand1, SemVersion operand2)
        => !(operand1?.Equals(operand2) ?? operand2 is null);
}