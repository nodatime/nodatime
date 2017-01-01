// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Testing.Extensions
{
    /// <summary>
    /// Extension methods for constructing <see cref="LocalDate"/> values in the ISO calendar.
    /// </summary>
    public static class LocalDateConstruction
    {
        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in January on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.January(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of January for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in January.</returns>
        public static LocalDate January(this int day, int year) => new LocalDate(year, 1, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in February on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.February(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of February for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in February.</returns>
        public static LocalDate February(this int day, int year) => new LocalDate(year, 2, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in March on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.March(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of March for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in March.</returns>
        public static LocalDate March(this int day, int year) => new LocalDate(year, 3, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in April on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.April(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of April for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in April.</returns>
        public static LocalDate April(this int day, int year) => new LocalDate(year, 4, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in May on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.May(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of May for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in May.</returns>
        public static LocalDate May(this int day, int year) => new LocalDate(year, 5, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in June on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.June(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of June for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in June.</returns>
        public static LocalDate June(this int day, int year) => new LocalDate(year, 6, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in July on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.July(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of July for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in July.</returns>
        public static LocalDate July(this int day, int year) => new LocalDate(year, 7, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in August on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.August(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of August for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in August.</returns>
        public static LocalDate August(this int day, int year) => new LocalDate(year, 8, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in September on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.September(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of September for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in September.</returns>
        public static LocalDate September(this int day, int year) => new LocalDate(year, 9, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in October on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.October(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of October for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in October.</returns>
        public static LocalDate October(this int day, int year) => new LocalDate(year, 10, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in November on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.November(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of November for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in November.</returns>
        public static LocalDate November(this int day, int year) => new LocalDate(year, 11, day);

        /// <summary>
        /// Constructs a <see cref="LocalDate"/> in December on the given day and year.
        /// </summary>
        /// <example>
        /// <code>
        /// var date = 1.December(2017);
        /// </code>
        /// </example>
        /// <param name="day">The day of December for the new date</param>
        /// <param name="year">The year for the new date</param>
        /// <returns>The specified date in December.</returns>
        public static LocalDate December(this int day, int year) => new LocalDate(year, 12, day);
    }
}
