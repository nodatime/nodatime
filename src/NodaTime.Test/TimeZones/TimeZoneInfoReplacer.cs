// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Class used to temporarily replace the shim used by <see cref="TimeZoneInfoInterceptor"/>,
    /// for test purposes. On disposal, the original is restored.
    /// </summary>
    internal class TimeZoneInfoReplacer : IDisposable, TimeZoneInfoInterceptor.ITimeZoneInfoShim
    {
        private readonly TimeZoneInfoInterceptor.ITimeZoneInfoShim originalShim;
        private readonly ReadOnlyCollection<TimeZoneInfo> zones;

        public TimeZoneInfo Local { get; }

        private TimeZoneInfoReplacer(TimeZoneInfo local, ReadOnlyCollection<TimeZoneInfo> zones)
        {
            originalShim = TimeZoneInfoInterceptor.Shim;
            Local = local;
            this.zones = zones;
            TimeZoneInfoInterceptor.Shim = this;

        }

        internal static IDisposable Replace(TimeZoneInfo local, params TimeZoneInfo[] allZones) =>
            new TimeZoneInfoReplacer(local, allZones.ToList().AsReadOnly());

        public TimeZoneInfo FindSystemTimeZoneById(string id)
        {
            var zone = zones.FirstOrDefault(z => z.Id == id);
            if (zone != null)
            {
                return zone;
            }
#if NETCORE
            // TimeZoneNotFoundException doesn't exist in netstandard. We're unlikely to use
            // this method in non-NET45 tests anyway, as it's only used in BclDateTimeZoneSource.
            throw new Exception($"No such time zone: {id}");
#else
            throw new TimeZoneNotFoundException($"No such time zone: {id}");
#endif
        }

        public ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones() => zones;

        public void Dispose() => TimeZoneInfoInterceptor.Shim = originalShim;
    }
}
