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

namespace NodaTime.TimeZones
{
    /// <summary>
    ///   Provides an interface for a specialized IDateTimeZone serializer.
    /// </summary>
    public interface IDateTimeZoneWriter
    {
        /// <summary>
        ///   Writes a boolean value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteBoolean(bool value);

        /// <summary>
        ///   Writes the given non-negative integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteCount(int value);

        /// <summary>
        ///   Writes the given dictionary of string to string to the stream.
        /// </summary>
        /// <param name = "dictionary">The <see cref = "IDictionary{TKey,TValue}" /> to write.</param>
        void WriteDictionary(IDictionary<string, string> dictionary);

        /// <summary>
        ///   Writes an enumeration's integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteEnum(int value);

        /// <summary>
        ///   Writes the <see cref = "Instant" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteInstant(Instant value);

        /// <summary>
        ///   Writes the integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteInteger(int value);

        /// <summary>
        ///   Writes the <see cref = "LocalInstant" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteLocalInstant(LocalInstant value);

        /// <summary>
        ///   Writes the integer milliseconds value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteMilliseconds(int value);

        /// <summary>
        ///   Writes the <see cref = "Offset" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteOffset(Offset value);

        /// <summary>
        ///   Writes the string value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteString(string value);

        /// <summary>
        ///   Writes the long ticks value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteTicks(long value);

        /// <summary>
        ///   Writes the <see cref = "IDateTimeZone" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        void WriteTimeZone(IDateTimeZone value);
    }
}
