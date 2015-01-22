// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using NodaTime.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Annotations
{
    [TestFixture]
    public class MutabilityTest
    {
        [Test]
        public void AllPublicClassesAreMutableOrImmutable()
        {
            var unannotatedClasses = typeof(Instant).Assembly
                                                    .GetTypes()
                                                    .Concat(new[] { typeof(ZonedDateTime.Comparer) })
                                                    .Where(t => t.IsClass && t.IsPublic && t.BaseType != typeof(MulticastDelegate))
                                                    .Where(t => !(t.IsAbstract && t.IsSealed)) // Ignore static classes
                                                    .OrderBy(t => t.Name)
                                                    .Where(t => !t.IsDefined(typeof(ImmutableAttribute), false) &&
                                                                !t.IsDefined(typeof(MutableAttribute), false))
                                                    .ToList();
            var type = typeof (ZonedDateTime.Comparer);
            Console.WriteLine(type.IsClass && type.IsPublic && type.BaseType != typeof (MulticastDelegate));
            Console.WriteLine(!(type.IsAbstract && type.IsSealed));
            Assert.IsEmpty(unannotatedClasses, "Unannotated classes: " + string.Join(", ", unannotatedClasses.Select(c => c.Name)));
        }

    }
}
