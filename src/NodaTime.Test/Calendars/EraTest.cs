// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Globalization;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Calendars
{
    public class EraTest
    {
        private static readonly IEnumerable<NamedWrapper<Era>> Eras = typeof(Era).GetTypeInfo()
            .DeclaredProperties // TODO: Only static and public ones...
            .Where(property => property.PropertyType == typeof(Era))
            .Select(property => new NamedWrapper<Era>((Era) property.GetValue(null, null)!, property.Name));

        [Test]
        [TestCaseSource(nameof(Eras))]
        public void ResourcePresence(NamedWrapper<Era> eraWrapper)
        {
            var era = eraWrapper.Value;
            var valueByName = PatternResources.ResourceManager.GetString(era.ResourceIdentifier, CultureInfo.InvariantCulture);
            Assert.NotNull(valueByName, "Missing resource for " + era.ResourceIdentifier);
        }

        [Test]
        [TestCaseSource(nameof(Eras))]
        public void ToStringReturnsName(NamedWrapper<Era> eraWrapper)
        {
            var era = eraWrapper.Value;
            Assert.AreEqual(era.Name, era.ToString());
        }
    }
}
