// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Xml.Linq;

namespace NodaTime.Benchmarks.Analyzer
{
    /// <summary>
    /// Similar to BenchmarkResult in NodaTime.Benchmarks, but with full type and namespace information, and
    /// parsing from XElement.
    /// </summary>
    internal sealed class BenchmarkResult
    {
        private const long TicksPerPicosecond = 100 * 1000L;
        private const long TicksPerNanosecond = 100;

        private readonly string type;
        private readonly string clrNamespace;
        private readonly string method;
        private readonly int iterations;
        private readonly Duration duration;

        internal string Type { get { return type; } }
        internal string Namespace { get { return clrNamespace; } }
        internal string Method { get { return method; } }
        internal string FullyQualifiedMethod { get { return string.Format("{0}.{1}.{2}", Namespace, Type, Method); } }
        internal int Iterations { get { return iterations; } }
        internal Duration Duration { get { return duration; } }
        internal long CallsPerSecond { get { return iterations * NodaConstants.TicksPerSecond / duration.Ticks; } }
        internal long TicksPerCall { get { return Duration.Ticks / iterations; } }
        internal long PicosecondsPerCall { get { return (Duration.Ticks * TicksPerPicosecond) / iterations; } }
        internal long NanosecondsPerCall { get { return (Duration.Ticks * TicksPerNanosecond) / iterations; } }

        private BenchmarkResult(string clrNamespace, string type, string method, int iterations, Duration duration)
        {
            this.clrNamespace = clrNamespace;
            this.type = type;
            this.method = method;
            this.iterations = iterations;
            this.duration = duration;
        }

        internal static BenchmarkResult FromXElement(XElement element)
        {
            XElement typeElement = element.Parent;
            return new BenchmarkResult(
                clrNamespace: typeElement.Attribute("namespace").Value,
                type: typeElement.Attribute("name").Value,
                method: element.Attribute("method").Value,
                iterations: (int)element.Attribute("iterations"),
                duration: Duration.FromTicks((long)element.Attribute("duration")));
        }
    }
}
