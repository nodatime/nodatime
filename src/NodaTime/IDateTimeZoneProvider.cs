using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NodaTime
{
    /// <summary>
    /// Provides stable, performant time zone data.
    /// </summary>
    /// <remarks>Consumers should be able to treat an <see cref="IDateTimeZoneProvider"/> like a cache: 
    /// lookups should be quick (after at most one lookup of a given ID), and the data for a given ID should always be the same.
    /// Consumers should not feel the need to cache data accessed through this interface.
    /// </remarks>
    public interface IDateTimeZoneProvider
    {
        /// <summary>
        /// The version ID of this provider.
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// Gets the complete list of valid time zone ids supported by this provider.
        /// </summary>
        /// <remarks>
        /// This list will be sorted in ordinal lexicographic order. It cannot be modified by callers, and
        /// must not be modified by the provider either: client code can safely treat it as thread-safe
        /// and deeply immutable.
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
        /// Returns the time zone with the given ID, if it's available.
        /// </summary>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given ID or <c>null</c> if the provider does not support it.</returns>
        DateTimeZone GetZoneOrNull(string id);

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <remarks>Unlike <see cref="GetZoneOrNull"/>, this indexer will never return a null reference. If the ID is not
        /// supported by this provider, it will throw <see cref="TimeZoneNotFoundException"/>.</remarks>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given ID.</returns>
        /// <exception cref="TimeZoneNotFoundException">This provider does not support the given ID.</exception>
        DateTimeZone this[string id] { get; }
    }
}