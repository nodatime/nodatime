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

using System;
using System.Windows.Controls;
using NodaTime.ZoneInfoCompiler;

namespace ZoneInfoCompilerW
{
    /// <summary>
    ///   Provides an <see cref = "ILog" /> implementation that appends all of the messages
    ///   to be logged to a <see cref = "ListBox" />.
    /// </summary>
    internal class ListBoxLog : LogBase
    {
        private readonly ListBox control;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ListBoxLog" /> class.
        /// </summary>
        /// <param name = "control">The list box to append to.</param>
        public ListBoxLog(ListBox control)
        {
            this.control = control;
        }

        /// <summary>
        ///   Called to actually log the message to where ever the logger sends its output. The
        ///   destination can be different based on the message type and different loggers may not
        ///   send all messages to the destination.
        /// </summary>
        /// <param name = "type">The type of log message.</param>
        /// <param name = "message">The message to log.</param>
        protected override void LogMessage(LogType type, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            control.Items.InvokeIfRequired(() =>
                                           {
                                               var item = new ListBoxLogItem(type, message);
                                               control.Items.Add(item);
                                               control.ScrollIntoView(item);
                                           });
        }
    }
}
