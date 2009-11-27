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

using NodaTime.Fields;

namespace NodaTime.Periods
{
    /// <summary>
    /// Original name: Period.
    /// </summary>
    public sealed class Period : PeriodBase
    {
        public static readonly Period Zero = new Period();

        public Period()
        {
        }

        public Period(
            int years, int months, int weeks, int days,
            int hours, int minutes, int seconds, int millis)
        {
            throw new NotImplementedException();
        }

        public override PeriodType PeriodType
        {
            get { throw new System.NotImplementedException(); }
        }

        public override int Size
        {
            get { throw new System.NotImplementedException(); }
        }

        public override DurationFieldType GetFieldType(int index)
        {
            throw new System.NotImplementedException();
        }

        public override int GetValue(int index)
        {
            throw new System.NotImplementedException();
        }

        public override int Get(DurationFieldType field)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsSupported(DurationFieldType field)
        {
            throw new System.NotImplementedException();
        }

        public override Period ToPeriod()
        {
            throw new System.NotImplementedException();
        }

        public static Period Years(int years)
        {
            throw new NotImplementedException();
        }

        public static Period Months(int months)
        {
            throw new NotImplementedException();
        }

        public static Period Weeks(int weeks)
        {
            throw new NotImplementedException();
        }

        public static Period Days(int days)
        {
            throw new NotImplementedException();
        }

        public static Period Hours(int hours)
        {
            throw new NotImplementedException();
        }
    }
}