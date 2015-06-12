using System;
using System.Web.Http.Controllers;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaModelBinderAttribute : System.Web.Http.ModelBinding.ModelBinderAttribute
    {
        private readonly NodaModelBinderResolver _resolver;

        public NodaModelBinderAttribute(Type binderType, NodaModelBinderResolver resolver)
            : base(binderType)
        {
            _resolver = resolver;
        }

        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            var original = parameter.Configuration.DependencyResolver;
            parameter.Configuration.DependencyResolver = new NodaDependencyResolver(original, _resolver);

            try
            {
                return base.GetBinding(parameter);
            }
            finally
            {
                parameter.Configuration.DependencyResolver = original;
            }
        }
    }
}