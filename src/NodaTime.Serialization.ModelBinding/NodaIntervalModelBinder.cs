// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaIntervalModelBinder : IModelBinder
    {
        private readonly InstantPattern _pattern;

        public NodaIntervalModelBinder()
        {
            _pattern = InstantPattern.ExtendedIsoPattern;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var modelType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;
            if (modelType != typeof(Interval))
            {
                return false;
            }

            var startInstant = default(Instant);
            var endInstant = default(Instant);
            var gotStartInstant = false;
            var gotEndInstant = false;

            var startValue = bindingContext.ValueProvider.GetValue("Start");
            if (startValue != null)
            {
                gotStartInstant = true;
                var rawStartValue = startValue.RawValue.ToString();
                var startParseResult = _pattern.Parse(rawStartValue);

                if (startParseResult.Success)
                {
                    startInstant = startParseResult.Value;
                }
            }

            var endValue = bindingContext.ValueProvider.GetValue("End");
            if (endValue != null)
            {
                gotEndInstant = true;
                var rawEndValue = endValue.RawValue.ToString();
                var endParseResult = _pattern.Parse(rawEndValue);

                if (endParseResult.Success)
                {
                    endInstant = endParseResult.Value;
                }
            }

            if (!gotStartInstant && !gotEndInstant)
            {
                return false;
            }

            if (!gotStartInstant || !gotEndInstant)
            {
                throw new InvalidNodaDataException("An Interval must contain Start and End properties.");
            }

            bindingContext.Model = new Interval(startInstant, endInstant);
            return true;
        }
    }
}