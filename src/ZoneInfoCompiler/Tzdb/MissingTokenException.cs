using System;

namespace NodaTime.ZoneInfoCompiler.Tzdb
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
