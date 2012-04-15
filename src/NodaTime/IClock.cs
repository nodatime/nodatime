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

namespace NodaTime
{
    /// <summary>
    /// Represents a clock which can tell the current time as an <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// Although it's not strictly incorrect to call <c>SystemClock.Instance.Now</c> directly,
    /// in the same way as you might call <see cref="DateTime.UtcNow"/>, it's strongly discouraged
    /// as a matter of style for production code. We recommend providing an instance of <see cref="IClock"/>
    /// to anything that needs it, which allows you to write tests using the stub clock in the NodaTime.Testing
    /// assembly (or your own implementation).
    /// </remarks>
    public interface IClock
    {
        /// <summary>
        /// Gets the current instant in time according to this clock. This is an instant on
        /// the time line which is independent of both time zone and calendar system.
        /// </summary>
        Instant Now { get; }
    }
}
