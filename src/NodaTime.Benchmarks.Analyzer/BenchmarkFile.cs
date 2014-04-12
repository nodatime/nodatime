// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace NodaTime.Benchmarks.Analyzer
{
    internal sealed class BenchmarkFile
    {
        private readonly string machine;
        private readonly string label;
        private readonly Instant startTime;
        private readonly ReadOnlyCollection<BenchmarkResult> results;

        public string Machine { get { return machine; } }
        public string Label { get { return label; } }
        public Instant StartTime { get { return startTime; } }
        public ReadOnlyCollection<BenchmarkResult> Results { get { return results; } }

        internal BenchmarkFile(string machine, string label, Instant startTime, ReadOnlyCollection<BenchmarkResult> results)
        {
            this.machine = machine;
            this.label = label;
            this.startTime = startTime;
            this.results = results;
        }

        internal static BenchmarkFile FromXDocument(XDocument document)
        {
            var root = document.Root;
            return new BenchmarkFile(
                machine: root.Element("environment").Attribute("machine").Value,
                label: (string)root.Attribute("label"),
                startTime: Instant.FromDateTimeUtc((DateTime)root.Attribute("start")),
                results: root.Descendants("test").Select(x => BenchmarkResult.FromXElement(x)).ToList().AsReadOnly());
        }
    }
}
