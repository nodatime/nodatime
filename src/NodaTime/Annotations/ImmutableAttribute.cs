// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that a type is immutable. After construction, the publicly visible
    /// state of the object will not change.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This attribute only applies to types, not fields:
    /// it's entirely feasible to have a readonly field of a mutable type, or a read/write
    /// field of an immutable type. In such cases for reference types (classes and interfaces)
    /// it's important to distinguish between the value of the variable (a reference) and the
    /// object it refers to. Value types are more complicated as in some cases the compiler
    /// will copy values before operating on them; however as all value types in Noda Time are
    /// immutable (aside from explictly implemented serialization operations) this rarely causes
    /// an issue.
    /// </p>
    /// <p>
    /// Some types may be publicly immutable, but contain privately mutable
    /// aspects, e.g. caches. If it proves to be useful to indicate the kind of
    /// immutability we're implementing, we can add an appropriate property to this
    /// attribute.
    /// </p>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class ImmutableAttribute : Attribute
    {
    }
}
