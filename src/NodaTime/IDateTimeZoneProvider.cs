// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// Provides stable, performant time zone data.
    /// </summary>
    /// <remarks>Consumers should be able to treat an <see cref="IDateTimeZoneProvider"/> like a cache: 
    /// lookups should be quick (after at most one lookup of a given ID), and the data for a given ID should always be
    /// the same (even if the specific instance returned is not).
    /// Consumers should not feel the need to cache data accessed through this interface.
    /// </remarks>
    public interface IDateTimeZoneProvider
    {
        /// <summary>
        /// The version ID of this provider.
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// Gets the list of valid time zone ids advertised by this provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This list will be sorted in ordinal lexicographic order. It cannot be modified by callers, and
        /// must not be modified by the provider either: client code can safely treat it as thread-safe
        /// and deeply immutable.
        /// </para>
        /// <para>
        /// In addition to the list returned here, providers always support the fixed-offset timezones with IDs "UTC"
        /// and "UTC+/-Offset". These may or may not be included explicitly in this list.
        /// </para>
        /// </remarks>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        ReadOnlyCollection<string> Ids { get; }

#if !PCL
        /// <summary>
        /// Gets the time zone from this provider that matches the system default time zone, if a matching time zone is
        /// available.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method will throw <see cref="DateTimeZoneNotFoundException"/> if no matching
        /// time zone is found. For the built-in Noda Time providers, this is unlikely to occur in practice (assuming
        /// the system is using a standard Windows time zone), but can occur even then, if no mapping is found.
        /// If it is necessary to handle this case, callers can construct a
        /// <see cref="BclDateTimeZone"/> via <see cref="BclDateTimeZone.ForSystemDefault"/>, which returns a
        /// <see cref="DateTimeZone"/> that wraps the system local <see cref="TimeZoneInfo"/>, and which always
        /// succeeds.
        /// </remarks>
        /// <exception cref="DateTimeZoneNotFoundException">The system default time zone is not mapped by
        /// this provider.</exception>
        /// <returns>
        /// The provider-specific representation of the system default time zone.
        /// </returns>
        DateTimeZone GetSystemDefault();
#endif

        /// <summary>
        /// Returns the time zone for the given ID, if it's available.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this may return a <see cref="DateTimeZone"/> that has a different ID to that requested, if the ID
        /// provided is an alias.
        /// </para>
        /// <para>
        /// Note also that this method is not required to return the same <see cref="DateTimeZone"/> instance for
        /// successive requests for the same ID; however, all instances returned for a given ID must compare
        /// as equal.
        /// </para>
        /// <para>
        /// The fixed-offset timezones with IDs "UTC" and "UTC+/-Offset" are always available. These must
        /// return instances that are equal to those returned by <see cref="DateTimeZone.ForOffset"/>.
        /// </para>
        /// </remarks>
        /// <param name="id">The time zone ID to find.</param>
        /// <returns>The <see cref="DateTimeZone" /> for the given ID or null if the provider does not support
        /// the given ID.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        DateTimeZone GetZoneOrNull(string id);

        /// <summary>
        /// Returns the time zone for the given ID.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="GetZoneOrNull"/>, this indexer will never return a null reference. If the ID is not
        /// supported by this provider, it will throw <see cref="DateTimeZoneNotFoundException" />.
        /// </para>
        /// <para>
        /// Note that this may return a <see cref="DateTimeZone"/> that has a different ID to that requested, if the ID
        /// provided is an alias.
        /// </para>
        /// <para>
        /// Note also that this method is not required to return the same <see cref="DateTimeZone"/> instance for
        /// successive requests for the same ID; however, all instances returned for a given ID must compare
        /// as equal.
        /// </para>
        /// <para>
        /// The fixed-offset timezones with IDs "UTC" and "UTC+/-Offset" are always available. These must
        /// return instances that are equal to those returned by <see cref="DateTimeZone.ForOffset"/>.
        /// </para>
        /// </remarks>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="DateTimeZone" /> for the given ID.</returns>
        /// <exception cref="DateTimeZoneNotFoundException">This provider does not support the given ID.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        DateTimeZone this[string id] { get; }
    }
}
