// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Benchmarks.Framework
{
    /// <summary>
    /// Attribute applied to any benchmark method or class to specify that it belongs
    /// in a particular category. Categories can then be explicitly included or
    /// excluded at execution time. When applied to a class, all benchmarks within that
    /// class are implicitly in that category.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category)
        {
            this.category = category;
        }

        private readonly string category;
        public string Category { get { return category; } }
    }
}
