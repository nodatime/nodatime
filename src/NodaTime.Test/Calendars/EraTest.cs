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
    public class EraTest
    {
        private static readonly IEnumerable<Era> Eras = typeof(Era).GetTypeInfo()
                                                                   .DeclaredProperties // TODO: Only static and public ones...
                                                                   .Where(property => property.PropertyType == typeof(Era))
                                                                   .Select(property => property.GetValue(null, null))
                                                                   .Cast<Era>();

        [TestCaseSource(nameof(Eras))]
        [Test]
        public void ResourcePresence(Era era)
        {
            var valueByName = PatternResources.ResourceManager.GetString(era.ResourceIdentifier, CultureInfo.InvariantCulture);
            Assert.NotNull(valueByName, "Missing resource for " + era.ResourceIdentifier);
            // Check that we *could* get the property by name as well... not that we need to.
            var valueByProperty = (string) typeof(PatternResources).GetTypeInfo()
                .GetProperty(era.ResourceIdentifier, BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
            Assert.AreEqual(valueByName, valueByProperty);
        }
    }
}
