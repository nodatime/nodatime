using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Tests for <see cref="DurationFieldValue"/>. Currently fairly bare, as the struct itself is trivial...
    /// but it may get bigger.
    /// </summary>
    [TestFixture]
    public class DurationFieldValueTest
    {
        [Test]
        public void Construction_RetainsValues()
        {
            DurationFieldValue fieldValue = new DurationFieldValue(DurationFieldType.Months, 15);
            Assert.AreEqual(DurationFieldType.Months, fieldValue.FieldType);
            Assert.AreEqual(15, fieldValue.Value);
        }
    }
}
