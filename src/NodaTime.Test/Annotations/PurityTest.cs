// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Annotations
{
    /// <summary>
    /// Tests for annotations around purity. No sniggering at the back, please.
    /// </summary>
    [TestFixture]
    public class PurityTest
    {
        [Test]
        public void AllPublicStructMethodsArePure()
        {
            var implicitlyPureNames = new HashSet<string> { "Equals", "GetHashCode", "CompareTo", "ToString" };

            var impureMethods = typeof(Instant).Assembly
                                               .GetTypes()
                                               .Where(t => t.IsValueType && t.IsPublic)
                                               .OrderBy(t => t.Name)
                                               .SelectMany(m => m.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                                               .Where(m => !m.IsSpecialName) // Real methods, not properties
                                               .Where(m => !implicitlyPureNames.Contains(m.Name))
                                               .Where(m => !m.IsDefined(typeof(PureAttribute), false))
                                               .ToList();
            Assert.IsEmpty(impureMethods, "Impure methods: " + string.Join(", ", impureMethods.Select(m => m.ReflectedType.Name + "." + m.Name)));
        }
    }
}
