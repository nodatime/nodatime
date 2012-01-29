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

namespace NodaTime.Fields
{
    /// <summary>
    /// Base class for imprecise period fields - i.e. where the duration of a single value depends on when the value occurs.
    /// (For example, months and years.) Derived classes need only override Add and GetInt64Difference.
    /// </summary>
    internal abstract class ImprecisePeriodField : PeriodField
    {
        internal ImprecisePeriodField(PeriodFieldType fieldType, long averageUnitTicks)
            : base(fieldType, averageUnitTicks, false, true)
        {
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return GetInt64Difference(localInstant + duration, localInstant);
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return Add(localInstant, value) - localInstant;
        }
    }
}
