#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    ///   Represents a time zone.
    /// </summary>
    /// <remarks>
    ///   Time zones primarily encapsulate two facts: and offset from UTC and a set of rules on how
    ///   the values are adjusted.
    /// </remarks>
    public abstract class DateTimeZone
    {
        /// <summary>
        ///   This is the ID of the UTC (Coordinated Universal Time) time zone.
        /// </summary>
        public const string UtcId = "UTC";

        public static readonly DateTimeZoneResourceProvider DefaultDateTimeZoneProvider = new DateTimeZoneResourceProvider("NodaTime.TimeZones.Tzdb");

        private static readonly DateTimeZone UtcZone = new FixedDateTimeZone(Offset.Zero);

        private static TimeZoneCache cache = new TimeZoneCache(false);

        private readonly string id;
        private readonly bool isFixed;

        /// <summary>
        ///   Gets the UTC (Coordinated Universal Time) time zone.
        /// </summary>
        /// <value>The UTC <see cref = "T:NodaTime.DateTimeZone" />.</value>
        public static DateTimeZone Utc { get { return UtcZone; } }

        /// <summary>
        ///   Gets the system default time zone which can only be changed by the system.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The time zones defined in the operating system are different than the ones defines in
        ///     this library so a mapping will occur. If an exact mapping can be made then that will be
        ///     used otherwise UTC will be used.
        ///   </para>
        /// </remarks>
        /// <value>The system default <see cref = "T:NodaTime.DateTimeZone" /> this will never be <c>null</c>.</value>
        public static DateTimeZone SystemDefault { get { return cache.SystemDefault; } }

        /// <summary>
        ///   Gets or sets the current time zone.
        /// </summary>
        /// <remarks>
        ///   This is the time zone that is used whenever a time zone is not given to a method. It can
        ///   be set to any valid time zone. Setting it to <c>null</c> causes the
        ///   <see cref = "P:NodaTime.DateTimeZone.SystemDefault" /> time zone to be used.
        /// </remarks>
        /// <value>The current <see cref = "T:NodaTime.DateTimeZone" />. This will never be <c>null</c>.</value>
        public static DateTimeZone Current { get { return cache.Current; } set { cache.Current = value; } }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// TODO: Consider whether this should be ForID (as ID is a two-letter abbreviation).
        /// </summary>
        /// <param name = "id">The time zone id to find.</param>
        /// <returns>The <see cref = "DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static DateTimeZone ForId(string id)
        {
            return cache.ForId(id);
        }

        /// <summary>
        ///   Gets the complete list of valid time zone ids provided by all of the registered
        ///   providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref = "IEnumerable{T}" /> of string ids.</value>
        public static IEnumerable<string> Ids { get { return cache.Ids; } }

        /// <summary>
        ///   Adds the given time zone provider to the front of the provider list.
        /// </summary>
        /// <remarks>
        ///   Because this adds the new provider to the from of the list, it will be checked first for
        ///   time zone definitions and therefore can override the default system definitions. This
        ///   allows for adding new or replacing existing time zones without updating the system. If
        ///   the provider is already on the list nothing changes.
        /// </remarks>
        /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to add.</param>
        public static void AddProvider(IDateTimeZoneProvider provider)
        {
            cache.AddProvider(provider);
        }

        /// <summary>
        ///   Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        ///   If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        public static bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            return cache.RemoveProvider(provider);
        }

        /// <summary>
        ///   Sets the UTC time zone only mode.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     If the mode is set to <c>true</c> then only the UTC time zone provider will be available
        ///     and only the UTC time zone will be accessible. This is mainly for use during the
        ///     building of the system when there is no existing time zone database.
        ///   </para>
        ///   <para>
        ///     When this method is called all existing providers are removed from the list. Then the UTC
        ///     provideer will be added and if the <paramref name = "utcOnlyFlag" /> is <c>false</c> then default
        ///     provider. This means that any providers added by user code will be removed. The
        ///     <see cref = "P:NodaTime.DateTimeZone.Current" /> setting will also be lost.
        ///   </para>
        /// </remarks>
        /// <param name = "utcOnlyFlag">if set to <c>true</c> then only the UTC provider will be available.</param>
        internal static void SetUtcOnly(bool utcOnlyFlag)
        {
            cache = new TimeZoneCache(utcOnlyFlag);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:NodaTime.DateTimeZone" /> class.
        /// </summary>
        /// <param name = "id">The unique id of this time zone.</param>
        /// <param name = "isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        protected DateTimeZone(string id, bool isFixed)
        {
            this.id = id;
            this.isFixed = isFixed;
        }

        /// <summary>
        /// Returns a chronology based on this time zone, in the ISO calendar system.
        /// TODO(jonskeet): Cache this value?
        /// </summary>
        internal Chronology ToIsoChronology()
        {
            return new Chronology(this, CalendarSystem.Iso);
        }

        /// <summary>
        ///   The database ID for the time zone.
        /// </summary>
        /// <remarks>
        ///   This must be unique across all time zone providers.
        /// </remarks>
        public string Id { get { return id; } }

        /// <summary>
        ///   Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        /// <remarks>
        ///   This is used as an optimization. If the time zone has not transitions but returns <c>true</c>
        ///   for this then the behavior will be correct but the system will have to do extra work. However
        ///   if the time zone has transitions and this returns <c>false</c> then the transitions will never
        ///   be examined.
        /// </remarks>
        public bool IsFixed { get { return isFixed; } }

        /// <summary>
        ///   Returns the offset from UTC, where a positive duration indicates that local time is
        ///   later than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name = "instant">The instant for which to calculate the offset.</param>
        /// <returns>
        ///   The offset from UTC at the specified instant.
        /// </returns>
        public virtual Offset GetOffsetFromUtc(Instant instant)
        {
            ZoneInterval interval = GetZoneInterval(instant);
            return interval.Offset;
        }

        /// <summary>
        ///   Gets the zone interval for the given instant. Null is returned if no interval is
        ///   defined by the time zone for the given instant.
        /// </summary>
        /// <param name = "instant">The <see cref = "T:NodaTime.Instant" /> to query.</param>
        /// <returns>The defined <see cref = "T:NodaTime.TimeZones.ZoneInterval" /> or <c>null</c>.</returns>
        public abstract ZoneInterval GetZoneInterval(Instant instant);

        /// <summary>
        ///   Returns the name associated with the given instant.
        /// </summary>
        /// <remarks>
        ///   For a fixed time zone this will always return the same value but for a time zone that
        ///   honors daylight savings this will return a different name depending on the time of year
        ///   it represents. For example in the Pacific Standard Time (UTC-8) it will return either
        ///   PST or PDT depending on the time of year.
        /// </remarks>
        /// <param name = "instant">The <see cref = "T:NodaTime.Instant" /> to get the name for.</param>
        /// <returns>The name of this time. Never returns <c>null</c>.</returns>
        public virtual string GetName(Instant instant)
        {
            ZoneInterval interval = GetZoneInterval(instant);
            return interval.Name;
        }

        #region LocalInstant methods
        /// <summary>
        ///   Gets the offset to subtract from a local time to get the UTC time.
        /// </summary>
        /// <param name = "localInstant">The <see cref = "T:NodaTime.LocalInstant" /> to get the offset of.</param>
        /// <returns>The offset to subtract from the specified local time to obtain a UTC instant.</returns>
        /// <remarks>
        ///   Around a DST transition, local times behave peculiarly. When the time springs forward,
        ///   (e.g. 12:59 to 02:00) some times never occur; when the time falls back (e.g. 1:59 to
        ///   01:00) some times occur twice. This method always returns a smaller offset when there is
        ///   ambiguity, i.e. it treats the local time as the later of the possibilities.
        /// </remarks>
        /// <exception cref = "T:NodaTime.SkippedTimeException">The local instant doesn't occur in this time zone
        ///   due to zone transitions.</exception>
        internal virtual Offset GetOffsetFromLocal(LocalInstant localInstant)
        {
            // TODO: Handle a null return value... possibly by throwing a new kind of exception.
            ZoneInterval interval = GetZoneInterval(localInstant);
            return interval.Offset;
        }

        /// <summary>
        ///   Gets the zone interval for the given local instant. Null is returned if no interval is
        ///   defined by the time zone for the given local instant.
        /// </summary>
        /// <param name = "localInstant">The <see cref = "T:NodaTime.LocalInstant" /> to query.</param>
        /// <returns>The defined <see cref = "T:NodaTime.TimeZones.ZoneInterval" /> or <c>null</c>.</returns>
        internal abstract ZoneInterval GetZoneInterval(LocalInstant localInstant);

        // In the future:
        // <summary>
        // Finds all zone intervals for the given local instant. Usually there's one (i.e. only a single
        // instant is mapped to the given local instant within the time zone) but during DST transitions
        // there can be either 0 (the given local instant doesn't exist, e.g. local time skipped from 1am to
        // 2am, but you gave us 1.30am) or 2 (the given local instant is ambiguous, e.g. local time skipped
        // from 2am to 1am, but you gave us 1.30am).
        // </summary>
        // <param name="localInstant">The local instant to find matching zone intervals for</param>
        // <returns>The struct containing up to two ZoneInterval references.</returns>
        //internal abstract ZoneIntervalPair GetZoneIntervals(LocalInstant localInstant);
        #endregion LocalInstant methods

        #region I/O
        /// <summary>
        ///   Writes the time zone to the specified writer.
        /// </summary>
        /// <param name = "writer">The writer to write to.</param>
        internal abstract void Write(DateTimeZoneWriter writer);
        #endregion I/O

        #region Object overrides
        /// <summary>
        ///   Returns a <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Id;
        }
        #endregion
    }
}