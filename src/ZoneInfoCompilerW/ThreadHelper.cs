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
using System.Threading;
using System.Windows.Threading;

namespace ZoneInfoCompilerW
{
    /// <summary>
    ///   Provides various helper and extension methods for handling threads.
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        ///   Simple helper extension method to marshall to correct thread if its required
        /// </summary>
        /// <param name = "control">The source control.</param>
        /// <param name = "methodcall">The method to call.</param>
        public static void InvokeIfRequired(this DispatcherObject control, Action methodcall)
        {
            control.InvokeIfRequired(methodcall, DispatcherPriority.Background);
        }

        /// <summary>
        ///   Simple helper extension method to marshall to correct thread if its required
        /// </summary>
        /// <param name = "control">The source control.</param>
        /// <param name = "methodcall">The method to call.</param>
        /// <param name = "priorityForCall">The thread priority.</param>
        public static void InvokeIfRequired(this DispatcherObject control, Action methodcall, DispatcherPriority priorityForCall)
        {
            //see if we need to Invoke call to Dispatcher thread
            if (control.Dispatcher.Thread != Thread.CurrentThread)
            {
                control.Dispatcher.Invoke(priorityForCall, methodcall);
            }
            else
            {
                methodcall();
            }
        }
    }
}
