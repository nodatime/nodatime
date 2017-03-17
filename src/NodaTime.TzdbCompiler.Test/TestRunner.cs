// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NUnitLite;
using System.Reflection;

namespace NodaTime.TzdbCompiler.Test
{
    // Not called Program as we're testing something called Program...
    class TestRunner
    {
        public static int Main(string[] args)
        {
            return new AutoRun(typeof(TestRunner).GetTypeInfo().Assembly).Execute(args);
        }
    }
}
