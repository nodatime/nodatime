using Microsoft.AspNetCore.Mvc.Filters;

namespace NodaTime.Helpers
{
    // As per https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters,
    // with minor modifications.
    public class AddHeaderAttribute : ResultFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;

        public AddHeaderAttribute(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_name, _value);
            base.OnResultExecuting(context);
        }
    }
}