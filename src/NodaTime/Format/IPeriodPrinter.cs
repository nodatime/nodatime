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
using System.IO;

namespace NodaTime.Format
{
    /// <summary>
    /// Internal interface for printing textual representations of time periods.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Application users will rarely use this class directly. Instead, you
    /// will use one of the factory classes to create a <see cref="PeriodFormatter"/>.
    /// </para>
    /// </remarks>
    public interface IPeriodPrinter
    {
        /// <summary>
        /// Returns the exact number of characters produced for the given period.
        /// </summary>
        /// <param name="period">The period to use</param>
        /// <param name="provider">The IFormatProvider to use</param>
        /// <returns>The estimated length</returns>
        int CalculatePrintedLength(IPeriod period, IFormatProvider provider);

        /// <summary>
        /// Returns the amount of fields from the given period that this printer
        /// will print.
        /// </summary>
        /// <param name="period">>The period to use</param>
        /// <param name="stopAt">Stop counting at this value, enter a number &gt; 256 to count all</param>
        /// <param name="provider">The IFormatProvider to use</param>
        /// <returns>Amount of fields printed</returns>
        int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider);

        /// <summary>
        /// Prints an IPeriod to a TextWriter.
        /// </summary>
        /// <param name="textWriter">Tthe formatted period is written out</param>
        /// <param name="period">The period to format</param>
        /// <param name="provider">The IFormatProvider to use</param>
        void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider);
    }
}
