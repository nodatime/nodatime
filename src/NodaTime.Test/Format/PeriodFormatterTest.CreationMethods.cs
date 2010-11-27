#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {
        #region WithProvider
        [Test]
        public void WithProvider_CreatesNewInstanceWithGivenProvider_ForNotNullProvider()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.That(sutDefault.Provider, Is.Null, "precondition");

            var sutWithProvider = sutDefault.WithProvider(provider1);

            Assert.That(sutWithProvider.Provider, Is.SameAs(provider1));
            Assert.That(sutWithProvider, Is.Not.SameAs(sutDefault));
        }

        [Test]
        public void WithProvider_CreatesNewInstanceWithGivenProvider_ForDifferentProvider()
        {
            var sutWithProvider1 = PeriodFormatter.FromParser(parser).WithProvider(provider1);

            var sutWithProvider2 = sutWithProvider1.WithProvider(provider2);

            Assert.That(sutWithProvider2.Provider, Is.SameAs(provider2));
            Assert.That(sutWithProvider2, Is.Not.SameAs(sutWithProvider1));
        }

        [Test]
        public void WithProvider_CreatesNewInstanceWithGivenProvider_ForNullProvider()
        {
            var sutWithProvider1 = PeriodFormatter.FromParser(parser).WithProvider(provider1);

            var sutWithNullProvider = sutWithProvider1.WithProvider(null);

            Assert.That(sutWithNullProvider.Provider, Is.Null);
            Assert.That(sutWithNullProvider, Is.Not.SameAs(sutWithProvider1));
        }

        [Test]
        public void WithProvider_ReturnsTheSame_ForNullProvider()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.That(sutDefault.Provider, Is.Null, "precondition");

            var sutWithProvider = sutDefault.WithProvider(null);

            Assert.That(sutWithProvider.Provider, Is.Null);
            Assert.That(sutWithProvider, Is.SameAs(sutDefault));
        }

        [Test]
        public void WithProvider_ReturnsTheSame_ForTheSameProvider()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithProvider(provider1);

            var sutWithProvider = sutDefault.WithProvider(provider1);

            Assert.That(sutWithProvider, Is.SameAs(sutDefault));
        }
        #endregion

        #region WithParseType
        [Test]
        public void WithParsePeriodType_CreatesNewInstanceWithGivenParseType_ForNotNullParseType()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.That(sutDefault.ParsePeriodType, Is.Null, "precondition");

            var sutWithParseType = sutDefault.WithParseType(monthsPeriodType);

            Assert.That(sutWithParseType.ParsePeriodType, Is.SameAs(monthsPeriodType));
            Assert.That(sutWithParseType, Is.Not.SameAs(sutDefault));
        }

        [Test]
        public void WithParsePeriodType_CreatesNewInstanceWithGivenParseType_ForDifferentParseType()
        {
            var sutMonths = PeriodFormatter.FromParser(parser).WithParseType(monthsPeriodType);

            var sutDays = sutMonths.WithParseType(daysPeriodType);

            Assert.That(sutDays.ParsePeriodType, Is.SameAs(daysPeriodType));
            Assert.That(sutDays, Is.Not.SameAs(sutMonths));
        }

        [Test]
        public void WithParsePeriodType_CreatesNewInstanceWithGivenParseType_ForNullParseType()
        {
            var sutMonths = PeriodFormatter.FromParser(parser).WithParseType(monthsPeriodType);

            var sutNullPeriodType = sutMonths.WithParseType(null);

            Assert.That(sutNullPeriodType.ParsePeriodType, Is.Null);
            Assert.That(sutNullPeriodType, Is.Not.SameAs(sutMonths));
        }

        [Test]
        public void WithParsePeriodType_ReturnsTheSame_ForNullPeriodType()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.That(sutDefault.ParsePeriodType, Is.Null, "precondition");

            var sutWithParseType = sutDefault.WithParseType(null);

            Assert.That(sutWithParseType.ParsePeriodType, Is.Null);
            Assert.That(sutWithParseType, Is.SameAs(sutDefault));
        }

        [Test]
        public void WithParsePeriodType_ReturnsTheSame_ForTheSamePeriodType()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithParseType(daysPeriodType);

            var sutWithParseType = sutDefault.WithParseType(daysPeriodType);

            Assert.That(sutWithParseType, Is.SameAs(sutDefault));
        }
        #endregion
    }
}