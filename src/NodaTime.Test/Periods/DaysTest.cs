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
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    [TestFixture]
    public partial class DaysTest
    {
        // Test in 2002/03 as time zones are more well known
        // (before the late 90's they were all over the place)
        // private static readonly DateTimeZone Paris = DateTimeZone.ForID("Europe/Paris");
        // the previous line is commented out because it fails the test - an exception during the constructor
        // prevents the DaysTest.Contants tests from running (and they actually work!)
    }
}
