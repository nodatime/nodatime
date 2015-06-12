using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime.Text;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaDateTimeZoneModelBinder : IModelBinder
    {
        private readonly IDateTimeZoneProvider _provider;

        public NodaDateTimeZoneModelBinder(IDateTimeZoneProvider provider)
        {
            _provider = provider;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var modelType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;
            if (modelType != typeof(DateTimeZone))
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            var model = _provider.GetZoneOrNull(rawValue);
            if (model == null)
            {
                return false;
            }

            bindingContext.Model = model;
            return true;
        }
    }
}