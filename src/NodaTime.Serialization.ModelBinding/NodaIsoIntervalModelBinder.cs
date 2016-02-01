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
    public class NodaIsoIntervalModelBinder : IModelBinder
    {
        private readonly InstantPattern _pattern;

        public NodaIsoIntervalModelBinder()
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

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
            {
                return false;
            }

            var rawValue = value.RawValue.ToString();

            var slash = rawValue.IndexOf('/');
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing.");
            }

            var startText = rawValue.Substring(0, slash);
            var endText = rawValue.Substring(slash + 1);

            var startInstant = default(Instant);
            var endInstant = default(Instant);

            if (startText != string.Empty)
            {
                startInstant = _pattern.Parse(startText).Value;
            }

            if (endText != string.Empty)
            {
                endInstant = _pattern.Parse(endText).Value;
            }

            bindingContext.Model = new Interval(startInstant, endInstant);
            return true;
        }
    }
}