// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Benchmarks.Custom;

internal class ConfigurationException : Exception
{
    internal ConfigurationException(string message) : base(message)
    {
    }
}
