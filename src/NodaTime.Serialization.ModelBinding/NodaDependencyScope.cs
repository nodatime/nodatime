// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaDependencyScope : IDependencyScope
    {
        private readonly IDependencyScope _original;
        private readonly NodaModelBinderResolver _resolver;

        public NodaDependencyScope(IDependencyScope original, NodaModelBinderResolver resolver)
        {
            _original = original;
            _resolver = resolver;
        }

        public void Dispose()
        {
            _original.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _resolver.GetCachedModelBinder(serviceType) ?? _original.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var result = _resolver.GetCachedModelBinder(serviceType);
            return result != null
                ? new[] { result }
                : _original.GetServices(serviceType);
        }
    }
}