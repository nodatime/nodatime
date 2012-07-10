using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// Gets a representation of the system default time zone.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method can throw <see cref="TimeZoneNotFoundException"/>,
        /// even with standard Windows time zones.
        /// This could be due to either the Unicode CLDR not being up-to-date with Windows time zone IDs,
        /// or Noda Time not being up-to-date with CLDR - or a source-specific problem. To handle this case,
        /// (for example, by providing a default), use <see cref="GetSystemDefaultOrNull"/> instead. 
        /// </remarks>
        /// <exception cref="TimeZoneNotFoundException">The system default time zone is not mapped by
        /// this provider.</exception>
        /// <returns>
        /// The source-specific representation of the system time zone.
        /// </returns>
        DateTimeZone GetSystemDefault();

        /// <summary>
        /// Gets a representation of the system default time zone.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method can return null, even with standard Windows time zones.
        /// This could be due to either the Unicode CLDR not being up-to-date with Windows time zone IDs,
        /// or Noda Time not being up-to-date with CLDR - or a provider-specific problem. Callers can use
        /// the null-coalescing operator to effectively provide a default.
        /// </remarks>
        /// <returns>
        /// The provider-specific representation of the system time zone, or null if the time zone
        /// could not be mapped.
        /// </returns>
        DateTimeZone GetSystemDefaultOrNull();

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
        /// successive requests for the same ID; however, all instances returned for a given ID should be equivalent.
        /// </para>
        /// <para>
        /// The fixed-offset timezones with IDs "UTC" and "UTC+/-Offset" are always available.
        /// </para>
        /// </remarks>
        /// <param name="id">The time zone ID to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> for the given ID or <c>null</c> if the provider does not support
        /// the given ID.</returns>
        DateTimeZone GetZoneOrNull(string id);

        /// <summary>
        /// Returns the time zone for the given ID.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="GetZoneOrNull"/>, this indexer will never return a null reference. If the ID is not
        /// supported by this provider, it will throw <see cref="TimeZoneNotFoundException"/>.
        /// </para>
        /// <para>
        /// Note that this may return a <see cref="DateTimeZone"/> that has a different ID to that requested, if the ID
        /// provided is an alias.
        /// </para>
        /// <para>
        /// Note also that this method is not required to return the same <see cref="DateTimeZone"/> instance for
        /// successive requests for the same ID; however, all instances returned for a given ID should be equivalent.
        /// </para>
        /// <para>
        /// The fixed-offset timezones with IDs "UTC" and "UTC+/-Offset" are always available.
        /// </para>
        /// </remarks>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> for the given ID.</returns>
        /// <exception cref="TimeZoneNotFoundException">This provider does not support the given ID.</exception>
        DateTimeZone this[string id] { get; }
    }
}
