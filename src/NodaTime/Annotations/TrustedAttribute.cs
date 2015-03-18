// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Indicates that a parameter is trusted to be valid, so callers must take care
    /// to only pass valid values.
    /// </summary>
    /// <remarks>
    /// <para>This attribute should never be applied to parameters in public members, as
    /// all public members should validate their parameters. The exception here is
    /// public members within internal types, as those aren't really exposed publicly.</para>
    /// <para>Parameters decorated with this attribute should typically be validated in
    /// debug configurations, using <see cref="Preconditions.DebugCheckArgumentRange(string,int,int,int)"/>
    /// or a similar method.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class TrustedAttribute : Attribute
    {
    }
}
