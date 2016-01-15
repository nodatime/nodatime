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
    }
}
