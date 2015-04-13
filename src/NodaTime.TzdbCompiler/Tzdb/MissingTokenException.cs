// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    ///   Thrown when an expected token is missing from the token stream.
    /// </summary>
    public class MissingTokenException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="MissingTokenException" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MissingTokenException(string name) : this(name, "Missing token " + name)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MissingTokenException" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="message">The message.</param>
        public MissingTokenException(string name, string message) : base(message)
        {
            Name = name;
        }

        /// <summary>
        ///   Gets or sets the name of the missing token
        /// </summary>
        /// <value>The token name.</value>
        public string Name { get; private set; }
    }
}
