// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using NodaTime.Calendars;
using NodaTime.Properties;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class EraTest
    {
        private static readonly IEnumerable<Era> Eras = typeof(Era).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                                   .Where(field => field.FieldType == typeof(Era))
                                                                   .Select(field => field.GetValue(null))
                                                                   .Cast<Era>();

        [TestCaseSource("Eras")]
        [Test]
        public void ResourcePresence(Era era)
        {
            Assert.NotNull(PatternResources.ResourceManager.GetString(era.ResourceIdentifier, CultureInfo.InvariantCulture),
                "Missing resource for " + era.ResourceIdentifier);
        }
    }
}
