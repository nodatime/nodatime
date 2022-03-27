// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET6_0_OR_GREATER
using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions
{
    public class DateOnlyExtensionsTest
    {
        [Test]
        public void ToLocalDate()
        {
            var dateOnly = new DateOnly(2011, 8, 18);
            var expected = new LocalDate(2011, 8, 18);
            var actual = dateOnly.ToLocalDate();
            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
