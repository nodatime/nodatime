#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Static access to time zones by ID, UTC etc. These were originally in DateTimeZone, but as
    /// that's now an interface it can't have methods etc.
    /// </summary>
    /// <remarks>
    /// The UTC time zone is defined to be present in all time zone databases and is built into the
    /// Noda Time system so it is always available. It is the only one guarenteed to be present in
    /// all systems.
    /// </remarks>
    public static class DateTimeZones
    {
        /// <summary>
        /// This is the ID of the UTC (Coordinated Univeral Time) time zone.
        /// </summary>
        public const string UtcId = "UTC";

        private static readonly IDateTimeZone utc = new FixedDateTimeZone(UtcId, Offset.Zero);
        private static IDateTimeZone current = utc;

        /// <summary>
        /// Gets or sets the system default time zone which can only be changed by the system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The time zones defined in the operating system may be different than the ones defines in
        /// this library so a mapping will occur. If an exact mapping can be made then that will be
        /// used otherwise either the closest possible <see cref="IDateTimeZone"/> will be used or
        /// one will be built.
        /// </para>
        /// </remarks>
        /// <value>The system default <see cref="IDateTimeZone"/>. this will never be <c>null</c>.</value>
        public static IDateTimeZone SystemDefault
        {
            // TODO: catch system time zone change message and reset this or always get the system one and map it?
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the current time zone.
        /// </summary>
        /// <remarks>
        /// This is the time zone that is used whenever a time zone is not given to a method. It can
        /// be set to any valid time zone. Setting it to <c>null</c> causes the <see
        /// cref="SystemDefault"/> time zone to be used.
        /// </remarks>
        /// <value>The current <see cref="IDateTimeZone"/>. This will never be <c>null</c>.</value>
        public static IDateTimeZone Current
        {
            get { return current; }
            set
            {
                if (value == null)
                {
                    current = SystemDefault;
                }
                else
                {
                    current = value;
                }
            }
        }

        /// <summary>
        /// Gets the UTC (Coordinated Univeral Time) time zone.
        /// </summary>
        /// <value>The UTC <see cref="IDateTimezone"/>.</value>
        public static IDateTimeZone Utc
        {
            get { return utc; }
        }

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="IDateTimeZone"/> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static IDateTimeZone ForId(string id)
        {
            if (id == UtcId)
            {
                return Utc;
            }
            throw new NotImplementedException();
        }
    }
}
