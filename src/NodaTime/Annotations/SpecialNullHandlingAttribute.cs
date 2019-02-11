// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that the parameter doesn't conform to simple NotNull/CanBeNull
    /// behaviour, e.g. for IPattern{T}.Parse, where the parameter shouldn't be null,
    /// but the result will be a failed ParseResult rather than an ArgumentNullException.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    internal sealed class SpecialNullHandlingAttribute : Attribute { }
}
