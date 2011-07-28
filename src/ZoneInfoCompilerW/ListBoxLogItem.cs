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

using System.Windows.Controls;
using NodaTime.ZoneInfoCompiler;

namespace ZoneInfoCompilerW
{
    /// <summary>
    ///   Provides the container object for log messages added to a <see cref = "ListBox" />.
    /// </summary>
    /// <remarks>
    ///   This type is immutable and thread-safe.
    /// </remarks>
    public class ListBoxLogItem
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ListBoxLogItem" /> class.
        /// </summary>
        /// <param name = "type">The log message type.</param>
        /// <param name = "message">The log message.</param>
        public ListBoxLogItem(LogBase.LogType type, string message)
        {
            Type = type;
            Message = message;
        }

        /// <summary>
        ///   The log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///   The log type of this message.
        /// </summary>
        /// <remarks>
        ///   This can be used to style each type differently.
        /// </remarks>
        public LogBase.LogType Type { get; private set; }

        /// <summary>
        ///   Returns a <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Message;
        }
    }
}
