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

using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
	partial class DaysTest
	{
        [Test]
        public void ConstantsTest_Valid()
        {
            Assert.AreEqual(0, Days.Zero.Value);
            Assert.AreEqual(1, Days.One.Value);
            Assert.AreEqual(2, Days.Two.Value);
            Assert.AreEqual(3, Days.Three.Value);
            Assert.AreEqual(4, Days.Four.Value);
            Assert.AreEqual(5, Days.Five.Value);
            Assert.AreEqual(6, Days.Six.Value);
            Assert.AreEqual(7, Days.Seven.Value);
            Assert.AreEqual(int.MaxValue, Days.MaxValue.Value);
            Assert.AreEqual(int.MinValue, Days.MinValue.Value);
        }
	}
}
