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
using System.Runtime.Serialization;

namespace NodaTime
{
    /// <summary>
    /// Original name: IllegalFieldValueException. We may not need this class
    /// at all, but it does have logic in it, so I thought I'd at least include it for the moment.
    /// </summary>
    [Serializable]
    public class IllegalFieldValueException : ArgumentOutOfRangeException
    {
        public IllegalFieldValueException() {}

        public IllegalFieldValueException(string paramName)
            : base(paramName) {}

        public IllegalFieldValueException(string message, Exception innerException)
            : base(message, innerException) {}

        public IllegalFieldValueException(string paramName, string message)
            : base(paramName, message) {}

        public IllegalFieldValueException(string paramName, string actualValue, string message)
            : base(paramName, actualValue, message) {}

        protected IllegalFieldValueException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}