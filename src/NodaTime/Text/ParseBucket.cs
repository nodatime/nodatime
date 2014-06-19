// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
        /// <param name="value">Complete value being parsed</param>
        internal abstract ParseResult<T> CalculateValue(PatternFields usedFields, string value);

        /// <summary>
        /// Convenience method to check whether a particular field has been used. It's here as it'll primarily
        /// be used by buckets; ideally we'd make it an extension method on PatternFields, or use Unconstrained
        /// Melody...
        /// </summary>
        internal static bool IsFieldUsed(PatternFields usedFields, PatternFields fieldToTest)
        {
            return (usedFields & fieldToTest) != 0;
        }

        /// <summary>
        /// Convenience method to check whether a particular field set of fields has been used. This is
        /// similar to <see cref="IsFieldUsed"/>, except it's expected to be used with multiple fields,
        /// and will only return true if all the specified fields are present.
        /// </summary>
        internal static bool AreAllFieldsUsed(PatternFields usedFields, PatternFields fieldsToTest)
        {
            return (usedFields & fieldsToTest) == fieldsToTest;
        }
    }
}
