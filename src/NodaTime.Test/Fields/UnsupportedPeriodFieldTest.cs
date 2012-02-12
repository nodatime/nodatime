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
    [TestFixture]
    public class UnsupportedPeriodFieldTest
    {
        [Test]
        public void ConstantProperties_ReturnExpectedValues()
        {
            PeriodField field = UnsupportedPeriodField.Seconds;
            Assert.IsFalse(field.IsSupported);
            Assert.IsTrue(field.IsFixedLength);
            Assert.AreEqual(0, field.UnitTicks);
        }

        [Test]
        public void CachedValuesAreSingletons()
        {
            PeriodField field1 = UnsupportedPeriodField.Seconds;
            PeriodField field2 = UnsupportedPeriodField.ForFieldType(PeriodFieldType.Seconds);
            PeriodField field3 = UnsupportedPeriodField.ForFieldType(PeriodFieldType.Seconds);

            Assert.AreSame(field1, field2);
            Assert.AreSame(field1, field3);
        }

        [Test]
        public void UnsupportedOperations_ThrowNotSupportedException()
        {
            LocalInstant when = new LocalInstant();
            Duration duration = new Duration();

            AssertUnsupported(x => x.Add(when, 0));
            AssertUnsupported(x => x.Add(when, 0L));
            AssertUnsupported(x => x.GetDifference(when, when));
            AssertUnsupported(x => x.GetDuration(10));
            AssertUnsupported(x => x.GetDuration(10, when));
            AssertUnsupported(x => x.GetInt64Difference(when, when));
            AssertUnsupported(x => x.GetInt64Value(duration));
            AssertUnsupported(x => x.GetInt64Value(duration, when));
            AssertUnsupported(x => x.GetValue(duration));
            AssertUnsupported(x => x.GetValue(duration, when));
            AssertUnsupported(x => x.Subtract(when, 0));
            AssertUnsupported(x => x.Subtract(when, 0L));
        }

        private static void AssertUnsupported(Action<UnsupportedPeriodField> action)
        {
            Assert.Throws<NotSupportedException>(() => action(UnsupportedPeriodField.Seconds));
        }
    }
}