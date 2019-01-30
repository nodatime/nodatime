// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.ObjectModel;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Interception for TimeZoneInfo static methods. These are still represented as
    /// static methods in this class, but they're implemented via a replacable shim, which
    /// by default delegates to the static methods in TimeZoneInfo.
    /// </summary>
    internal static class TimeZoneInfoInterceptor
    {
        /// <summary>
        /// The shim to use for all the static methods. We don't care about thread safety here,
        /// beyond "it must be correct when used in production" - it's only ever changed in tests,
        /// which are single-threaded anyway.
        /// </summary>
        internal static ITimeZoneInfoShim Shim { get; set; } = new BclShim();

        internal static TimeZoneInfo? Local => Shim.Local;
        internal static TimeZoneInfo FindSystemTimeZoneById(string id) => Shim.FindSystemTimeZoneById(id);
        internal static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones() => Shim.GetSystemTimeZones();

        internal interface ITimeZoneInfoShim
        {
            TimeZoneInfo? Local { get; }
            TimeZoneInfo FindSystemTimeZoneById(string id);
            ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();
        }

        /// <summary>
        /// Implementation that just delegates in a simple manner.
        /// </summary>
        private class BclShim : ITimeZoneInfoShim
        {
            public TimeZoneInfo? Local => TimeZoneInfo.Local;

            public TimeZoneInfo FindSystemTimeZoneById(string id) => TimeZoneInfo.FindSystemTimeZoneById(id);

            public ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones() => TimeZoneInfo.GetSystemTimeZones();
        }
    }
}
