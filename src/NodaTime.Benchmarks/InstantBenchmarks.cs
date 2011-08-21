#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NodaTime.Benchmarks.Timing;
using NodaTime.Format;
using NodaTime.Globalization;
using System.Globalization;

namespace NodaTime.Benchmarks
{
    internal class InstantBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.GetFormatInfo(CultureInfo.InvariantCulture);

        [Benchmark]
        public void TryParseExact_Valid_General()
        {
            Instant result;
            Instant.TryParseExact("2011-08-21T08:11:30Z", "g", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_Numeric()
        {
            Instant result;
            Instant.TryParseExact("123456789", "n", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidFormat()
        {
            Instant result;
            Instant.TryParseExact("0", "x", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_General()
        {
            Instant result;
            Instant.TryParseExact("2011-13-21T08:11:30Z", "g", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_Numeric()
        {
            Instant result;
            Instant.TryParseExact("12345xyz", "n", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }
    }
}
