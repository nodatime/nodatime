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
    /// Internal interface for parsing textual representations of time periods.
    /// </summary>
    /// <remarks>
    /// Application users will rarely use this class directly. Instead, you
    /// will use one of the factory classes to create a <see cref="PeriodFormatter"/>.
    /// </remarks>
    internal interface IPeriodParser
    {
        /// <summary>
        /// Parses a period from the given text, at the given position, appending the
        /// result into the given PeriodBuilder.
        /// </summary>
        /// <remarks>
        /// If the parse succeeds, the return value is the new text position.
        /// To determine the position where the parse failed, apply the
        /// one's complement operator (~) on the return value.
        /// </remarks>
        /// <param name="periodText">Text to parse</param>
        /// <param name="position">Position to start parsing from</param>
        /// <param name="builder">Period builder to use</param>
        /// <param name="provider">The IFormatProvider to use for parsing</param>
        /// <returns>New position, if negative, parse failed.</returns>
        int Parse(String periodText, int position, PeriodBuilder builder, IFormatProvider provider);
    }
}