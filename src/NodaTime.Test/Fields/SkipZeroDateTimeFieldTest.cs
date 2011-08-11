#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Tests for SkipZeroDateTimeField.
    /// </summary>
    [TestFixture]
    public class SkipZeroDateTimeFieldTest
    {
        private readonly DateTimeField sampleField = new SkipZeroDateTimeField(CalendarSystem.Iso.Fields.Year);

        [Test]
        public void GetValue()
        {
            Assert.AreEqual(2010, sampleField.GetValue(new LocalInstant(2010, 1, 1, 0, 0)));
            Assert.AreEqual(1, sampleField.GetValue(new LocalInstant(1, 1, 1, 0, 0)));
            Assert.AreEqual(-1, sampleField.GetValue(new LocalInstant(0, 1, 1, 0, 0)));
            Assert.AreEqual(-10, sampleField.GetValue(new LocalInstant(-9, 1, 1, 0, 0)));
        }

        [Test]
        public void GetInt64Value()
        {
            Assert.AreEqual(2010L, sampleField.GetInt64Value(new LocalInstant(2010, 1, 1, 0, 0)));
            Assert.AreEqual(1L, sampleField.GetInt64Value(new LocalInstant(1, 1, 1, 0, 0)));
            Assert.AreEqual(-1L, sampleField.GetInt64Value(new LocalInstant(0, 1, 1, 0, 0)));
            Assert.AreEqual(-10L, sampleField.GetInt64Value(new LocalInstant(-9, 1, 1, 0, 0)));
        }

        [Test]
        public void SetValue_NonZero()
        {
            LocalInstant start = new LocalInstant(2010, 1, 1, 0, 0);
            Assert.AreEqual(new LocalInstant(1500, 1, 1, 0, 0), sampleField.SetValue(start, 1500));
            // We ask for -6, so we're given the "normal" -5
            Assert.AreEqual(new LocalInstant(-5, 1, 1, 0, 0), sampleField.SetValue(start, -6));
        }

        [Test]
        public void SetValue_Zero()
        {
            LocalInstant start = new LocalInstant(2010, 1, 1, 0, 0);
            Assert.Throws<ArgumentException>(() => sampleField.SetValue(start, 0));
        }

        [Test]
        public void GetMinimumValue()
        {
            Assert.AreEqual(CalendarSystem.Iso.Fields.Year.GetMinimumValue() - 1,
                sampleField.GetMinimumValue());
        }
    }
}
