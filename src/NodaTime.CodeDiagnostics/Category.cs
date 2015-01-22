// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.CodeDiagnostics
{
    internal enum Category
    {
        /// <summary>
        /// Category for diagnostics which might indicate an observable bug.
        /// </summary>
        Correctness,

        /// <summary>
        /// Category for diagnostics which might indicate a lack of declarative
        /// clarity - usually the absence of an attribute.
        /// </summary>
        Clarity,

        /// <summary>
        /// Category for diagnostics which indicate an inconsistent use of attributes,
        /// such as applying [NotNull] to a value type parameter.
        /// </summary>
        Inconsistency
    }
}
