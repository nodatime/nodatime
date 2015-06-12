using System;
using System.Web.Http.Controllers;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaActionValueBinder : IActionValueBinder
    {
        private readonly IActionValueBinder _original;
        public NodaModelBinderResolver Resolver { get; }

        public NodaActionValueBinder(IActionValueBinder original, IDateTimeZoneProvider provider)
        {
            _original = original;
            Resolver = new NodaModelBinderResolver(provider);
        }

        public HttpActionBinding GetBinding(HttpActionDescriptor actionDescriptor)
        {
            foreach (var parameter in actionDescriptor.GetParameters())
            {
                if (parameter.ParameterBinderAttribute != null)
                {
                    continue;
                }

                var parameterType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
                var modelBinder = Resolver.GetCachedModelBinder(parameterType);

                if (modelBinder != null)
                {
                    parameter.ParameterBinderAttribute = new NodaModelBinderAttribute(parameterType, Resolver);
                }
            }

            return _original.GetBinding(actionDescriptor);
        }
    }
}