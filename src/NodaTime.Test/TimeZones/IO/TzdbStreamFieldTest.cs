// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Test.TimeZones.IO
{
    public class TzdbStreamFieldTest
    {
        // Only tests for situations which aren't covered elsewhere

        [Test]
        public void InsufficientData()
        {
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            writer.WriteByte(1);
            writer.WriteCount(10);

            stream.Position = 0;
            var iterator = TzdbStreamField.ReadFields(stream).GetEnumerator();

            Assert.Throws<InvalidNodaDataException>(() => iterator.MoveNext());
        }
    }
}
