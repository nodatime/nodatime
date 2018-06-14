// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Extensions;
using System;

namespace NodaTime.Web
{
    /// <summary>
    /// Just a place to make sure the recipes keep working. At some point this should be in NodaTime.Demo,
    /// with code to extract the recipes directly. Not today...
    /// </summary>
    public class RecipesScratchpad
    {
        private void Birthday()
        {
            LocalDate birthDate = new LocalDate(1976, 6, 19);
            DateTimeZone zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZonedClock clock = SystemClock.Instance.InZone(zone);
            LocalDate today = clock.GetCurrentDate();
            Console.WriteLine($"Today's date: {today:yyyy-MM-dd}");
            Period age = Period.Between(birthDate, today);
            Console.WriteLine(
                $"Jon is: {age.Years} years, {age.Months} months, {age.Days} days old.");
        }

        private void EndOfMonth()
        {
            LocalDate today = new LocalDate(2014, 6, 27);
            LocalDate endOfMonth = today.With(DateAdjusters.EndOfMonth);
            Console.WriteLine(endOfMonth); // 2014-06-30
        }

        private void StartOfMonth()
        {
            LocalDateTime now = new LocalDateTime(2014, 6, 27, 7, 14, 25);
            LocalDateTime startOfMonth = now.With(DateAdjusters.StartOfMonth);
            Console.WriteLine(startOfMonth); // 2014-06-01T07:14:25
        }

        private void TimeTruncation()
        {
            LocalDateTime now = new LocalDateTime(2014, 6, 27, 7, 14, 25, 500);
            LocalDateTime truncated = now.With(TimeAdjusters.TruncateToMinute); // 2014-06-27T07:14:00
        }
    }
}
