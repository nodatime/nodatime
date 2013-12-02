// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Annotations
{
    /// <summary>
    /// Attribute indicating that a particular member would normally be private (or potentially protected)
    /// but is exposed for test purposes.
    /// </summary>
    /// <remarks>
    /// Currently this excludes nested types, fields, and events - but it could be expanded to do so. Likewise
    /// we don't indicate the intended access mode, which could be done via an enum. For the moment we'll
    /// assume everything would be private apart from for testing.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    internal class VisibleForTestingAttribute : Attribute
    {
    }
}
