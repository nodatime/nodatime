// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.Extensions.Logging;

namespace NodaTime.Web.Configuration
{
    /// <summary>
    /// Options for Stackdriver integration.
    /// </summary>
    public class StackdriverOptions
    {
        /// <summary>
        /// Whether or not to configure Stackdriver logging. Defaults to false.
        /// </summary>
        public bool UseStackdriverLogging { get; set; }

        /// <summary>
        /// Whether or not to configure Stackdriver error reporting. Defaults to false.
        /// </summary>
        public bool UseStackdriverErrorReporting { get; set; }
        
        /// <summary>
        /// The log name is projects/{project_id}/logs/{log_id}. If this property is non-null, it's
        /// used to populate log ID. Otherwise, the container name is used if possible, and "aspnetcore" otherwise.
        /// </summary>
        public string? LogId { get; set; }

        /// <summary>
        /// Set to true to add console logging back in, usually to diagnose misconfiguration of Stackdriver.
        /// </summary>
        public bool IncludeConsoleLogging { get; set; }

        /// <summary>
        /// The service name used for error reporting. If this property is non-null, it's
        /// used to populate log ID. Otherwise, the container name is used if possible, and "aspnetcore" otherwise.
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// The service version used for error reporting. If this property is null, the environment name is used instead.
        /// </summary>
        public string? ServiceVersion { get; set; }

        /// <summary>
        /// The log level for the Stackdriver logger, or Debug by default. This default is normally fine,
        /// as ASP.NET Core will provide additional filtering.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }
}
