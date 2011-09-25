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
using NodaTime.Globalization;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class OffsetPatternTest
    {
        [Test]
        public void SingleCharacterStandardPattern()
        {
            NodaFormatInfo formatInfo = NodaFormatInfo.InvariantInfo.Clone();
            formatInfo.OffsetPatternLong = "H";

            Offset offset = Offset.FromHours(5);
            // Long pattern: we need a better way of expressing "the long pattern"...
            IPattern<Offset> pattern = OffsetPattern.Create("l", formatInfo);
            Assert.AreEqual("5", pattern.Format(offset));
        }

        [Test]
        public void SingleCharacterStandardPattern_CharacterIsAlsoNormallyStandard()
        {
            NodaFormatInfo formatInfo = NodaFormatInfo.InvariantInfo.Clone();
            // This means "minutes" not "use the medium pattern".
            formatInfo.OffsetPatternLong = "m";

            Offset offset = Offset.Create(5, 9, 0, 0);
            // Long pattern: we need a better way of expressing "the long pattern"...
            var pattern = OffsetPattern.Create("l", formatInfo);
            Assert.AreEqual("9", pattern.Format(offset));
        }
    }
}
