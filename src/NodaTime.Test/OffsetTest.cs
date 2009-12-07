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

namespace NodaTime.Test
{
    [TestFixture]
    public partial class OffsetTest
    {
        Offset threeHours = MakeOffset(3, 0, 0, 0);
        Offset threeHoursPrime = MakeOffset(3, 0, 0, 0);
        Offset negativeThreeHours = MakeOffset(-3, 0, 0, 0);
        Offset negativeTwelveHours = MakeOffset(-12, 0, 0, 0);

        private static Offset MakeOffset(int hours, int minutes, int seconds, int milliseconds)
        {
            int millis = (hours * NodaConstants.MillisecondsPerHour);
            millis += (minutes * NodaConstants.MillisecondsPerMinute);
            millis += (seconds * NodaConstants.MillisecondsPerSecond);
            millis += milliseconds;
            return new Offset(millis);
        }

        private static string OperatorMessage(Offset left, string op, Offset right)
        {
            return left + " " + op + " " + right;
        }
    }
}
