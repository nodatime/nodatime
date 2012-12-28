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
#if !PCL
using System.Runtime.Serialization;
#endif

namespace NodaTime.Text
{
    /// <summary>
    /// Exception thrown to indicate that the specified value could not be parsed.
    /// </summary>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    public sealed class UnparsableValueException : FormatException
    {
        /// <summary>
        /// Creates a new UnparsableValueException with no message.
        /// </summary>
        public UnparsableValueException()
        {
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message.
        /// </summary>
        /// <param name="message">The failure message</param>
        public UnparsableValueException(string message)
            : base(message)
        {
        }
#if !PCL
        /// <summary>
        /// Creates a new UnparsableValueException from the given serialization information.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private UnparsableValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
