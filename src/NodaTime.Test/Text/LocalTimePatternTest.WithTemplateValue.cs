#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    public partial class LocalTimePatternTest
    {
        [Test]
        public void TemplateValue_DefaultsToMidnight()
        {
            var pattern = LocalTimePattern.CreateWithInvariantInfo("HH");
            Assert.AreEqual(LocalTime.Midnight, pattern.TemplateValue);
        }

        [Test]
        public void WithTemplateValue_PropertyFetch()
        {
            LocalTime newValue = new LocalTime(1, 23, 45);
            var pattern = LocalTimePattern.CreateWithInvariantInfo("HH").WithTemplateValue(newValue);
            Assert.AreEqual(newValue, pattern.TemplateValue);
        }

        [Test]
        [TestCaseSource(typeof(LocalTimePatternTestSupport), "TemplateValueData")]
        public void Parse_WithTemplateValue(LocalTimePatternTestSupport.LocalTimeData data)
        {
            LocalTimePattern pattern = LocalTimePattern.CreateWithInvariantInfo(data.P)
                .WithCulture(data.C)
                .WithTemplateValue(data.TemplateValue);
            LocalTime parsed = pattern.Parse(data.S).Value;
            Assert.AreEqual(data.V, parsed);
        }
    }
}
