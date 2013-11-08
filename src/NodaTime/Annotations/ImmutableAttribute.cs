// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that a type is mutable.
    /// </summary>
    /// <remarks>
    /// Some types may be publicly immutable, but contain privately mutable
    /// aspects, e.g. caches. If it proves to be useful to indicate the kind of
    /// immutability we're implementing, we can add an appropriate property to this
    /// attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class ImmutableAttribute : Attribute
    {
    }
}
