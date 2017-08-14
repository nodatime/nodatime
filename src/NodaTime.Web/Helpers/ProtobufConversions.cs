// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;
using Google.Protobuf.WellKnownTypes;

namespace NodaTime.Web.Helpers
{
    // This will be in its own package one day...
    public static class ProtobufConversions
    {
        public static Instant ToInstant(this Timestamp timestamp) =>
            Instant.FromUnixTimeSeconds(timestamp.Seconds).PlusNanoseconds(timestamp.Nanos);
    }
}
