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
using NodaTime.Base;

namespace NodaTime
{
    /// <summary>
    /// Original name: Days.
    /// </summary>
    public sealed class Days : SingleFieldPeriodBase
    {
        private static Days zero = new Days(0);
        public static Days Zero { get { return zero; } }
        private static Days one = new Days(1);
        public static Days One { get { return one; } }
        private static Days two = new Days(2);
        public static Days Two { get { return two; } }
        private static Days three = new Days(3);
        public static Days Three { get { return three; } }
        private static Days four = new Days(4);
        public static Days Four { get { return four; } }
        private static Days five = new Days(5);
        public static Days Five { get { return five; } }
        private static Days six = new Days(6);
        public static Days Six { get { return six; } }
        private static Days seven = new Days(7);
        public static Days Seven { get { return seven; } }
        private static Days minValue = new Days(int.MinValue);
        public static Days MinValue { get { return minValue; } }
        private static Days maxValue = new Days(int.MaxValue);
        public static Days MaxValue { get { return maxValue; } }

        private Days(int days) : base(days) { }

        public int Value { get { return value; } }

        public override DurationFieldType FieldType
        {
            get { throw new System.NotImplementedException(); }
        }

        public override PeriodType PeriodType
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
