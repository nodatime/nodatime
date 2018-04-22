// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using BenchmarkDotNet.Attributes;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class OffsetBenchmarks
    {
        private static readonly Offset SampleOffset = Offset.FromHoursAndMinutes(12, 34);

        private readonly IPattern<Offset> offsetPattern;

        public OffsetBenchmarks()
        {
            offsetPattern = OffsetPattern.CreateWithInvariantCulture("HH:mm");
        }
        
        [Benchmark]
        public void ParseExactIncludingPreparse_Valid()
        {
            var pattern = OffsetPattern.CreateWithInvariantCulture("HH:mm");
            ParseResult<Offset> parseResult = pattern.Parse("12:34");
            parseResult.TryGetValue(default(Offset), out Offset result);
        }

        [Benchmark]
        public void PreparsedParseExact_Valid()
        {
            ParseResult<Offset> parseResult = offsetPattern.Parse("12:34");
            parseResult.TryGetValue(default(Offset), out Offset result);
        }

        [Benchmark]
        public void ParsePattern_Valid()
        {
            OffsetPattern.CreateWithInvariantCulture("HH:mm");
        }

        [Benchmark]
        public void PreparedParseExact_InvalidValue()
        {
            ParseResult<Offset> parseResult = offsetPattern.Parse("123:45");
            parseResult.TryGetValue(default(Offset), out Offset result);
        }

        [Benchmark]
        public void ToString_ExplicitFormat()
        {
            SampleOffset.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void ParsedPattern_Format()
        {
            offsetPattern.Format(SampleOffset);
        }
    }
}
