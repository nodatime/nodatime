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

namespace NodaTime.Fields
{
    /// <summary>
    /// A simple combination of a <see cref="DurationFieldType" /> and a 64-bit integer value.
    /// These are used when representing periods.
    /// </summary>
    public struct DurationFieldValue
    {
        private readonly DurationFieldType fieldType;
        private readonly long value;

        /// <summary>
        /// The <see cref="DurationFieldType"/> component of the value.
        /// </summary>
        public DurationFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// The 64-bit integer component of the value.
        /// </summary>
        public long Value { get { return value; } }

        /// <summary>
        /// Creates a new instance combining the specified type and value.
        /// </summary>
        public DurationFieldValue(DurationFieldType fieldType, long value)
        {
            this.fieldType = fieldType;
            this.value = value;
        }
    }
}
