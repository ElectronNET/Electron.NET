// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Exceptions;

/// <summary>
/// The release notes parser.
/// </summary>
/// <remarks>
/// Original from Cake build tool source:
/// https://github.com/cake-build/cake/blob/9828d7b246d332054896e52ba56983822feb3f05/src/Cake.Common/ReleaseNotesParser.cs
/// </remarks>
public sealed class ReleaseNotesParser
{
    private readonly Regex _versionRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReleaseNotesParser"/> class.
    /// </summary>
    public ReleaseNotesParser()
    {
        _versionRegex = new Regex(@"(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?");
    }

    /// <summary>
    /// Parses all release notes.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>All release notes.</returns>
    public IReadOnlyList<ReleaseNotes> Parse(string content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var lines = content.SplitLines();
        if (lines.Length > 0)
        {
            var line = lines[0].Trim();

            if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                return ParseComplexFormat(lines);
            }

            if (line.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                return ParseSimpleFormat(lines);
            }
        }

        throw new BuildAbortedException("Unknown release notes format.");
    }

    private IReadOnlyList<ReleaseNotes> ParseComplexFormat(string[] lines)
    {
        var lineIndex = 0;
        var result = new List<ReleaseNotes>();

        while (true)
        {
            if (lineIndex >= lines.Length)
            {
                break;
            }

            // Create release notes.
            var semVer = SemVersion.Zero;
            var version = SemVersion.TryParse(lines[lineIndex], out semVer);
            if (!version)
            {
                throw new BuildAbortedException("Could not parse version from release notes header.");
            }

            var rawVersionLine = lines[lineIndex];

            // Increase the line index.
            lineIndex++;

            // Parse content.
            var notes = new List<string>();
            while (true)
            {
                // Sanity checks.
                if (lineIndex >= lines.Length)
                {
                    break;
                }

                if (lines[lineIndex].StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // Get the current line.
                var line = (lines[lineIndex] ?? string.Empty).Trim('*').Trim();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    notes.Add(line);
                }

                lineIndex++;
            }

            result.Add(new ReleaseNotes(semVer, notes, rawVersionLine));
        }

        return result.OrderByDescending(x => x.SemVersion).ToArray();
    }

    private IReadOnlyList<ReleaseNotes> ParseSimpleFormat(string[] lines)
    {
        var lineIndex = 0;
        var result = new List<ReleaseNotes>();

        while (true)
        {
            if (lineIndex >= lines.Length)
            {
                break;
            }

            // Trim the current line.
            var line = (lines[lineIndex] ?? string.Empty).Trim('*', ' ');
            if (string.IsNullOrWhiteSpace(line))
            {
                lineIndex++;
                continue;
            }

            // Parse header.
            var semVer = SemVersion.Zero;
            var version = SemVersion.TryParse(lines[lineIndex], out semVer);
            if (!version)
            {
                throw new BuildAbortedException("Could not parse version from release notes header.");
            }

            // Parse the description.
            line = line.Substring(semVer.ToString().Length).Trim('-', ' ');

            // Add the release notes to the result.
            result.Add(new ReleaseNotes(semVer, new[] { line }, line));

            lineIndex++;
        }

        return result.OrderByDescending(x => x.SemVersion).ToArray();
    }
}