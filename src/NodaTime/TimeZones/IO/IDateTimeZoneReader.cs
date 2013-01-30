// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Interface for reading time-related data from a binary stream.
    /// This is similar to <see cref="BinaryReader" />, but heavily
    /// oriented towards our use cases. 
    /// </summary>
    internal interface IDateTimeZoneReader
    {
        /// <summary>
        /// Reads a non-negative integer from the stream, which must have been written
        /// by a call to <see cref="IDateTimeZoneWriter.WriteCount"/>.
        /// </summary>
        /// <returns>The integer read from the stream</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        int ReadCount();

        /// <summary>
        /// Reads a non-negative integer from the stream, which must have been written
        /// by a call to <see cref="IDateTimeZoneWriter.WriteSignedCount"/>.
        /// </summary>
        /// <returns>The integer read from the stream</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        int ReadSignedCount();

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <returns>The string read from the stream; will not be null</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        string ReadString();

        /// <summary>
        /// Reads an offset from the stream.
        /// </summary>
        /// <returns>The offset read from the stream</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        Offset ReadOffset();

        /// <summary>
        /// Reads an instant representing a zone interval transition from the stream.
        /// </summary>
        /// <param name="previous">The previous transition written (usually for a given timezone), or null if there is
        /// no previous transition.</param>
        /// <returns>The instant read from the stream</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        Instant ReadZoneIntervalTransition(Instant? previous);

        /// <summary>
        /// Reads a string-to-string dictionary from the stream.
        /// </summary>
        /// <returns>The dictionary read from the stream</returns>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        IDictionary<string, string> ReadDictionary();

        /// <summary>
        /// Reads a time zone from the stream, assigning it the given ID.
        /// </summary>
        /// <returns>The zone read from the stream</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null</exception>
        /// <exception cref="InvalidNodaDataException">The data was invalid</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        DateTimeZone ReadTimeZone(string id);
    }
}
