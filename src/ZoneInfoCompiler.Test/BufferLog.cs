#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using System.Collections.Generic;
using NodaTime.ZoneInfoCompiler;

namespace ZoneInfoCompiler.Test
{
    /// <summary>
    ///   Provides an implementation of <see cref="ILog" /> that adds the messages to a
    ///   buffer.
    /// </summary>
    internal class BufferLog : LogBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="BufferLog" /> class.
        /// </summary>
        internal BufferLog()
        {
            Lines = new List<string>();
        }

        internal IList<string> Lines { get; private set; }

        /// <summary>
        ///   Called to actually log the message to where ever the logger sends its
        ///   output. The destination can be different based on the type and different
        ///   loggers may not send all messages to the destination.
        /// </summary>
        /// <param name="type">The type of log message.</param>
        /// <param name="message">The message to log.</param>
        protected override void LogMessage(LogType type, string message)
        {
            Lines.Add(type + " " + message);
        }
    }
}
