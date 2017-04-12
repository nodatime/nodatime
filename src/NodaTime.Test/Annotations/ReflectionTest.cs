// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Annotations;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NodaTime.Test.Annotations
{
    /// <summary>
    /// Not necessary annotations, but a general purpose "anything using reflection" test.
    /// </summary>
    public class ReflectionTest
    {
        [Test]
        public void ConversionNamesMatchTargetType()
        {
            Regex conversionName = new Regex("^To([A-Z][a-zA-Z0-9]*)$");

            var badMethods = typeof(Instant).GetTypeInfo().Assembly
                .DefinedTypes
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .OrderBy(t => t.Name)
                .SelectMany(m => m.DeclaredMethods)
                .Where(m => m.IsPublic)
                .Where(m => conversionName.IsMatch(m.Name))
                .Where(m => !m.Name.StartsWith("To" + m.ReturnType.Name));

            TestHelper.AssertNoFailures(badMethods, m => $"{m.DeclaringType.Name}.{m.Name}", TestExemptionCategory.ConversionName);
        }
    }
}
