// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace NodaTime.Serialization.ModelBinding
{
    public static class Extensions
    {
        public static HttpConfiguration ConfigureForNodaTime(this HttpConfiguration config, IDateTimeZoneProvider provider)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("config");
            }

            var originalActionValueBinder = (IActionValueBinder)config.Services.GetService(typeof(IActionValueBinder));
            config.Services.Replace(typeof(IActionValueBinder), new NodaActionValueBinder(originalActionValueBinder, provider));

            return config;
        }

        public static HttpConfiguration WithIsoIntervalConverter(this HttpConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            var actionValueBinder = config.Services.GetService(typeof(IActionValueBinder)) as NodaActionValueBinder;
            if (actionValueBinder != null)
            {
                actionValueBinder.Resolver.UseIsoIntervalModelBinder = true;
            }

            return config;
        }
    }
}