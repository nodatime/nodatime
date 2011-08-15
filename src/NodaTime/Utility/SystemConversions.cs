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

using System;

namespace NodaTime.Utility
{
    /// <summary>
    /// Provides conversions between .NET system types and NodeTime types. Not all possible
    /// conversions are present, just the one necessary to support NodaTime functionality.
    /// </summary>
    internal static class SystemConversions
    {
        internal static readonly long DateTimeEpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        internal static Instant DateTimeToInstant(DateTime dateTime)
        {
            var dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return new Instant(dt.Ticks - DateTimeEpochTicks);
        }

        internal static DateTime InstantToDateTime(Instant instant)
        {
            return new DateTime(instant.Ticks + DateTimeEpochTicks, DateTimeKind.Utc);
        }
    }
}