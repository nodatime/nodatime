// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Tests for FieldSet; most just work on one or two fields as samples - the
    /// logic is the same for all fields of a given type.
    /// </summary>
    [TestFixture]
    public class FieldSetTest
    {
        private readonly DateTimeField sampleField = new FixedLengthDateTimeField(DateTimeFieldType.SecondOfMinute, FixedLengthPeriodField.Seconds,
                                                                              FixedLengthPeriodField.Minutes);

        [Test]
        public void FieldsAreCopiedFromBuilderToSet()
        {
            FieldSet fieldSet = new FieldSet.Builder { SecondOfMinute = sampleField, Seconds = sampleField.PeriodField }.Build();
            Assert.AreSame(sampleField, fieldSet.SecondOfMinute);
            Assert.AreSame(sampleField.PeriodField, fieldSet.Seconds);
        }

        [Test]
        public void UnsupportedDateTimeFields_HaveUnsupportedPeriodFields()
        {
            FieldSet fieldSet = new FieldSet.Builder { Seconds = FixedLengthPeriodField.Seconds }.Build();
            DateTimeField field = fieldSet.SecondOfMinute;
            Assert.IsFalse(field.IsSupported);
            Assert.IsFalse(field.PeriodField.IsSupported);
        }

        [Test]
        public void UnspecifiedPeriodFields_DefaultToUnsupported()
        {
            FieldSet fieldSet = new FieldSet.Builder().Build();
            foreach (var prop in typeof(FieldSet).GetProperties().Where(p => p.PropertyType == typeof(PeriodField)))
            {
                PeriodField field = (PeriodField)prop.GetValue(fieldSet, null);
                Assert.IsNotNull(field);
                Assert.IsFalse(field.IsSupported);
            }
        }

        [Test]
        public void UnspecifiedDateTimeFields_DefaultToUnsupported()
        {
            FieldSet fieldSet = new FieldSet.Builder().Build();
            foreach (var prop in typeof(FieldSet).GetProperties().Where(p => p.PropertyType == typeof(DateTimeField)))
            {
                DateTimeField field = (DateTimeField)prop.GetValue(fieldSet, null);
                Assert.IsNotNull(field);
                Assert.IsFalse(field.IsSupported);
            }
        }

        [Test]
        public void Builder_WithFieldSet_CopiesFields()
        {
            FieldSet originalFieldSet = new FieldSet.Builder { SecondOfMinute = sampleField, Seconds = sampleField.PeriodField }.Build();

            FieldSet newFieldSet = new FieldSet.Builder(originalFieldSet).Build();
            Assert.AreSame(sampleField, newFieldSet.SecondOfMinute);
            Assert.AreSame(sampleField.PeriodField, newFieldSet.Seconds);
        }

        [Test]
        public void Builder_WithNullFieldSet_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FieldSet.Builder(null));
        }

        [Test]
        public void WithSupportedFieldsFrom_WithNullSet_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FieldSet.Builder().WithSupportedFieldsFrom(null));
        }

        [Test]
        public void WithSupportedFieldsFrom_CopiedSupportedFields()
        {
            FieldSet originalFieldSet = new FieldSet.Builder { SecondOfMinute = sampleField }.Build();
            DateTimeField newField = new FixedLengthDateTimeField(DateTimeFieldType.SecondOfMinute, FixedLengthPeriodField.Seconds, FixedLengthPeriodField.Minutes);

            FieldSet newFieldSet = new FieldSet.Builder { SecondOfMinute = newField }.WithSupportedFieldsFrom(originalFieldSet).Build();
            // SecondOfMinute is supported in originalFieldSet, so the field is copied over
            Assert.AreSame(originalFieldSet.SecondOfMinute, newFieldSet.SecondOfMinute);
        }

        [Test]
        public void WithSupportedFieldsFrom_DoesNotCopyUnsupportedFields()
        {
            FieldSet originalFieldSet = new FieldSet.Builder { SecondOfMinute = sampleField }.Build();
            Assert.IsFalse(originalFieldSet.SecondOfDay.IsSupported);

            DateTimeField newField = new FixedLengthDateTimeField(DateTimeFieldType.SecondOfMinute, FixedLengthPeriodField.Seconds, FixedLengthPeriodField.Minutes);

            FieldSet newFieldSet = new FieldSet.Builder { SecondOfDay = newField }.WithSupportedFieldsFrom(originalFieldSet).Build();
            // SecondOfDay isn't supported in originalFieldSet, so the property we set is kept
            Assert.AreSame(newField, newFieldSet.SecondOfDay);
        }

        [Test]
        public void WithSupportedFieldsFrom_ReturnsSameBuilderReference()
        {
            FieldSet originalSet = new FieldSet.Builder().Build();
            FieldSet.Builder builder = new FieldSet.Builder();
            Assert.AreSame(builder, builder.WithSupportedFieldsFrom(originalSet));
        }
    }
}