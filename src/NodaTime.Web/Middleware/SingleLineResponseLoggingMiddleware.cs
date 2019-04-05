// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace NodaTime.Web.Middleware
{
    /// <summary>
    /// By default, ASP.NET Core logs one line at the start of each request, and one line at the end of the request.
    /// This middleware logs a single line per request. That means we can't see when a request starts as easily,
    /// and won't see failure information, but it's otherwise easier to use.
    /// </summary>
    public sealed class SingleLineResponseLoggingMiddleware
    {
        private readonly Stopwatch stopwatch;
        private readonly ILogger<SingleLineResponseLoggingMiddleware> logger;
        private readonly RequestDelegate next;

        public SingleLineResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            stopwatch = Stopwatch.StartNew();
            this.next = next;
            logger = loggerFactory.CreateLogger<SingleLineResponseLoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            TimeSpan start = stopwatch.Elapsed;
            await next(context);
            TimeSpan end = stopwatch.Elapsed;
            TimeSpan duration = end - start;

            if (logger.IsEnabled(LogLevel.Information))
            {
                var request = context.Request;
                var response = context.Response;
                logger.LogInformation("{0} {1}{2} => {3} {4:0.000}ms ({5}: {6})",
                    request.Method, request.Path, request.QueryString,
                    (HttpStatusCode) response.StatusCode, duration.TotalMilliseconds,
                    request.Headers["X-Forwarded-For"], request.Headers["User-Agent"]);
            }
        }
    }
}
