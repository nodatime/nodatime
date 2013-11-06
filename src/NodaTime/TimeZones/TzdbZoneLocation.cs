// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A location entry generated from the "zone.tab" file in a TZDB release. This can be used to provide
    /// users with a choice of time zone, although it is not internationalized.
    /// </summary>
    public sealed class TzdbZoneLocation
    {
        private readonly int latitudeSeconds, longitudeSeconds;
        private readonly string countryName, countryCode;
        private readonly string comment;
        private readonly string zoneId;

        /// <summary>
        /// Latitude in degrees; positive for North, negative for South.
        /// </summary>
        /// <remarks>The value will be in the range [-90, 90].</remarks>
        public double Latitude { get { return latitudeSeconds / 3600.0; } }

        /// <summary>
        /// Longitude in degrees; positive for East, negative for West.
        /// </summary>
        /// <remarks>The value will be in the range [-180, 180].</remarks>
        public double Longitude { get { return longitudeSeconds / 3600.0; } }

        /// <summary>
        /// The English name of the country containing the location.
        /// </summary>
        /// <remarks>This will never be null.</remarks>
        public string CountryName { get { return countryName; } }

        /// <summary>
        /// The ISO-3166 2-letter country code for the country containing the location.
        /// </summary>
        /// <remarks>This will never be null.</remarks>
        public string CountryCode { get { return countryCode; } }

        /// <summary>
        /// The ID of the time zone for this location.
        /// </summary>
        /// <remarks>This will never be null, and if this mapping was fetched
        /// from a <see cref="TzdbDateTimeZoneSource"/>, it will always be a valid ID within that source.
        /// </remarks>
        public string ZoneId { get { return zoneId; } }

        /// <summary>
        /// The comment (in English) for the mapping, if any.
        /// </summary>
        /// <remarks>
        /// This is usually used to differentiate between locations in the same country.
        /// This will return an empty string if no comment was provided in the original data.
        /// </remarks>
        public string Comment { get { return comment; } }

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
            Preconditions.CheckArgumentRange("latitudeSeconds", latitudeSeconds, -90 * 3600, 90 * 3600);
            Preconditions.CheckArgumentRange("longitudeSeconds", longitudeSeconds, -180 * 3600, 180 * 3600);
            this.latitudeSeconds = latitudeSeconds;
            this.longitudeSeconds = longitudeSeconds;
            this.countryName = Preconditions.CheckNotNull(countryName, "countryName");
            this.countryCode = Preconditions.CheckNotNull(countryCode, "countryCode");
            this.zoneId = Preconditions.CheckNotNull(zoneId, "zoneId");
            this.comment = Preconditions.CheckNotNull(comment, "comment");
        }

        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteSignedCount(latitudeSeconds);
            writer.WriteSignedCount(longitudeSeconds);
            writer.WriteString(countryName);
            writer.WriteString(countryCode);
            writer.WriteString(zoneId);
            writer.WriteString(comment);
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
