// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Testing
{
    /// <summary>
    /// Helper static methods for argument/state validation. Copied from NodaTime.Utility,
    /// as we don't want the Testing assembly to have internal access to NodaTime, but we
    /// don't really want to expose Preconditions publically.
    /// </summary>
    internal static class Preconditions
    {
        /// <summary>
        /// Returns the given argument after checking whether it's null. This is useful for putting
        /// nullity checks in parameters which are passed to base class constructors.
        /// </summary>
        internal static T CheckNotNull<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return argument;
        }
    }
}
