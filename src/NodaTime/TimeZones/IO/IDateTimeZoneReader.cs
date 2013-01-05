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
        /// <exception cref="EndOfStreamException">The data didn't contain a complete value</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        int ReadCount();

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <returns>The string read from the stream; will not be null</returns>
        /// <exception cref="EndOfStreamException">The data didn't contain a complete value</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        string ReadString();

        /// <summary>
        /// Reads an offset from the stream.
        /// </summary>
        /// <returns>The offset read from the stream</returns>
        /// <exception cref="EndOfStreamException">The data didn't contain a complete value</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        Offset ReadOffset();

        /// <summary>
        /// Reads an instant from the stream.
        /// </summary>
        /// <returns>The instant read from the stream</returns>
        /// <exception cref="EndOfStreamException">The data didn't contain a complete value</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        Instant ReadInstant();

        /// <summary>
        /// Reads a string-to-string dictionary from the stream.
        /// </summary>
        /// <returns>The dictionary read from the stream</returns>
        /// <exception cref="EndOfStreamException">The data didn't contain a complete value</exception>
        /// <exception cref="IOException">The stream could not be read</exception>
        IDictionary<string, string> ReadDictionary(); 
    }
}
