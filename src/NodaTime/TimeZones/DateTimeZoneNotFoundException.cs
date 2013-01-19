#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
    /// Exception thrown when time zone is requested from an <see cref="IDateTimeZoneProvider"/>,
    /// but the specified ID is invalid for that provider.
    /// </summary>
    /// <remarks>
    /// This type only exists as <c>TimeZoneNotFoundException</c> doesn't exist in the Portable Class Library.
    /// By creating an exception which derives from <c>TimeZoneNotFoundException</c> on the desktop version
    /// and <c>Exception</c> on the PCL version, we achieve reasonable consistency while remaining
    /// backwardly compatible with Noda Time v1 (which was desktop-only, and threw <c>TimeZoneNotFoundException</c>).
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    public class DateTimeZoneNotFoundException
#if PCL
        : Exception
#else
        : TimeZoneNotFoundException
#endif
    {
        /// <summary>
        /// Creates an instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public DateTimeZoneNotFoundException(string message) : base(message) { }
    }
}
