// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Xml.Linq;

namespace NodaTime.Benchmarks.Framework
{
    internal sealed class XmlResultHandler : BenchmarkResultHandler
    {
        private readonly XDocument document;
        private XElement currentType;
        private XElement warnings;
        private readonly string path;

        internal XmlResultHandler(string path)
        {
            document = new XDocument(new XElement("benchmark"));
            document.Root.Add(new XAttribute("start", SystemClock.Instance.Now.ToDateTimeUtc()));
            this.path = path;
        }

        internal override void HandleStartRun(BenchmarkOptions options)
        {
            document.Root.Add(new XElement("environment",
                new XAttribute("runtime", Environment.Version),
                new XAttribute("os", Environment.OSVersion),
                new XAttribute("cores", Environment.ProcessorCount),
                new XAttribute("is-64bit-process", Environment.Is64BitProcess),
                new XAttribute("is-64bit-os", Environment.Is64BitOperatingSystem),
                new XAttribute("machine", Environment.MachineName)));
            document.Root.Add(new XElement("options",
                new XAttribute("type-filter", options.TypeFilter ?? ""),
                new XAttribute("method-filter", options.MethodFilter ?? ""),
                new XAttribute("test-time", options.TestTime.ToTimeSpan()),
                new XAttribute("warmup-time", options.WarmUpTime.ToTimeSpan())));
        }

        internal override void HandleStartType(Type type)
        {
            currentType = new XElement("type", new XAttribute("name", type.FullName));
            document.Root.Add(currentType);
        }

        internal override void HandleResult(BenchmarkResult result)
        {
            currentType.Add(new XElement("test",
                new XAttribute("method", result.Method.Name),
                new XAttribute("iterations", result.Iterations),
                new XAttribute("duration", result.Duration.Ticks),
                // These two can be inferred, but just for readability let's include them here too
                new XAttribute("ticks-per-call", result.TicksPerCall),
                new XAttribute("calls-per-second", result.CallsPerSecond)));
        }

        internal override void HandleWarning(string text)
        {
            if (warnings != null)
            {
                warnings = new XElement("warnings");
                document.Root.AddFirst(warnings);
            }
            warnings.Add(new XElement("warning", text));
        }

        internal override void HandleEndRun()
        {
            document.Root.Add(new XAttribute("end", SystemClock.Instance.Now.ToDateTimeUtc()));
            document.Save(path);
        }
    }
}
