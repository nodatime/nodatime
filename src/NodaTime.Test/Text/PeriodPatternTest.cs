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

using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    // Container class just to house the nested types
    public static partial class PeriodPatternTest
    {
        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Period" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Period>
        {
            public IPattern<Period> StandardPattern { get; set; }

            // Irrelevant
            protected override Period DefaultTemplate
            {
                get { return Period.FromDays(0); }
            }

            public Data()
                : this(Period.FromDays(0))
            {
            }

            public Data(Period value)
                : base(value)
            {
                this.StandardPattern = PeriodPattern.RoundtripPattern;
            }

            public Data(PeriodBuilder builder)
                : this(builder.Build())
            {
            }

            internal override IPattern<Period> CreatePattern()
            {
                return StandardPattern;
            }
        }
    }
}
