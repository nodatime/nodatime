#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Interface for writing time-related data to a binary stream.
    /// This is similar to <see cref="BinaryWriter" />, but heavily
    /// oriented towards our use cases.
    /// </summary>
    /// <remarks>
    /// <para>It is expected that the code reading data written by an implementation
    /// will be able to identify which implementation to use. Initially we have two
    /// implementations: one for reading the data from the legacy resource format,
    /// and one for reading the data from the first version of the newer blob format.
    /// When the legacy resource format is retired, it's possible that we will only
    /// have one implementation moving forward - but the interface will allow us to
    /// evolve the details of the binary structure independently of the code in the
    /// time zone implementations which knows how to write/read in terms of this interface
    /// and <see cref="IDateTimeZoneReader"/>.
    /// </para>
    /// </remarks>
    internal interface IDateTimeZoneWriter
    {
        /// <summary>
        /// Writes a non-negative integer to the stream. This is optimized towards
        /// cases where most values will be small.
        /// </summary>
        /// <param name="count">The integer to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        void WriteCount(int count);

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <remarks>Other than the legacy resource writer, callers can reasonably expect that
        /// these values will be pooled in some fashion, so should not apply their own pooling.</remarks>
        /// <param name="value">The string to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        void WriteString(string value);

        /// <summary>
        /// Writes an offset to the stream.
        /// </summary>
        /// <param name="offset">The offset to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteOffset(Offset offset);

        /// <summary>
        /// Writes an instant to the stream.
        /// </summary>
        /// <param name="instant">The offset to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteInstant(Instant instant);

        /// <summary>
        /// Writes a string-to-string dictionary to the stream.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteDictionary(IDictionary<string, string> dictionary);

        /// <summary>
        /// Writes the given time zone to the stream.
        /// </summary>
        /// <param name="zone">The zone to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null.</exception>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteTimeZone(DateTimeZone zone);
    }
}
