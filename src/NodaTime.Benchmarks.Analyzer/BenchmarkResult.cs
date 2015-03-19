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
    public sealed class BenchmarkResult
    {
        private const long TicksPerPicosecond = 100 * 1000L;
        private const long TicksPerNanosecond = 100;

        public string FullType { get; }
        public string Type { get; }
        public string Namespace { get; }
        public string Method { get; }
        public string FullyQualifiedMethod { get; }
        public int Iterations { get; }
        public Duration Duration { get; }
        public long CallsPerSecond => Iterations * NodaConstants.TicksPerSecond / Duration.Ticks;
        public long TicksPerCall => Duration.Ticks / Iterations;
        public long PicosecondsPerCall  => (Duration.Ticks * TicksPerPicosecond) / Iterations;
        public long NanosecondsPerCall => (Duration.Ticks * TicksPerNanosecond) / Iterations;

        private BenchmarkResult(string clrNamespace, string type, string method, int iterations, Duration duration)
        {
            Namespace = clrNamespace;
            Type = type;
            Method = method;
            FullType = $"{Namespace}.{Type}";
            FullyQualifiedMethod = $"{FullType}.{Method}";
            Iterations = iterations;
            Duration = duration;
        }

        public static BenchmarkResult FromXElement(XElement element)
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
