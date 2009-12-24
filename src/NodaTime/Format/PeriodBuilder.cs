using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Periods;
using NodaTime.Fields;

namespace NodaTime.Format
{
    public sealed class PeriodBuilder
    {
        private PeriodType periodType;
        private readonly int[] values;

        public PeriodBuilder(PeriodType periodType)
        {
            this.periodType = periodType;
            values = new int[periodType.Size];
        }


        public PeriodBuilder Append(DurationFieldType fieldType, int value)
        {
            return this;
        }

        public Period ToPeriod()
        {
            return new Period(values, periodType);
        }
    }
}
