// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Properties;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class EraTest
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        private static readonly IEnumerable<Era> Eras = typeof(Era).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                                   .Where(property => property.PropertyType == typeof(Era))
                                                                   .Select(property => property.GetValue(null, null))
                                                                   .Cast<Era>();
#pragma warning restore 0414

        [TestCaseSource("Eras")]
        [Test]
        public void ResourcePresence(Era era)
        {
            Assert.NotNull(PatternResources.ResourceManager.GetString(era.ResourceIdentifier, CultureInfo.InvariantCulture),
                "Missing resource for " + era.ResourceIdentifier);
        }
    }
}
