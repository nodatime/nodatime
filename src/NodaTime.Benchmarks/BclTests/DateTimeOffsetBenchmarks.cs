// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks.BclTests
{
    [Category("BCL")]
    public class DateTimeOffsetBenchmarks
    {
        private static readonly DateTimeOffset sample = new DateTimeOffset(2009, 12, 26, 10, 8, 30, 234, TimeSpan.Zero);
        private static readonly DateTimeOffset earlier = new DateTimeOffset(2009, 12, 26, 10, 8, 30, 234, TimeSpan.FromHours(1));
        private static readonly DateTimeOffset later = new DateTimeOffset(2009, 12, 26, 10, 8, 30, 234, TimeSpan.FromHours(-1));

        private static readonly IComparer<DateTimeOffset> defaultComparer = Comparer<DateTimeOffset>.Default;
            
        [Benchmark]
        public void CompareTo()
        {
            sample.CompareTo(earlier);
            sample.CompareTo(sample);
            sample.CompareTo(later);
        }

        [Benchmark]
        public void Comparer_Compare()
        {
            defaultComparer.Compare(sample, earlier);
            defaultComparer.Compare(sample, sample);
            defaultComparer.Compare(sample, later);
        }

        [Benchmark]
        public void Comparison_Operators()
        {
            (sample < earlier).Consume();
#pragma warning disable 1718
            (sample < sample).Consume();
#pragma warning restore 1718
            (sample < later).Consume();
        }

        [Benchmark]
        [Category("Text")]
        public void Format()
        {
            sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).Consume();
        }
    }
}
