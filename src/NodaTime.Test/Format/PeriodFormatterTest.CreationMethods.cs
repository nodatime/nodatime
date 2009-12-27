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
using System;
using NUnit.Framework;
using NodaTime.Format;
using NodaTime.Periods;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {
        [Test]
        public void WithProvider_CreatesNewInstance()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.IsNull(sutDefault.Provider);
            var sutWithProvider = sutDefault.WithProvider(provider);
            Assert.AreEqual(provider, sutWithProvider.Provider);
            Assert.AreNotSame(sutDefault, sutWithProvider);
        }


        [Test]
        public void WithProvider_ReturnsTheSameInistance_IfNewProviderIsNull()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.IsNull(sutDefault.Provider);
            var sutWithProvider = sutDefault.WithProvider(null);
            Assert.IsNull(sutWithProvider.Provider);
            Assert.AreSame(sutDefault, sutWithProvider);
        }

        [Test]
        public void WithProvider_ReturnsTheSameInstance_IfNewProviderIsTheSame()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithProvider(provider);
            var sutWithProvider = sutDefault.WithProvider(provider);
            Assert.AreSame(sutDefault, sutWithProvider);
        }

        [Test]
        public void WithParsePeriodType_CreatesNewInstance()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.IsNull(sutDefault.ParsePeriodType);
            var sutWithParseType = sutDefault.WithParseType(PeriodType.Days);
            Assert.AreEqual(PeriodType.Days, sutWithParseType.ParsePeriodType);
            Assert.AreNotSame(sutDefault, sutWithParseType);
        }

        [Test]
        public void WithParsePeriodType_ReturnsTheSameInstance_IfNewPeriodTypeIsNull()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            Assert.IsNull(sutDefault.ParsePeriodType);
            var sutWithParseType = sutDefault.WithParseType(null);
            Assert.IsNull(sutWithParseType.ParsePeriodType);
            Assert.AreSame(sutDefault, sutWithParseType);
        }

        [Test]
        public void WithParsePeriodType_ReturnsTheSameInstance_IfNewPeriodTypeIsTheSame()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithParseType(PeriodType.Years);
            var sutWithParseType = sutDefault.WithParseType(PeriodType.Years);
            Assert.AreSame(sutDefault, sutWithParseType);
        }
    }
}
