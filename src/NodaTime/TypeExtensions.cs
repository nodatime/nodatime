// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime
{
#if NET40
    /// <summary>
    /// Compatibility functions around the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Get the type info (introduced with .NET 4.5 as extension method)
        /// </summary>
        /// <param name="type">The type to get the info for</param>
        /// <returns>The type information</returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
#endif
}
