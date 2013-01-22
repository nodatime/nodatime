// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DurationTest
    {
        private readonly Duration threeMillion = new Duration(3000000L);
        private readonly Duration negativeFiftyMillion = new Duration(-50000000L);
        private readonly Duration negativeEpsilon = new Duration(-1L);

        /// <summary>
        ///   Using the default constructor is equivalent to Duration.Zero.
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Duration();
            Assert.AreEqual(Duration.Zero, actual);
        }

    }
}
