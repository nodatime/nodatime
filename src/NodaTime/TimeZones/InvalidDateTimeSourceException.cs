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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Exception thrown to indicate that a time zone source has violated the conditions of <see cref="IDateTimeZoneSource"/>.
    /// This exception is primarily intended to be thrown from <see cref="DateTimeZoneCache"/>, and only in the face of a buggy
    /// source; user code should not 
    /// </summary>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    public sealed class InvalidDateTimeZoneSourceException : Exception
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="message">The local date time which is skipped in the specified time zone.</param>
        public InvalidDateTimeZoneSourceException(string message)
            : base(message)
        {
        }
    }
}