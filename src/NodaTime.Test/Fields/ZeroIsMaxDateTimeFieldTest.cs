#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class ZeroIsMaxDateTimeFieldTest
    {
        private readonly ZeroIsMaxDateTimeField field = new ZeroIsMaxDateTimeField
            (new PreciseDateTimeField(DateTimeFieldType.HourOfDay,
                                      PreciseDurationField.Hours, PreciseDurationField.Days),
             DateTimeFieldType.ClockHourOfDay);

        [Test]
        public void GetMinimum_AlwaysReturns1()
        {
            Assert.AreEqual(1, field.GetMinimumValue());
            Assert.AreEqual(1, field.GetMinimumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetMaximum_AlwaysReturnsWrappedMaximumPlus1()
        {
            Assert.AreEqual(24, field.GetMaximumValue());
            Assert.AreEqual(24, field.GetMaximumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetValue_ForZero_ReturnsMaximum()
        {
            Assert.AreEqual(24, field.GetValue(new LocalInstant(0)));
        }

        [Test]
        public void GetValue_ForNonZero_ReturnsOriginalValue()
        {
            Assert.AreEqual(1, field.GetValue(new LocalInstant(NodaConstants.TicksPerHour)));
        }

        [Test]
        public void TestSetValue_WithMaximumUsesZero()
        {
            Assert.AreEqual(0, field.SetValue(new LocalInstant(NodaConstants.TicksPerHour), 24).Ticks);
        }

        [Test]
        public void TestSetValue_WithNonMaximumPassesValueThrough()
        {
            Assert.AreEqual(NodaConstants.TicksPerHour * 2,
                            field.SetValue(new LocalInstant(0), 2).Ticks);
        }
    }
}