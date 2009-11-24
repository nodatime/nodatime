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

using NodaTime.Fields;

namespace NodaTime.Periods
{
    /// <summary>
    /// Original name: Days.
    /// </summary>
    public sealed class Days : SingleFieldPeriodBase
    {
        public static readonly Days Zero = new Days(0);
        public static readonly Days One = new Days(1);
        public static readonly Days Two = new Days(2);
        public static readonly Days Three = new Days(3);
        public static readonly Days Four = new Days(4);
        public static readonly Days Five = new Days(5);
        public static readonly Days Six = new Days(6);
        public static readonly Days Seven = new Days(7);
        public static readonly Days MinValue = new Days(int.MinValue);
        public static readonly Days MaxValue = new Days(int.MaxValue);

        private Days(int days) : base(days) { }

        public new int Value { get { return base.Value; } }

        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Days; }
        }

        public override PeriodType PeriodType
        {
            get { return PeriodType.Days; }
        }
    }
}
