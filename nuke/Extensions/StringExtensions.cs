// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

// ReSharper disable once CheckNamespace
/// <summary>
/// Contains extension methods for <see cref="System.String"/>.
/// </summary>
/// <remarks>
/// Original from Cake build tool source:
/// https://github.com/cake-build/cake/blob/9828d7b246d332054896e52ba56983822feb3f05/src/Cake.Core/Extensions/StringExtensions.cs
/// </remarks>
public static class StringExtensions
{
    /// <summary>
    /// Quotes the specified <see cref="System.String"/>.
    /// </summary>
    /// <param name="value">The string to quote.</param>
    /// <returns>A quoted string.</returns>
    public static string Quote(this string value)
    {
        if (!IsQuoted(value))
        {
            value = string.Concat("\"", value, "\"");
        }

        return value;
    }

    /// <summary>
    /// Unquote the specified <see cref="System.String"/>.
    /// </summary>
    /// <param name="value">The string to unquote.</param>
    /// <returns>An unquoted string.</returns>
    public static string UnQuote(this string value)
    {
        if (IsQuoted(value))
        {
            value = value.Trim('"');
        }

        return value;
    }

    /// <summary>
    /// Splits the <see cref="String"/> into lines.
    /// </summary>
    /// <param name="content">The string to split.</param>
    /// <returns>The lines making up the provided string.</returns>
    public static string[] SplitLines(this string content)
    {
        content = NormalizeLineEndings(content);
        return content.Split(new[] { "\r\n" }, StringSplitOptions.None);
    }

    /// <summary>
    /// Normalizes the line endings in a <see cref="String"/>.
    /// </summary>
    /// <param name="value">The string to normalize line endings in.</param>
    /// <returns>A <see cref="String"/> with normalized line endings.</returns>
    public static string NormalizeLineEndings(this string value)
    {
        if (value != null)
        {
            value = value.Replace("\r\n", "\n");
            value = value.Replace("\r", string.Empty);
            return value.Replace("\n", "\r\n");
        }

        return string.Empty;
    }

    private static bool IsQuoted(this string value)
    {
        return value.StartsWith("\"", StringComparison.OrdinalIgnoreCase)
               && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase);
    }
}