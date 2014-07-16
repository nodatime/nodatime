// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that a value-type field which would otherwise by <c>readonly</c>
    /// is read/write so that invoking members on the field avoids taking a copy of
    /// the field value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ReadWriteForEfficiencyAttribute : Attribute
    {
    }
}
