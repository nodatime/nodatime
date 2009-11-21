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

namespace NodaTime.Fields
{
    /// <summary>
    /// An immutable collection of DateTimeFields
    /// </summary>
    public sealed class DateTimeFieldSet
    {
        /// <summary>
        /// Returns the field for the specified type.
        /// </summary>
        /// <remarks>
        /// This will never return null, but the field may be unsupported.
        /// </remarks>
        /// <param name="fieldType">The field type to fetch</param>
        /// <exception cref="ArgumentOutOfRangeException">An invalid
        /// field type is specified.</exception>
        /// <returns>The field for the specified type.</returns>
        public IDateTimeField this[DateTimeFieldType fieldType] { get { throw new NotImplementedException(); } }
    }
}
