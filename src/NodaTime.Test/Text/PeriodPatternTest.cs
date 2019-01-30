// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
            // Irrelevant
            protected override Period DefaultTemplate => Period.FromDays(0);

            public Data() : this(Period.FromDays(0))
            {
            }

            public Data(Period value) : base(value)
            {
                this.StandardPattern = PeriodPattern.Roundtrip;
            }

            public Data(PeriodBuilder builder) : this(builder.Build())
            {
            }

            internal override IPattern<Period> CreatePattern() => StandardPattern!;
        }
    }
}
