// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET6_0_OR_GREATER
using System;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="TimeOnly"/>.
    /// </summary>
    public static class TimeOnlyExtensions
    {
        /// <summary>
        /// Converts a <see cref="TimeOnly"/> to the equivalent <see cref="LocalTime"/>.
        /// </summary>
        /// <param name="time">The time of day to convert.</param>
        /// <returns>The <see cref="LocalTime"/> equivalent.</returns>
        public static LocalTime ToLocalTime(this TimeOnly time) => LocalTime.FromTimeOnly(time);
    }
}
#endif
