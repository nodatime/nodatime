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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NodaTime.Utility;

namespace NodaTime.Test.Utility
{
    [TestFixture]
    public class TypeInitializationTest
    {
        [Test]
        public void BuildInitializerLoops()
        {
            Assembly assembly = typeof(TypeInitializationChecker).Assembly;
            var dependencies = new List<TypeInitializationChecker.Dependency>();
            // Test each type in a new AppDomain - we want to see what happens where each type is initialized first.
            // Note: Namespace prefix check is present to get this to survive in test runners which
            // inject extra types. (Seen with JetBrains.Profiler.Core.Instrumentation.DataOnStack.)

            // Checking for the presence of a type initializer beforehand should make this considerably swifter,
            // but comes with an odd risk; see http://tinyurl.com/cljz4nx for details.
            foreach (var type in assembly.GetTypes().Where(t => t.FullName.StartsWith("NodaTime") && t.TypeInitializer != null))
            {
                // Note: this won't be enough to load the assembly in all test runners. In particular, it fails in
                // NCrunch at the moment.
                AppDomainSetup setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
                AppDomain domain = AppDomain.CreateDomain("InitializationTest" + type.Name, AppDomain.CurrentDomain.Evidence, setup);
                var helper = (TypeInitializationChecker)domain.CreateInstanceAndUnwrap(assembly.FullName,
                    typeof(TypeInitializationChecker).FullName);
                // Clone the dependencies in *our* AppDomain so we can unload the test one.
                dependencies.AddRange(helper.FindDependencies(type.FullName)
                                            .Select(dep => new TypeInitializationChecker.Dependency(dep.From, dep.To)));
                AppDomain.Unload(domain);
            }
            var lookup = dependencies.ToLookup(d => d.From, d => d.To);
            // This is less efficient than it might be, but I'm aiming for simplicity: starting at each type
            // which has a dependency, can we make a cycle?
            // See Tarjan's Algorithm in Wikipedia for ways this could be made more efficient.
            // http://en.wikipedia.org/wiki/Tarjan's_strongly_connected_components_algorithm
            foreach (var group in lookup)
            {
                Stack<string> path = new Stack<string>();
                CheckForCycles(group.Key, path, lookup);
            }
        }

        private static void CheckForCycles(string next, Stack<string> path, ILookup<string, string> dependencyLookup)
        {
            if (path.Contains(next))
            {
                Assert.Fail("Type initializer cycle: {0}-{1}", string.Join("-", path.Reverse().ToArray()), next);
            }
            path.Push(next);
            foreach (var candidate in dependencyLookup[next].Distinct())
            {
                CheckForCycles(candidate, path, dependencyLookup);
            }
            path.Pop();
        }
    }
}
