// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DayOfWeek"/>.
    /// </summary>
    public static class DayOfWeekExtensions
    {
        /// <summary>
        /// Converts a <see cref="DayOfWeek"/> into the corresponding <see cref="IsoDayOfWeek"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="BclConversions.ToIsoDayOfWeek"/>.</remarks>        
        /// <param name="dayOfWeek">The <c>DayOfWeek</c> to convert.</param>
        /// <returns>The <c>IsoDayOfWeek</c> equivalent to <paramref name="dayOfWeek"/></returns>
        public static IsoDayOfWeek ToIsoDayOfWeek(this DayOfWeek dayOfWeek) => BclConversions.ToIsoDayOfWeek(dayOfWeek);
    }
}
