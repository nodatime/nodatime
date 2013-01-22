// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalTimeDemo
    {
        [Test]
        public void Construction()
        {
            LocalTime time = new LocalTime(16, 20, 0);
            Assert.AreEqual("16:20:00", time.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
