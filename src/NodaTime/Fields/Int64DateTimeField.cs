// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Fields
{
    internal class Int64DateTimeField : DateTimeField
    {
        private readonly NodaFunc<LocalInstant, long> getter;

        internal Int64DateTimeField(NodaFunc<LocalInstant, long> getter)
        {
            this.getter = getter;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return getter(localInstant);
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return (int) getter(localInstant);
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
