// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using System.Reflection;

namespace NodaTime.Benchmarks.Framework
{
    internal sealed class BenchmarkRunner
    {
        private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        // Most of the options are relevant for the runner, so it's not worth splitting them out
        // into *just* the bits the runner is interested in.
        private readonly BenchmarkOptions options;
        private readonly BenchmarkResultHandler resultHandler;

        internal BenchmarkRunner(BenchmarkOptions options, BenchmarkResultHandler resultHandler)
        {
            this.options = options;
            this.resultHandler = resultHandler;
        }

        internal void RunTests()
        {
            resultHandler.HandleStartRun(options);
            var types = typeof(Program).Assembly
                                       .GetTypes()
                                       .OrderBy(type => type.FullName)
                                       .Where(type => type.GetMethods(AllInstance)
                                       .Any(IsBenchmark));

            foreach (Type type in types)
            {
                if (options.TypeFilter != null && type.Name != options.TypeFilter)
                {
                    continue;
                }

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    resultHandler.HandleWarning(string.Format("Ignoring {0}: no public parameterless constructor", type.Name));
                    continue;
                }
                resultHandler.HandleStartType(type);
                object instance = ctor.Invoke(null);
                foreach (var method in type.GetMethods(AllInstance).Where(IsBenchmark))
                {
                    if (options.MethodFilter != null && method.Name != options.MethodFilter)
                    {
                        continue;
                    }

                    if (method.GetParameters().Length != 0)
                    {
                        resultHandler.HandleWarning(string.Format("Ignoring {0}: method has parameters", method.Name));
                        continue;
                    }
                    BenchmarkResult result = RunBenchmark(method, instance, options);
                    if (result.Duration == Duration.Zero)
                    {
                        resultHandler.HandleWarning(string.Format("Test {0} had zero duration; no useful result.", result.Method.Name));
                    }
                    resultHandler.HandleResult(result);
                }
                resultHandler.HandleEndType();
            }
            resultHandler.HandleEndRun();
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
                    double scaledIterations = scale * iterations;
                    // Make sure we never end up overflowing...
                    iterations = (int)Math.Min(scaledIterations, int.MaxValue - 1);
                    break;
                }
                // Make sure we don't end up overflowing due to doubling...
                if (iterations >= int.MaxValue / 2)
                {
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
