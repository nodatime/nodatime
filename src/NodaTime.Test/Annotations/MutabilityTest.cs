// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using System.Reflection;
using NodaTime.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Annotations
{
    public class MutabilityTest
    {
        [Test]
        public void AllPublicClassesAreMutableOrImmutable()
        {
            var unannotatedClasses = typeof(Instant).GetTypeInfo().Assembly
                                                    .DefinedTypes
                                                    .Where(t => t.IsClass && (t.IsNestedPublic || t.IsPublic) && t.BaseType != typeof(MulticastDelegate))
                                                    .Where(t => !(t.IsAbstract && t.IsSealed)) // Ignore static classes
                                                    .OrderBy(t => t.Name)
                                                    .Where(t => !t.IsDefined(typeof(ImmutableAttribute)) &&
                                                                !t.IsDefined(typeof(MutableAttribute)));

            TestHelper.AssertNoFailures(unannotatedClasses, c => c.Name);
        }
    }
}
