// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Base class for variable length period fields - i.e. where the duration of a single value depends on when the value occurs.
    /// (For example, months and years.) Derived classes need only override Add and GetInt64Difference.
    /// </summary>
    internal abstract class VaryiableLengthPeriodField : PeriodField
    {
        internal VaryiableLengthPeriodField(PeriodFieldType fieldType, long averageUnitTicks)
            : base(fieldType, averageUnitTicks, false, true)
        {
        }
    }
}
