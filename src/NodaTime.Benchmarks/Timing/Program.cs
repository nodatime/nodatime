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

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    internal class Program
    {
        private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static void Main(string[] args)
        {
            BenchmarkOptions options = BenchmarkOptions.FromCommandLine(args);

            var types =
                typeof(Program).Assembly.GetTypes().OrderBy<Type, string>(GetTypeDisplayName).Where(type => type.GetMethods(AllInstance).Any(IsBenchmark));

            var results = new List<BenchmarkResult>();
            foreach (Type type in types)
            {
                if (options.TypeFilter != null && type.Name != options.TypeFilter)
                {
                    continue;
                }

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    Console.WriteLine(@"Ignoring {0}: no public parameterless constructor", type.Name);
                    continue;
                }
                Console.WriteLine(@"Running benchmarks in {0}", GetTypeDisplayName(type));
                object instance = ctor.Invoke(null);
                foreach (var method in type.GetMethods(AllInstance).Where(IsBenchmark))
                {
                    if (options.MethodFilter != null && method.Name != options.MethodFilter)
                    {
                        continue;
                    }

                    if (method.GetParameters().Length != 0)
                    {
                        Console.WriteLine(@"Ignoring {0}: method has parameters", method.Name);
                        continue;
                    }
                    BenchmarkResult result = RunBenchmark(method, instance, options);
                    Console.WriteLine(@"  " + result.ToString(options));
                    results.Add(result);
                }
            }
        }

        private static string GetTypeDisplayName(Type type)
        {
            return type.FullName.Replace("NodaTime.Benchmarks.", "");
        }

        private static BenchmarkResult RunBenchmark(MethodInfo method, object instance, BenchmarkOptions options)
        {
            var action = (Action)Delegate.CreateDelegate(typeof(Action), instance, method);
            // Start small, double until we've hit our warm-up time
            int iterations = 100;
            while (true)
            {
                Duration duration = RunTest(action, iterations, options.Timer);
                if (duration >= options.WarmUpTime)
                {
                    // Scale up the iterations to work out the full test time
                    double scale = ((double)options.TestTime.Ticks) / duration.Ticks;
                    iterations = (int)(iterations * scale);
                    break;
                }
                iterations *= 2;
            }
            Duration testDuration = RunTest(action, iterations, options.Timer);
            return new BenchmarkResult(method, iterations, testDuration);
        }

        private static Duration RunTest(Action action, int iterations, IBenchTimer timer)
        {
            PrepareForTest();
            timer.Reset();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            return timer.ElapsedTime;
        }

        private static void PrepareForTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static bool IsBenchmark(MethodInfo method)
        {
            return method.IsDefined(typeof(BenchmarkAttribute), false);
        }
    }
}
