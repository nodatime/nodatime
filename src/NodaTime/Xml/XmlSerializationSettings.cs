// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Xml
{
    /// <summary>
    /// Ambient settings applied during XML serialization and deserialization.
    /// XML serialization in .NET does not provide any "hooks" for advanced configuration.
    /// Most of the time that's a good thing, in terms of compatibility: we want
    /// the same data to be serialized the same way across multiple programs for
    /// interoperability. There are some exceptions to this however, where decisions
    /// need to be made and there's no single "right for everyone" choice.
    /// </summary>
    public static class XmlSerializationSettings
    {
        private static readonly object stateLock = new object();

        private static IDateTimeZoneProvider dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;

        /// <summary>
        /// Gets the <see cref="IDateTimeZoneProvider"/> to use to interpret a time zone ID read as part of
        /// XML serialization.
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
