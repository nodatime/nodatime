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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NodaTime.Utility
{
    /// <summary>
    /// This class only exists due to the dangers of cycles in type initializers. It has to be
    /// present in the Noda Time assembly to avoid introducing a new dependency, but it has nothing
    /// time-specific, and could be reused in other projects easily. See TypeInitializationTest
    /// in the NodaTime.Test assembly for the corresponding unit tests, and the following blog
    /// post for more details:
    /// TBD
    /// </summary>
    internal sealed class TypeInitializationChecker : MarshalByRefObject
    {
        /// <summary>
        /// The list of dependencies recorded by <see cref="RecordInitializationStart"/>, or null
        /// to avoid recording dependencies.
        /// </summary>
        private static List<Dependency> dependencies = null;

        /// <summary>
        /// The expected entry method for tests: we can stop looking at a stack trace when we find this method.
        /// </summary>
        private static readonly MethodInfo EntryMethod = typeof(TypeInitializationChecker).GetMethod("FindDependencies");

        /// <summary>
        /// If dependency recording is enabled (typically only for a unit test), this checks for type
        /// initializers in the current stack trace and records any dependencies: one type initializer
        /// earlier in the stack trace than another. If dependency recording is disabled, this method is a
        /// no-op.
        /// </summary>
        internal static int RecordInitializationStart()
        {
            if (dependencies == null)
            {
                return 0;
            }
            Type previousType = null;
            foreach (var frame in new StackTrace().GetFrames())
            {
                var method = frame.GetMethod();
                if (method == EntryMethod)
                {
                    break;
                }
                var declaringType = method.DeclaringType;
                if (method == declaringType.TypeInitializer)
                {
                    if (previousType != null)
                    {
                        dependencies.Add(new Dependency(declaringType, previousType));
                    }
                    previousType = declaringType;
                }
            }
            return 0;
        }

        /// <summary>
        /// Invoked from the unit tests, this finds the dependency chain for a single type
        /// by invoking its type initializer.
        /// </summary>
        public Dependency[] FindDependencies(string name)
        {
            dependencies = new List<Dependency>();
            Type type = typeof(TypeInitializationChecker).Assembly.GetType(name, true);
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            return dependencies.ToArray();
        }

        /// <summary>
        /// A simple from/to tuple, which can be marshaled across AppDomains.
        /// </summary>
        internal sealed class Dependency : MarshalByRefObject
        {
            public string From { get; private set; }
            public string To { get; private set; }
            internal Dependency(Type from, Type to)
            {
                From = from.FullName;
                To = to.FullName;
            }

            internal Dependency(string from, string to)
            {
                From = from;
                To = to;
            }
        }
    }
}
