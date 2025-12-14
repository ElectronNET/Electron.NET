// <copyright file="TimeSpanExtensions.cs" company="Emby LLC">
// Copyright © Emby LLC. All rights reserved.
// </copyright>

namespace ElectronNET.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The TimeSpanExtensions class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "OK")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "OK")]
    [SuppressMessage("ReSharper", "StyleCop.SA1300", Justification = "OK")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "OK")]
    internal static class TimeSpanExtensions
    {
        public static TimeSpan ms(this int value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan ms(this long value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan seconds(this int value)
        {
            return TimeSpan.FromSeconds(value);
        }

        public static TimeSpan minutes(this int value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan hours(this int value)
        {
            return TimeSpan.FromHours(value);
        }

        public static TimeSpan days(this int value)
        {
            return TimeSpan.FromDays(value);
        }

        public static TimeSpan ms(this double value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        public static TimeSpan seconds(this double value)
        {
            return TimeSpan.FromSeconds(value);
        }

        public static TimeSpan minutes(this double value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan hours(this double value)
        {
            return TimeSpan.FromHours(value);
        }

        public static TimeSpan days(this double value)
        {
            return TimeSpan.FromDays(value);
        }
    }
}
