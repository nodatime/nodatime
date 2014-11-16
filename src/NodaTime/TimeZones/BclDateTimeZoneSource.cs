// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
#if !PCL
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an implementation of <see cref="IDateTimeZoneSource" /> that loads data from the BCL
    /// <see cref="TimeZoneInfo"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All calls to <see cref="ForId"/> return instances of <see cref="BclDateTimeZone"/>, including for fixed-offset IDs
    /// (i.e. "UTC" and "UTC+/-Offset").
    /// </para>
    /// <para>
    /// This class is not available in the PCL version.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type maintains no state, and all members are thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class BclDateTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// Returns the IDs of all system time zones.
        /// </summary>
        /// <returns>The IDs available from this source.</returns>
        public IEnumerable<string> GetIds()
        {
            // Always include the local time zone, since Mono may not include it in the list of system time zones, even
            // though it allows the Id to be passed to FindSystemTimeZoneById().
            // See https://code.google.com/p/noda-time/issues/detail?id=235.
            return TimeZoneInfo.GetSystemTimeZones()
                .Select(zone => zone.Id)
                .Union(GetTimeZoneInfoLocalIdOrEmpty());
        }

        /// <summary>
        /// Returns an enumerable containing a singleton element of the Id of the local time zone
        /// (<c>TimeZoneInfo.Local.Id</c>), unless the local time zone is not available, or not a system time zone, in
        /// which case returns an empty enumerable.
        /// </summary>
        private static IEnumerable<string> GetTimeZoneInfoLocalIdOrEmpty()
        {
            // This complexity is entirely to handle Mono, which fails quite badly at this in some cases.
            try
            {
                // May throw TimeZoneNotFoundException, particularly on Mono/Windows.
                // See https://bugzilla.xamarin.com/show_bug.cgi?id=11817
                var local = TimeZoneInfo.Local;

                if (local != null)  // https://code.google.com/p/noda-time/issues/detail?id=235#c8
                {
                    // Make sure we can look it up again, as there are legitimate cases where the local time zone is not
                    // a system time zone.  If not, this also throws TimeZoneNotFoundException.
                    TimeZoneInfo.FindSystemTimeZoneById(local.Id);

                    return new[] { local.Id };
                }
            }
            catch (TimeZoneNotFoundException)
            {
            }
            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        /// <remarks>
        /// This source returns a string such as "TimeZoneInfo: 3.5.0.0" corresponding to the version of the assembly
        /// containing <see cref="TimeZoneInfo"/>.
        /// </remarks>
        public string VersionId => "TimeZoneInfo: " + typeof(TimeZoneInfo).Assembly.GetName().Version;

        /// <summary>
        /// Creates a new instance of <see cref="BclDateTimeZone" /> from the <see cref="TimeZoneInfo"/> with the given
        /// ID. The ID must be a known system time zone ID.
        /// </summary>
        /// <remarks>
        /// This method explicitly implements <see cref="IDateTimeZoneSource.ForId"/> by delegating to the
        /// <see cref="ForId"/> method which has a return type of <see cref="BclDateTimeZone"/>, ensuring that all
        /// zones returned by this implementation are instances of <see cref="BclDateTimeZone"/> (rather than the built-in
        /// fixed offset zones).
        /// </remarks>        
        DateTimeZone IDateTimeZoneSource.ForId(string id) => ForId(id);

        /// <summary>
        /// Creates a new instance of <see cref="BclDateTimeZone" /> from the <see cref="TimeZoneInfo"/> with the given
        /// ID. The ID must be a known system time zone ID.
        /// </summary>
        /// <param name="id">The ID of the system time zone to convert</param>
        /// <exception cref="ArgumentException">The given zone doesn't exist.</exception>
        /// <returns>The Noda Time representation of the given BCL time zone</returns>
        public BclDateTimeZone ForId(string id)
        {
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return BclDateTimeZone.FromTimeZoneInfo(zone);
            }
            catch (TimeZoneNotFoundException)
            {                
                throw new ArgumentException(id + " is not a system time zone ID", "id");
            }
        }

        /// <summary>
        /// Maps the BCL ID to "our" ID as an identity projection.
        /// </summary>
        /// <param name="timeZone">The BCL time zone, which must be a known system time zone.</param>
        /// <returns>
        /// The ID for the given BCL time zone for this source; that is, the value of the <c>Id</c> property of the
        /// passed-in <see cref="TimeZoneInfo"/>.
        /// </returns>
        public string MapTimeZoneId(TimeZoneInfo timeZone) => timeZone.Id;
    }
}
#endif
