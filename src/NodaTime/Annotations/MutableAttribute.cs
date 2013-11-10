 // Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that a type is mutable. Some members of this type
    /// allow state to be visibly changed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class MutableAttribute : Attribute
    {
    }
}
