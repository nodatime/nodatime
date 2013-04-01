// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Benchmarks.Framework
{
    internal sealed class CompositeResultHandler : BenchmarkResultHandler
    {
        private readonly IList<BenchmarkResultHandler> handlers;

        internal CompositeResultHandler(IEnumerable<BenchmarkResultHandler> handlers)
        {
            this.handlers = handlers.ToList();
        }

        internal override void HandleStartRun(BenchmarkOptions options)
        {
            foreach (var handler in handlers)
            {
                handler.HandleStartRun(options);
            }
        }

        internal override void HandleStartType(Type type)
        {
            foreach (var handler in handlers)
            {
                handler.HandleStartType(type);
            }

        }

        internal override void HandleResult(BenchmarkResult result)
        {
            foreach (var handler in handlers)
            {
                handler.HandleResult(result);
            }
        }

        internal override void HandleEndType()
        {
            foreach (var handler in handlers)
            {
                handler.HandleEndType();
            }
        }

        internal override void HandleWarning(string text)
        {
            foreach (var handler in handlers)
            {
                handler.HandleWarning(text);
            }            
        }

        internal override void HandleEndRun()
        {
            foreach (var handler in handlers)
            {
                handler.HandleEndRun();
            }
        }
    }
}
