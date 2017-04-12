// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Attribute to effectively ignore a particular kind of test, because it's known
    /// not to apply to this member. The optional message isn't stored, because we never
    /// need it - it's for documentation purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TestExemptionAttribute : Attribute
    {
        internal TestExemptionCategory Category { get; }

        internal TestExemptionAttribute(TestExemptionCategory category, string message = null)
        {
            Category = category;
        }
    }

    internal enum TestExemptionCategory
    {
        ConversionName = 1
    }
}
