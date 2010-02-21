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
using System.IO;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Provides an interface for a simple logging interface. If the LineNumber is greater than 0
    /// then the line number is appended to the message. If the FileName is not null then the file
    /// name is appended to the message.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets or sets the name of the file where the logging ocurred. If null then the log message
        /// is outside of file processing.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the line number currently being processed.
        /// </summary>
        /// <value>The line number.</value>
        int LineNumber { get; set; }

        /// <summary>
        /// Writes an information message to the log. The string is formatted using string.Format().
        /// </summary>
        /// <param name="format">The format string to log.</param>
        /// <param name="arguments">The arguments for the string format if any.</param>
        void Info(string format, params object[] arguments);

        /// <summary>
        /// Writes a warning message to the log. The string is formatted using string.Format().
        /// </summary>
        /// <param name="format">The format string to log.</param>
        /// <param name="arguments">The arguments for the string format if any.</param>
        void Warn(string format, params object[] arguments);

        /// <summary>
        /// Writes an error message to the log. The string is formatted using string.Format().
        /// </summary>
        /// <param name="format">The format string to log.</param>
        /// <param name="arguments">The arguments for the string format if any.</param>
        void Error(string format, params object[] arguments);

        /// <summary>
        /// Gets the <see cref="TextWriter"/> that sends its output to <see cref="Info"/>.
        /// </summary>
        /// <value>The <see cref="TextWriter"/>.</value>
        TextWriter InfoWriter { get; }

        /// <summary>
        /// Gets the <see cref="TextWriter"/> that sends its output to <see cref="Warn"/>.
        /// </summary>
        /// <value>The <see cref="TextWriter"/>.</value>
        TextWriter WarnWriter { get; }

        /// <summary>
        /// Gets the <see cref="TextWriter"/> that sends its output to <see cref="Error"/>.
        /// </summary>
        /// <value>The <see cref="TextWriter"/>.</value>
        TextWriter ErrorWriter { get; }
    }
}
