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
    /// Original name: DurationFieldType.
    /// Like DateTimeFieldType - may or may not end up as an enum.
    /// </summary>
    /// <remarks>
    /// Values are defined to be compatible with flags. This is so PeriodType can combine these values safely.
    /// </remarks>
    [Flags]
    public enum DurationFieldType
    {
        Eras = 1,
        Centuries = 1 << 1,
        WeekYears = 1 << 2,
        Years = 1 << 3,
        Months = 1 << 4,
        Weeks = 1 << 5,
        Days = 1 << 6,
        HalfDays = 1 << 7,
        Hours = 1 << 8,
        Minutes = 1 << 9,
        Seconds = 1 << 10,
        Milliseconds = 1 << 11,
        Ticks = 1 << 12
    }
}
