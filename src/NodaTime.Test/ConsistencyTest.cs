// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests using reflection to check that types are consistent.
    /// </summary>
    public class ConsistencyTest
    {
        private static IEnumerable<Type> AllPublicTypes = typeof(LocalDate).Assembly
            .GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        [Test]
        public void EqualityOperatorForEquatable()
        {
            var expectedExceptions = new[] { typeof(MapZone) };

            var failures = AllPublicTypes
                .Where(t => typeof(IEquatable<>).MakeGenericType(t).IsAssignableFrom(t))
                .Where(t => t.GetMethod("op_Equality") is null)
                .Except(expectedExceptions)
                .ToList();
            Assert.IsEmpty(failures);
        }

        [Test]
        public void ComparisonOperatorForComparable()
        {
            var expectedExceptions = new Type[] { };

            var failures = AllPublicTypes
                .Where(t => typeof(IComparable<>).MakeGenericType(t).IsAssignableFrom(t))
                .Where(t => t.GetMethod("op_LessThan") is null || t.GetMethod("op_LessThanOrEqual") is null)
                .Except(expectedExceptions)
                .ToList();
            Assert.IsEmpty(failures);
        }
    }
}
