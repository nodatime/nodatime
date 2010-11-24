#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Thrown when an expected token is missing from the token stream.
    /// </summary>
    public class MissingTokenException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MissingTokenException" /> class.
        /// </summary>
        /// <param name = "name">The name.</param>
        public MissingTokenException(string name) : this(name, "Missing token " + name)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MissingTokenException" /> class.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <param name = "message">The message.</param>
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
