// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A location entry generated from the "zone.tab" file in a TZDB release. This can be used to provide
    /// users with a choice of time zone, although it is not internationalized.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class TzdbZoneLocation
    {
        private readonly int latitudeSeconds, longitudeSeconds;

        /// <summary>
        /// Gets the latitude in degrees; positive for North, negative for South.
        /// </summary>
        /// <remarks>The value will be in the range [-90, 90].</remarks>
        /// <value>The latitude in degrees; positive for North, negative for South.</value>
        public double Latitude => latitudeSeconds / 3600.0;

        /// <summary>
        /// Gets the longitude in degrees; positive for East, negative for West.
        /// </summary>
        /// <remarks>The value will be in the range [-180, 180].</remarks>
        /// <value>The longitude in degrees; positive for East, negative for West.</value>
        public double Longitude => longitudeSeconds / 3600.0;

        /// <summary>
        /// Gets the English name of the country containing the location.
        /// </summary>
        /// <value>The English name of the country containing the location.</value>
        [NotNull] public string CountryName { get; }

        /// <summary>
        /// Gets the ISO-3166 2-letter country code for the country containing the location.
        /// </summary>
        /// <value>The ISO-3166 2-letter country code for the country containing the location.</value>
        [NotNull] public string CountryCode { get; }

        /// <summary>
        /// The ID of the time zone for this location.
        /// </summary>
        /// <remarks>If this mapping was fetched from a <see cref="TzdbDateTimeZoneSource"/>, it will always be a valid ID within that source.
        /// </remarks>
        /// <value>The ID of the time zone for this location.</value>
        [NotNull] public string ZoneId { get; }

        /// <summary>
        /// Gets the comment (in English) for the mapping, if any.
        /// </summary>
        /// <remarks>
        /// This is usually used to differentiate between locations in the same country.
        /// This will return an empty string if no comment was provided in the original data.
        /// </remarks>
        /// <value>The comment (in English) for the mapping, if any.</value>
        [NotNull] public string Comment { get; }

        /// <summary>
        /// Creates a new location.
        /// </summary>
        /// <remarks>This constructor is only public for the sake of testability. Non-test code should
        /// usually obtain locations from a <see cref="TzdbDateTimeZoneSource"/>.
        /// </remarks>
        /// <param name="latitudeSeconds">Latitude of the location, in seconds.</param>
        /// <param name="longitudeSeconds">Longitude of the location, in seconds.</param>
        /// <param name="countryName">English country name of the location, in degrees. Must not be null.</param>
        /// <param name="countryCode">ISO-3166 country code of the location. Must not be null.</param>
        /// <param name="zoneId">Time zone identifier of the location. Must not be null.</param>
        /// <param name="comment">Optional comment. Must not be null, but may be empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">The latitude or longitude is invalid.</exception>
        public TzdbZoneLocation(int latitudeSeconds, int longitudeSeconds, string countryName, string countryCode,
            string zoneId, string comment)
        {
            Preconditions.CheckArgumentRange(nameof(latitudeSeconds), latitudeSeconds, -90 * 3600, 90 * 3600);
            Preconditions.CheckArgumentRange(nameof(longitudeSeconds), longitudeSeconds, -180 * 3600, 180 * 3600);
            this.latitudeSeconds = latitudeSeconds;
            this.longitudeSeconds = longitudeSeconds;
            this.CountryName = Preconditions.CheckNotNull(countryName, nameof(countryName));
            this.CountryCode = Preconditions.CheckNotNull(countryCode, nameof(countryCode));
            this.ZoneId = Preconditions.CheckNotNull(zoneId, nameof(zoneId));
            this.Comment = Preconditions.CheckNotNull(comment, nameof(comment));
        }

        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteSignedCount(latitudeSeconds);
            writer.WriteSignedCount(longitudeSeconds);
            writer.WriteString(CountryName);
            writer.WriteString(CountryCode);
            writer.WriteString(ZoneId);
            writer.WriteString(Comment);
        }

        internal static TzdbZoneLocation Read(IDateTimeZoneReader reader)
        {
            int latitudeSeconds = reader.ReadSignedCount();
            int longitudeSeconds = reader.ReadSignedCount();
            string countryName = reader.ReadString();
            string countryCode = reader.ReadString();
            string zoneId = reader.ReadString();
            string comment = reader.ReadString();
            // We could duplicate the validation, but there's no good reason to. It's odd
            // to catch ArgumentException, but we're in pretty tight control of what's going on here.
            try
            {
                return new TzdbZoneLocation(latitudeSeconds, longitudeSeconds, countryName, countryCode, zoneId, comment);
            }
            catch (ArgumentException e)
            {
                throw new InvalidNodaDataException("Invalid zone location data in stream", e);
            }
        }
    }
}
