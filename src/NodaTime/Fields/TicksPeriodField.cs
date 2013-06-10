// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Singleton period field for a fixed duration of 1 tick.
    /// </summary>
    internal sealed class TicksPeriodField : PeriodField
    {
        private static readonly TicksPeriodField instance = new TicksPeriodField();

        public static TicksPeriodField Instance { get { return instance; } }

        private TicksPeriodField() : base(PeriodFieldType.Ticks, 1, true, true)
        {
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return new LocalInstant(localInstant.Ticks + value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant(localInstant.Ticks + value);
        }

        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int)(minuendInstant.Ticks - subtrahendInstant.Ticks);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant.Ticks - subtrahendInstant.Ticks;
        }
    }
}