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
    ///   Provides an interface for a specialized IDateTimeZone deserializer.
    /// </summary>
    public interface IDateTimeZoneReader
    {
        /// <summary>
        ///   Reads a boolean value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteBoolean" />.
        /// </remarks>
        /// <returns>The boolean value.</returns>
        bool ReadBoolean();

        /// <summary>
        ///   Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteCount" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        int ReadCount();

        /// <summary>
        ///   Reads a string to string dictionary value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteDictionary" />.
        /// </remarks>
        /// <returns>The <see cref = "IDictionary{TKey,TValue}" /> value from the stream.</returns>
        IDictionary<string, string> ReadDictionary();

        /// <summary>
        ///   Reads an enumeration integer value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteEnum" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        int ReadEnum();

        /// <summary>
        ///   Reads an <see cref = "Instant" /> value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteInstant" />.
        /// </remarks>
        /// <returns>The <see cref = "Instant" /> value from the stream.</returns>
        Instant ReadInstant();

        /// <summary>
        ///   Reads an integer value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteInteger" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        int ReadInteger();

        /// <summary>
        ///   Reads an <see cref = "LocalInstant" /> value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteLocalInstant" />.
        /// </remarks>
        /// <returns>The <see cref = "LocalInstant" /> value from the stream.</returns>
        LocalInstant ReadLocalInstant();

        /// <summary>
        ///   Reads an integer millisecond value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteMilliseconds" />.
        /// </remarks>
        /// <returns>The integer millisecond value from the stream.</returns>
        int ReadMilliseconds();

        /// <summary>
        ///   Reads an <see cref = "Offset" /> value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteOffset" />.
        /// </remarks>
        /// <returns>The <see cref = "Offset" /> value from the stream.</returns>
        Offset ReadOffset();

        /// <summary>
        ///   Reads a string value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteString" />.
        /// </remarks>
        /// <returns>The string value from the stream.</returns>
        string ReadString();

        /// <summary>
        ///   Reads a long ticks value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteTicks" />.
        /// </remarks>
        /// <returns>The long ticks value from the stream.</returns>
        long ReadTicks();

        /// <summary>
        ///   Reads an <see cref = "IDateTimeZone" /> value from the stream.
        /// </summary>
        /// <remarks>
        ///   The value must have been written by <see cref = "IDateTimeZoneWriter.WriteTimeZone" />.
        /// </remarks>
        /// <returns>The <see cref = "IDateTimeZone" /> value from the stream.</returns>
        IDateTimeZone ReadTimeZone(string id);
    }
}
