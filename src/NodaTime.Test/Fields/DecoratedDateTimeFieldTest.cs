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
    public class DecoratedDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithNullWrappedField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SimpleDecoratedDateTimeField(null, DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void Constructor_WithUnsupportedWrappedField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new SimpleDecoratedDateTimeField(UnsupportedDateTimeField.MinuteOfDay, DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void WrappedField()
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, field.FieldType);
            Assert.AreSame(field, decorated.WrappedField);
        }

        [Test]
        public void FieldType_IsNotDelegated()
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(DateTimeFieldType.YearOfEra, decorated.FieldType);
        }

        [Test]
        public void Delegation()
        {
            LocalInstant when1 = new LocalInstant(12345L);
            LocalInstant when2 = new LocalInstant(98765L);
            Duration duration = new Duration(10000L);
            // Just a smattering
            AssertDelegated(x => x.GetValue(when1));
            AssertDelegated(x => x.GetInt64Value(when1));
            AssertDelegated(x => x.GetMaximumValue(when1));
            AssertDelegated(x => x.GetMaximumValue());
            AssertDelegated(x => x.SetValue(when1, 100));
            AssertDelegated(x => x.RoundFloor(when1));
        }

        private static void AssertDelegated<T>(Func<DateTimeField, T> func)
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(func(field), func(decorated));
        }

        private static DateTimeField CreateSampleField()
        {
            return new PreciseDateTimeField(DateTimeFieldType.TickOfMillisecond, TicksDurationField.Instance, PreciseDurationField.Milliseconds);
        }

        private class SimpleDecoratedDateTimeField : DecoratedDateTimeField
        {
            internal SimpleDecoratedDateTimeField(DateTimeField wrappedField, DateTimeFieldType fieldType) : base(wrappedField, fieldType)
            {
            }
        }
    }
}