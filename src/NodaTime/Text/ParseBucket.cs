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

using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// Base class for "buckets" of parse data - as field values are parsed, they are stored in a bucket,
    /// then the final value is calculated at the end.
    /// </summary>
    internal abstract class ParseBucket<T>
    {
        /// <summary>
        /// Performs the final conversion from fields to a value. The parse can still fail here, if there
        /// are incompatible field values.
        /// </summary>
        /// <param name="usedFields">Indicates which fields were part of the original text pattern.</param>
        /// <param name="templateValue">The template value to use for unspecified fields.</param>
        internal abstract ParseResult<T> CalculateValue(PatternFields usedFields);

        /// <summary>
        /// Convenience method to check whether a particular field has been used. It's here as it'll primarily
        /// be used by buckets; ideally we'd make it an extension method on PatternFields, or use Unconstrained
        /// Melody...
        /// </summary>
        internal static bool IsFieldUsed(PatternFields usedFields, PatternFields fieldToTest)
        {
            return (usedFields & fieldToTest) != 0;
        }
    }
}
