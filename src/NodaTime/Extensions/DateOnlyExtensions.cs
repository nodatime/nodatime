// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET6_0_OR_GREATER
using System;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateOnly"/>.
    /// </summary>
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateOnly"/> to the equivalent <see cref="LocalDate"/>.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>The <see cref="LocalDate"/> equivalent, which is always in the ISO calendar system.</returns>
        public static LocalDate ToLocalDate(this DateOnly date) => LocalDate.FromDateOnly(date);
    }
}
#endif
