// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Test
{
    /// <summary>
    /// Test parameters are converted into strings to generate
    /// test names; this can cause problems either due to non-uniqueness,
    /// or due to brackets in the name. This wrapper allows custom names to
    /// be provided, and automatically converts round brackets to square ones.
    /// </summary>
    /// <typeparam name="T">Type of underlying value</typeparam>
    public class NamedWrapper<T>
    {
        public T Value { get; }
        private readonly string name;

        public NamedWrapper(T value, string name)
        {
            Value = value;
            this.name = name;
        }

        public override string ToString() => SanitizeName(name);

        internal static string SanitizeName(string name) => name.Replace('(', '[').Replace(')', ']');
    }
}
