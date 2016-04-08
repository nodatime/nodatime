// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    // No tests here any more!
    // .NET changed (in 4.6?) to use a more complicated algorithm than in earlier versions.
    // Noda Time 2.0 supports that via stored data, as well as the earlier algorithm,
    // but we're not changing 1.x. Unfortunately that means we don't have useful
    // tests any more, as we were only testing against the BCL. This should be fine
    // so long as we don't need to change PersianYearMonthDayCalculator in the 1.x line.
}
