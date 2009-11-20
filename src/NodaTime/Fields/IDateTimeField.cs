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
namespace NodaTime
{
    /// <summary>
    /// Original name: DateTimeField
    /// </summary>
    public interface IDateTimeField
    {
        DateTimeFieldType FieldType { get; }

        // Int32 is more convenient and fine for almost all fields
        int GetValue(LocalInstant localInstant);
        long GetLongValue(LocalInstant localInstant);
        bool IsSupported { get; }
        bool IsLenient { get; }

        // TODO: GetValueAsText

        LocalInstant Add(LocalInstant localInstant, int value);
        LocalInstant Add(LocalInstant localInstant, long value);

        // TODO: Add/AddWrap for partial and field

        int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        long GetLongDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        // Long is always valid here; I don't think we need an overload for SetValue(int)
        LocalInstant SetValue(LocalInstant localInstant, long value);

        // TODO: Set value for partials and text

        DurationField DurationField { get; }
        DurationField RangeDurationField { get; }
        bool IsLeap(LocalInstant localInstant);
        int GetLeapAmount(LocalInstant localInstant);
        DurationField LeapDurationField { get; }
        long GetMaximumValue();
        long GetMaximumValue(LocalInstant localInstant);
        long GetMinimumValue();
        long GetMinimumValue(LocalInstant localInstant);
        LocalInstant RoundFloor(LocalInstant localInstant);
        LocalInstant RoundCeiling(LocalInstant localInstant);
        LocalInstant RoundHalfFloor(LocalInstant localInstant);
        LocalInstant RoundHalfCeiling(LocalInstant localInstant);
        LocalInstant RoundHalfEven(LocalInstant localInstant);
        Duration Remainder(LocalInstant localInstant);
    }
}
