// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Annotations;
using NUnit.Framework;
using System.Reflection;

namespace NodaTime.Test.Annotations
{
    public class ReadWriteForEfficiencyTest
    {
        private static IEnumerable<FieldInfo> GetFieldsWithAttribute()
        {
            return from type in typeof(Instant).Assembly.GetTypes()
                   from field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                   where field.IsDefined(typeof(ReadWriteForEfficiencyAttribute))
                   select field;
        }

        [Test]
        public void AttributeOnlyAppliedToWritableFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => field.IsInitOnly)
                .ToList();
            Assert.IsEmpty(badFields);
        }

        [Test]
        public void AttributeOnlyAppliedToValueTypeFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => !field.FieldType.IsValueType)
                .ToList();
            Assert.IsEmpty(badFields);
        }

        [Test]
        public void AttributeOnlyAppliedToPrivateFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => !field.IsPrivate)
                .ToList();
            Assert.IsEmpty(badFields);
        }
    }
}
