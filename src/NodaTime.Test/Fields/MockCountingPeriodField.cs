// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    internal class MockCountingPeriodField : PeriodField
    {
        // TODO(Post-V1): Use a proper mock?
        private readonly long unitTicks;

        internal MockCountingPeriodField(PeriodFieldType fieldType) : this(fieldType, 60)
        {
        }

        internal MockCountingPeriodField(PeriodFieldType fieldType, long unitTicks) : base(fieldType, unitTicks, true, true)
        {
            this.unitTicks = unitTicks;
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return 0;
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(0);
        }

        internal static int int32Additions;
        internal static LocalInstant AddInstantArg;
        internal static int AddValueArg;

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            int32Additions++;
            AddInstantArg = localInstant;
            AddValueArg = value;
            return new LocalInstant(localInstant.Ticks + value * unitTicks);
        }

        internal static int int64Additions;
        internal static LocalInstant Add64InstantArg;
        internal static long Add64ValueArg;

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            int64Additions++;
            Add64InstantArg = localInstant;
            Add64ValueArg = value;

            return new LocalInstant(localInstant.Ticks + value * unitTicks);
        }

        internal static int differences;
        internal static LocalInstant DiffFirstArg;
        internal static LocalInstant DiffSecondArg;

        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            differences++;
            DiffFirstArg = minuendInstant;
            DiffSecondArg = subtrahendInstant;
            return 30;
        }

        internal static int differences64;
        internal static LocalInstant Diff64FirstArg;
        internal static LocalInstant Diff64SecondArg;

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            differences64++;
            Diff64FirstArg = minuendInstant;
            Diff64SecondArg = subtrahendInstant;
            return 30;
        }
    }
}