// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime.Text;

namespace NodaTime.Web.Helpers
{
    public static class NodaHtmlHelpers
    {
        private static readonly InstantPattern instantPattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss");

        public static string RenderTimestamp(this IHtmlHelper helper, Instant? instant) => instant == null ? null : instantPattern.Format(instant.Value);
        public static string RenderTimestamp(this IHtmlHelper helper, Timestamp timestamp) => RenderTimestamp(helper, timestamp?.ToInstant());
        public static string RenderTimestampAsUtcDate(this IHtmlHelper helper, Timestamp timestamp) => RenderTimestamp(helper, timestamp?.ToInstant().InUtc().Date);
        public static string RenderTimestamp(this IHtmlHelper helper, LocalDate? date) => date == null ? null : LocalDatePattern.Iso.Format(date.Value);

        public static string RenderTime(this IHtmlHelper helper, double? nanos)
        {
            if (nanos == null)
            {
                return null;
            }
            if (nanos < 1000)
            {
                return $"{nanos.Value:G4} ns";
            }
            double micros = nanos.Value / 1_000d;
            if (micros < 1000)
            {
                return $"{micros:G4} \u00b5s";
            }
            double millis = nanos.Value / 1_000_000d;
            if (millis < 1000)
            {
                return $"{millis:G4} ms";
            }
            double seconds = nanos.Value / 1_000_000_000d;
            return $"{seconds:G4} s";
        }
    }
}
