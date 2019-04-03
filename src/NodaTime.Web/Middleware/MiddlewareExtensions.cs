// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.AspNetCore.Builder;

namespace NodaTime.Web.Middleware
{
    /// <summary>
    /// Extension methods to configure the middleware in this namespace.
    /// </summary>
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSingleLineResponseLogging(this IApplicationBuilder builder) =>
            builder.UseMiddleware<SingleLineResponseLoggingMiddleware>();
    }
}
