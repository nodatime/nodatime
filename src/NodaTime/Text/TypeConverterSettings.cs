// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Ambient settings applied for the default type converters. There's no simple context
    /// to express this information in a more elegant way, although users can create their
    /// own type converters to apply if they wish to.
    /// </summary>
    public static class TypeConverterSettings
    {
        private static readonly object stateLock = new object();

        private static IDateTimeZoneProvider dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;

        /// <summary>
        /// Gets the <see cref="IDateTimeZoneProvider"/> to use to interpret a time zone ID read as part of
        /// a TypeConverter operation for a <see cref="ZonedDateTime"/>.
        /// Note that if a value other than <see cref="DateTimeZoneProviders.Tzdb"/> is required, it should be set on
        /// application startup, before any type converters are used. Type converters are cached internally by the framework,
        /// so changes to this property after the first type converter for <see cref="ZonedDateTime"/> is created will
        /// not generally be visible.
        /// </summary>
        /// <remarks>
        /// This property defaults to <see cref="DateTimeZoneProviders.Tzdb"/>.
        /// </remarks>
        /// <value>The <c>IDateTimeZoneProvider</c> to use to interpret a time zone ID read as part of
        /// XML serialization.</value>
        public static IDateTimeZoneProvider DateTimeZoneProvider
        {
            get
            {
                lock (stateLock)
                {
                    return dateTimeZoneProvider;
                }
            }
            set
            {
                lock (stateLock)
                {
                    dateTimeZoneProvider = Preconditions.CheckNotNull(value, nameof(value));
                }
            }
        }
    }
}
