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
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Tests for <see cref="DurationFieldValue"/>. Currently fairly bare, as the struct itself is trivial...
    /// but it may get bigger.
    /// </summary>
    [TestFixture]
    public class DurationFieldValueTest
    {
        [Test]
        public void Construction_RetainsValues()
        {
            DurationFieldValue fieldValue = new DurationFieldValue(DurationFieldType.Months, 15);
            Assert.AreEqual(DurationFieldType.Months, fieldValue.FieldType);
            Assert.AreEqual(15, fieldValue.Value);
        }
    }
}
