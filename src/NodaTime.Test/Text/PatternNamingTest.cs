// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Text
{
    public class PatternNamingTest
    {
        // https://github.com/nodatime/nodatime/issues/556
        [Test]
        public void NoPatternSuffix()
        {
            var properties =
                from type in typeof(Instant).GetTypeInfo().Assembly.DefinedTypes
                where type.Name.EndsWith("Pattern")
                from property in type.DeclaredProperties
                where property.GetMethod?.IsStatic == true && property.Name.EndsWith("Pattern")
                select $"{type.Name}.{property.Name}";
            Assert.IsEmpty(properties, $"Badly named properties: {string.Join(", ", properties)}");
        }
    }
}
