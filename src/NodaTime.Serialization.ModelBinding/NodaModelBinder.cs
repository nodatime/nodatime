// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime.Text;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaModelBinder<T> : IModelBinder
    {
        private readonly IPattern<T> _pattern;

        public NodaModelBinder(IPattern<T> pattern)
        {
            _pattern = pattern;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var modelType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;
            if (modelType != typeof(T))
            {
                return false;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            var result = _pattern.Parse(rawValue);
            if (result.Success)
            {
                bindingContext.Model = result.Value;
                return true;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, result.Exception);
            return false;
        }
    }
}