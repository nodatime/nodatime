// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime.Text;

namespace NodaTime.Web
{
    public static class NodaHtmlHelpers
    {
        private static readonly InstantPattern instantPattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss");

        public static string RenderTimestamp(this IHtmlHelper helper, Instant instant) => instantPattern.Format(instant);
        public static string RenderTimestamp(this IHtmlHelper helper, Timestamp timestamp) => RenderTimestamp(helper, timestamp.ToInstant());

        // TODO: Render microseconds etc.
        public static string RenderTime(this IHtmlHelper helper, double? nanos) => nanos == null ? "" : $"{nanos.Value:G4} ns";
    }
}
