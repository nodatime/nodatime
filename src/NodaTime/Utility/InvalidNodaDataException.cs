// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Utility
{
    /// <summary>
    /// Exception thrown when data read by Noda Time (such as serialized time zone data) is invalid. This includes
    /// data which is truncated, i.e. we expect more data than we can read.
    /// </summary>
    /// <remarks>
    /// This type only exists as <c>InvalidDataException</c> doesn't exist in the Portable Class Library.
    /// Unfortunately, <c>InvalidDataException</c> itself is sealed, so we can't derive from it for the sake
    /// of backward compatibility.
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    public class InvalidNodaDataException : Exception
    {
        /// <summary>
        /// Creates an instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public InvalidNodaDataException(string message) : base(message) { }
    }
}
