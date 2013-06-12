// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Fields
{
    internal class Int32DateTimeField : DateTimeField
    {
        private readonly NodaFunc<LocalInstant, int> getter;

        internal Int32DateTimeField(NodaFunc<LocalInstant, int> getter)
        {
            this.getter = getter;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return getter(localInstant);
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return getter(localInstant);
        }

        internal override long GetMaximumValue()
        {
            throw new NotImplementedException();
        }

        internal override long GetMaximumValue(LocalInstant ignored)
        {
            throw new NotImplementedException();
        }

        internal override long GetMinimumValue()
        {
            throw new NotImplementedException();
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotImplementedException();
        }
    }
}
