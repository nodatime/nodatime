// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;

namespace NodaTime.TimeZones
{
    // Delegates used for mapping local date/time values to ZonedDateTime.

    /// <summary>
    /// Chooses between two <see cref="ZonedDateTime"/> values that resolve to the same <see cref="LocalDateTime"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This delegate is used by <see cref="Resolvers.CreateMappingResolver"/> when handling an ambiguous local time,
    /// due to clocks moving backward in a time zone transition (usually due to an autumnal daylight saving transition).
    /// </para>
    /// <para>
    /// The returned value should be one of the two parameter values, based on the policy of the specific
    /// implementation. Alternatively, it can throw an <see cref="AmbiguousTimeException" /> to implement a policy of
    /// "reject ambiguous times."
    /// </para>
    /// <para>See the <see cref="Resolvers" /> class for predefined implementations.</para>
    /// <para>
    /// Implementations of this delegate can reasonably
    /// assume that the target local date and time really is ambiguous; the behaviour when the local date and time
    /// can be unambiguously mapped into the target time zone (or when it's skipped) is undefined.
    /// </para>
    /// </remarks>
    /// <param name="earlier">The earlier of the ambiguous matches for the original local date and time</param>
    /// <param name="later">The later of the ambiguous matches for the original local date and time</param>
    /// <exception cref="AmbiguousTimeException">The implementation rejects requests to map ambiguous times.</exception>
    /// <returns>
    /// A <see cref="ZonedDateTime"/> in the target time zone; typically, one of the two input parameters.
    /// </returns>
    public delegate ZonedDateTime AmbiguousTimeResolver(ZonedDateTime earlier, ZonedDateTime later);

    /// <summary>
    /// Resolves a <see cref="LocalDateTime"/> to a <see cref="ZonedDateTime" /> in the situation
    /// where the requested local time does not exist in the target time zone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This delegate is used by <see cref="Resolvers.CreateMappingResolver"/> when handling the situation where the
    /// requested local time does not exist, due to clocks moving forward in a time zone transition (usually due to a
    /// spring daylight saving transition).
    /// </para>
    /// <para>
    /// The returned value will necessarily represent a different local date and time to the target one, but
    /// the exact form of mapping is up to the delegate implementation. For example, it could return a value
    /// as close to the target local date and time as possible, or the time immediately after the transition.
    /// Alternatively, it can throw a <see cref="SkippedTimeException" /> to implement a policy of "reject
    /// skipped times."
    /// </para>
    /// <para>See the <see cref="Resolvers" /> class for predefined implementations.</para>
    /// <para>
    /// Implementations of this delegate can reasonably
    /// assume that the target local date and time really is skipped; the behaviour when the local date and time
    /// can be directly mapped into the target time zone is undefined.
    /// </para>
    /// </remarks>
    /// <param name="localDateTime">The local date and time to map to the given time zone</param>
    /// <param name="zone">The target time zone</param>
    /// <param name="intervalBefore">The zone interval directly before the target local date and time would have occurred</param>
    /// <param name="intervalAfter">The zone interval directly after the target local date and time would have occurred</param>
    /// <exception cref="SkippedTimeException">The implementation rejects requests to map skipped times.</exception>
    /// <returns>A <see cref="ZonedDateTime"/> in the target time zone.</returns>
    public delegate ZonedDateTime SkippedTimeResolver(LocalDateTime localDateTime, [NotNull] DateTimeZone zone,
        [NotNull] ZoneInterval intervalBefore, [NotNull] ZoneInterval intervalAfter);

    /// <summary>
    /// Resolves the result of attempting to map a local date and time to a target time zone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This delegate is consumed by <see cref="LocalDateTime.InZone"/> and <see cref="DateTimeZone.ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/>,
    /// among others. It provides the strategy for converting a <see cref="ZoneLocalMapping"/> (the result of attempting
    /// to map a local date and time to a target time zone) to a <see cref="ZonedDateTime"/>.
    /// </para>
    /// <para>See the <see cref="Resolvers" /> class for predefined implementations and a way of combining
    /// separate <see cref="SkippedTimeResolver" /> and <see cref="AmbiguousTimeResolver" /> values.</para>
    /// </remarks>
    /// <param name="mapping">The intermediate result of mapping a local time to a target time zone.</param>
    /// <exception cref="AmbiguousTimeException">The implementation rejects requests to map ambiguous times.</exception>
    /// <exception cref="SkippedTimeException">The implementation rejects requests to map skipped times.</exception>
    /// <returns>A <see cref="ZonedDateTime"/> in the target time zone.</returns>
    public delegate ZonedDateTime ZoneLocalMappingResolver([NotNull] ZoneLocalMapping mapping);
}
