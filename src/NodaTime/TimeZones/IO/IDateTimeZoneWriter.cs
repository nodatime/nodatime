// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Interface for writing time-related data to a binary stream.
    /// This is similar to <see cref="BinaryWriter" />, but heavily
    /// oriented towards our use cases.
    /// </summary>
    /// <remarks>
    /// <para>It is expected that the code reading data written by an implementation
    /// will be able to identify which implementation to use. As of Noda Time 2.0,
    /// there is only one implementation - but the interface will allow us to
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
        /// Writes a possibly-negative integer to the stream. This is optimized for
        /// values of small magnitudes.
        /// </summary>
        /// <param name="count">The integer to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteSignedCount(int count);

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <remarks>Callers can reasonably expect that
        /// these values will be pooled in some fashion, so should not apply their own pooling.</remarks>
        /// <param name="value">The string to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteString([NotNull] string value);

        /// <summary>
        /// Writes a number of milliseconds to the stream, where the number
        /// of milliseconds must be in the range (-1 day, +1 day).
        /// </summary>
        /// <param name="millis">The number of milliseconds to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millis"/> is out of range.</exception>
        void WriteMilliseconds(int millis);

        /// <summary>
        /// Writes an offset to the stream.
        /// </summary>
        /// <param name="offset">The offset to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteOffset(Offset offset);

        /// <summary>
        /// Writes an instant representing a zone interval transition to the stream.
        /// </summary>
        /// <remarks>
        /// This method takes a previously-written transition. Depending on the implementation, this value may be
        /// required by the reader in order to reconstruct the next transition, so it should be deterministic for any
        /// given value.
        /// </remarks>
        /// <param name="previous">The previous transition written (usually for a given timezone), or null if there is
        /// no previous transition.</param>
        /// <param name="value">The transition to write to the stream.</param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteZoneIntervalTransition(Instant? previous, Instant value);

        /// <summary>
        /// Writes a string-to-string dictionary to the stream.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <exception cref="IOException">The value couldn't be written to the stream.</exception>
        void WriteDictionary([NotNull] IDictionary<string, string> dictionary);

        /// <summary>
        /// Writes the given 8 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteByte(byte value);
    }
}
