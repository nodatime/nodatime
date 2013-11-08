using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
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
                                                    .Where(t => t.IsClass && t.IsPublic && t.BaseType != typeof(MulticastDelegate))
                                                    .Where(t => !(t.IsAbstract && t.IsSealed)) // Ignore static classes
                                                    .OrderBy(t => t.Name)
                                                    .Where(t => !t.IsDefined(typeof(ImmutableAttribute), false) &&
                                                                !t.IsDefined(typeof(MutableAttribute), false))
                                                    .ToList();
            Assert.IsEmpty(unannotatedClasses, "Unannotated classes: " + string.Join(", ", unannotatedClasses.Select(c => c.Name)));
        }

    }
}
