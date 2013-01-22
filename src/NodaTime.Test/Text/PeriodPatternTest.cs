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
