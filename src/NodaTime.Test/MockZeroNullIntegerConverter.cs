#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Converters;
using NodaTime.Format;

namespace NodaTime.Test
{
    public class MockZeroNullIntegerConverter : IInstantConverter
    {
        public static readonly IInstantConverter Instance = new MockZeroNullIntegerConverter();

        public long GetInstantMilliseconds(object obj, Chronology chrono)
        {
            return 0;
        }

        public long GetInstantMilliseconds(object obj, Chronology chrono, DateTimeFormatter parser)
        {
            return 0;
        }

        public Chronology GetChronology(object obj, IDateTimeZone zone)
        {
            return null;
        }

        public Chronology GetChronology(object obj, Chronology chrono)
        {
            return null;
        }

        public Type GetSupportedType()
        {
            return typeof (int);
        }
    }
}