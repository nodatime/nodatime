using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace NodaTime.Web
{
    /// <summary>
    /// Just a wrapper around Trace, really...
    /// </summary>
    public static class Log
    {
        public static void Info(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }
    }
}