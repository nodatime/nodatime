// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Temporary measure until BenchmarkDotNet supports categories.
    /// This allows us to keep our old attributes without losing
    /// any information.
    /// See https://github.com/PerfDotNet/BenchmarkDotNet/issues/248
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal class CategoryAttribute : Attribute
    {
        public string Category { get; set; }

        public CategoryAttribute(string category)
        {
            Category = category;
        }
    }
}
