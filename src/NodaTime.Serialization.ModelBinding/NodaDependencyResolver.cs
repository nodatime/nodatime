using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver _original;
        private readonly NodaModelBinderResolver _resolver;

        public NodaDependencyResolver(IDependencyResolver original, NodaModelBinderResolver resolver)
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

        public IDependencyScope BeginScope()
        {
            return new NodaDependencyScope(_original.BeginScope(), _resolver);
        }
    }
}