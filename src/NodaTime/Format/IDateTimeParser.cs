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

using System;

namespace NodaTime.Format
{
    /// <summary>
    /// Internal interface for parsing textual representations of datetimes.
    /// </summary>
    public interface IDateTimeParser
    {
        /// <summary>
        /// Gets the expected maximum number of characters consumed.
        /// </summary>
        /// <remarks>
        /// The actual amount should rarely exceed this estimate.
        /// </remarks>
        int EstimatedParsedLength { get; }

        /// <summary>
        /// Parse an element from the given text, saving any fields into the given
        /// DateTimeParserBucket. If the parse succeeds, the return value is the new
        /// text position. Note that the parse may succeed without fully reading the
        /// text.
        /// </summary>
        /// <param name="bucket">Field are saved into this, not null</param>
        /// <param name="text">The text to parse, not null</param>
        /// <param name="position">Position to start parsing from</param>
        /// <returns>New position, negative value means parse failed -
        /// apply complement operator (~) to get position of failure</returns>
        /// <remarks>
        /// If it fails, the return value is negative. To determine the position
        /// where the parse failed, apply the one's complement operator (~) on the
        /// return value.
        /// </remarks>
        /// <exception cref="ArgumentException">If any field is out of range</exception>
        int ParseInto(DateTimeParserBucket bucket, string text, int position);
    }
}
