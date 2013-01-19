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

namespace NodaTime.Utility
{
    /// <summary>
    /// Exception thrown when data read by Noda Time (such as serialized time zone data) is invalid.
    /// </summary>
    /// <remarks>
    /// This type only exists as <c>InvalidDataException</c> doesn't exist in the Portable Class Library.
    /// Unfortunately, <c>InvalidDataException itself is sealed, so we can't derive from it for the sake
    /// of backward compatibility.</c>
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    // TODO: Derive from IOException instead, like EndOfStreamException does?
    public class InvalidNodaDataException : Exception
    {
        /// <summary>
        /// Creates an instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public InvalidNodaDataException(string message) : base(message) { }
    }
}
