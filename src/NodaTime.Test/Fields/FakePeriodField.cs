// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Class allowing simple construction of fields for testing constructors of other fields.
    /// </summary>
    internal class FakePeriodField : PeriodField
    {
        internal FakePeriodField(long unitTicks, bool fixedLength) : base(PeriodFieldType.Seconds, unitTicks, fixedLength, true)
        {
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(0);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return new LocalInstant();
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant();
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return 0;
        }
    }
}