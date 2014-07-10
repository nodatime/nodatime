// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalInstantTest
    {
        [Test]
        public void FromDateTime()
        {
            LocalInstant expected = new LocalInstant(2011, 08, 18, 20, 53);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                LocalInstant actual = LocalInstant.FromDateTime(x);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
