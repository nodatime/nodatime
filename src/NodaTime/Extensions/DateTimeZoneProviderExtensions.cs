// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Utility;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IDateTimeZoneProvider"/>
    /// </summary>
    public static class DateTimeZoneProviderExtensions
    {
        /// <summary>
        /// Returns a lazily-evaluated sequence of time zones from the specified provider,
        /// in the same order in which the IDs are returned by the provider.
        /// </summary>
        /// <param name="provider">The provider to fetch time zones from.</param>
        /// <returns>All the time zones from the provider.</returns>
        public static IEnumerable<DateTimeZone> GetAllZones([NotNull] this IDateTimeZoneProvider provider)
        {
            Preconditions.CheckNotNull(provider, nameof(provider));
            return provider.Ids.Select(id => provider[id]);
        }
    }
}
