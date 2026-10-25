// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;

namespace NodaTime.Test;

public class DateIntervalTest
{
    private static readonly CalendarSystem JulianCalendar = CalendarSystem.Julian;

    [Test]
    public void Construction_DifferentCalendars()
    {
        LocalDate start = new LocalDate(1600, 1, 1);
        LocalDate end = new LocalDate(1800, 1, 1, JulianCalendar);
        Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
    }

    [Test]
    public void Construction_EndBeforeStart()
    {
        LocalDate start = new LocalDate(1600, 1, 1);
        LocalDate end = new LocalDate(1500, 1, 1);
        Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
    }

    [Test]
    public void Construction_EqualStartAndEnd()
    {
        LocalDate start = new LocalDate(2000, 1, 1);
        Assert.DoesNotThrow(() => new DateInterval(start, start));
    }

    [Test]
    public void Construction_Properties()
    {
        LocalDate start = new LocalDate(2000, 1, 1);
        LocalDate end = new LocalDate(2001, 6, 19);
        var interval = new DateInterval(start, end);
        Assert.AreEqual(start, interval.Start);
        Assert.AreEqual(end, interval.End);
    }

    [Test]
    public void Equals()
    {
        LocalDate start = new LocalDate(2000, 1, 1);
        LocalDate end = new LocalDate(2001, 6, 19);
        var interval = new DateInterval(start, end);
        var equalInterval = new DateInterval(start, end);
        var unequalIntervals = new[]
        {
            new DateInterval(start.PlusDays(1), end),
            new DateInterval(start, end.PlusDays(1)),
            // This is a really, really similar calendar to ISO, but we do distinguish.
            // (The calendar system has to be the same for the start and end, so we don't
            // need to perform start-different vs end-different equality tests.)
            new DateInterval(start.WithCalendar(CalendarSystem.Gregorian), end.WithCalendar(CalendarSystem.Gregorian)),
        };
        TestHelper.TestEqualsClass(interval, equalInterval, unequalIntervals);

        Assert.IsTrue(interval == equalInterval);
        Assert.IsFalse(interval != equalInterval);

        Assert.IsFalse(interval == null);
        Assert.IsTrue(interval != null);
        Assert.IsFalse(null == interval);
        Assert.IsTrue(null != interval);

        foreach (var unequalInterval in unequalIntervals)
        {
            Assert.IsFalse(interval == unequalInterval);
            Assert.IsTrue(interval != unequalInterval);
        }
    }

    [Test]
    public void StringRepresentation()
    {
        LocalDate start = new LocalDate(2000, 1, 1);
        LocalDate end = new LocalDate(2001, 6, 19);
        var interval = new DateInterval(start, end);
        Assert.AreEqual("[2000-01-01, 2001-06-19]", interval.ToString());
    }

    [Test]
    public void Length()
    {
        LocalDate start = new LocalDate(2000, 1, 1);
        LocalDate end = new LocalDate(2000, 2, 10);
        var interval = new DateInterval(start, end);
        Assert.AreEqual(41, interval.Length);
    }

    [Test]
    public void Calendar()
    {
        var calendar = CalendarSystem.Julian;
        LocalDate start = new LocalDate(2000, 1, 1, calendar);
        LocalDate end = new LocalDate(2000, 2, 10, calendar);
        var interval = new DateInterval(start, end);
        Assert.AreEqual(calendar, interval.Calendar);
    }

    [Test]
    [TestCase("1999-12-31", false, TestName = "Before start")]
    [TestCase("2000-01-01", true, TestName = "On start")]
    [TestCase("2005-06-06", true, TestName = "In middle")]
    [TestCase("2014-06-30", true, TestName = "On end")]
    [TestCase("2014-07-01", false, TestName = "After end")]
    public void Contains(string candidateText, bool expected)
    {
        var start = new LocalDate(2000, 1, 1);
        var end = new LocalDate(2014, 06, 30);
        var candidate = LocalDatePattern.Iso.Parse(candidateText).Value;
        var interval = new DateInterval(start, end);
        Assert.AreEqual(expected, interval.Contains(candidate));
    }

    [Test]
    public void Contains_DifferentCalendar()
    {
        var start = new LocalDate(2000, 1, 1);
        var end = new LocalDate(2014, 06, 30);
        var interval = new DateInterval(start, end);
        var candidate = new LocalDate(2000, 1, 1, JulianCalendar);
        Assert.Throws<ArgumentException>(() => interval.Contains(candidate));
    }

    [Test]
    public void Deconstruction()
    {
        var start = new LocalDate(2017, 11, 6);
        var end = new LocalDate(2017, 11, 10);
        var value = new DateInterval(start, end);

        var (actualStart, actualEnd) = value;

        Assert.Multiple(() =>
        {
            Assert.AreEqual(start, actualStart);
            Assert.AreEqual(end, actualEnd);
        });
    }

    [Test]
    public void Contains_NullInterval_Throws()
    {
        var start = new LocalDate(2017, 11, 6);
        var end = new LocalDate(2017, 11, 10);
        var value = new DateInterval(start, end);

        Assert.Throws<ArgumentNullException>(() => value.Contains(null!));
    }

    [Test]
    public void Contains_IntervalWithinAnotherCalendar_Throws()
    {
        var value = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
            new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

        var other = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
            new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

        Assert.Throws<ArgumentException>(() => value.Contains(other));
    }

    [TestCase("2014-03-07,2014-03-07", "2014-03-07,2014-03-07", true)]
    [TestCase("2014-03-07,2014-03-10", "2015-01-01,2015-04-01", false)]
    [TestCase("2015-01-01,2015-04-01", "2014-03-07,2014-03-10", false)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-07,2014-03-15", true)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-10,2014-03-31", true)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-10,2014-03-15", true)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-05,2014-03-09", false)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-20,2014-04-07", false)]
    [TestCase("2014-11-01,2014-11-30", "2014-01-01,2014-12-31", false)]
    public void Contains_IntervalOverload(string firstInterval, string secondInterval, bool expectedResult)
    {
        DateInterval value = ParseInterval(firstInterval);
        DateInterval other = ParseInterval(secondInterval);
        Assert.AreEqual(expectedResult, value.Contains(other));
    }

    [Test]
    public void Intersection_NullInterval_Throws()
    {
        var value = new DateInterval(new LocalDate(100), new LocalDate(200));
        Assert.Throws<ArgumentNullException>(() => value.Intersection(null!));
    }

    [Test]
    public void Intersection_IntervalInDifferentCalendar_Throws()
    {
        var value = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
            new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

        var other = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
            new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

        Assert.Throws<ArgumentException>(() => value.Intersection(other));
    }

    [TestCase("2014-03-07,2014-03-07", "2014-03-07,2014-03-07", "2014-03-07,2014-03-07")]
    [TestCase("2014-03-07,2014-03-10", "2015-01-01,2015-04-01", null)]
    [TestCase("2015-01-01,2015-04-01", "2014-03-07,2014-03-10", null)]
    [TestCase("2014-03-07,2014-03-31", "2014-03-07,2014-03-15", "2014-03-07,2014-03-15")]
    [TestCase("2014-03-07,2014-03-31", "2014-03-10,2014-03-31", "2014-03-10,2014-03-31")]
    [TestCase("2014-03-07,2014-03-31", "2014-03-10,2014-03-15", "2014-03-10,2014-03-15")]
    [TestCase("2014-03-07,2014-03-31", "2014-03-05,2014-03-09", "2014-03-07,2014-03-09")]
    [TestCase("2014-03-07,2014-03-31", "2014-03-20,2014-04-07", "2014-03-20,2014-03-31")]
    [TestCase("2014-11-01,2014-11-30", "2014-01-01,2014-12-31", "2014-11-01,2014-11-30")]
    public void Intersection(string firstInterval, string secondInterval, string expectedInterval)
    {
        var value = ParseInterval(firstInterval);
        var other = ParseInterval(secondInterval);
        var expectedResult = ParseIntervalOrNull(expectedInterval);
        Assert.AreEqual(expectedResult, value.Intersection(other));
    }

    [Test]
    public void Union_NullInterval_Throws()
    {
        var value = new DateInterval(new LocalDate(100), new LocalDate(200));
        Assert.Throws<ArgumentNullException>(() => value.Union(null!));
    }

    [Test]
    public void Union_DifferentCalendar_Throws()
    {
        var value = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
            new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

        var other = new DateInterval(
            new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
            new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

        Assert.Throws<ArgumentException>(() => value.Union(other));
    }

    [TestCase("2014-03-07,2014-03-20", "2015-03-07,2015-03-20", null, Description = "Disjointed intervals")]
    [TestCase("2014-03-07,2014-03-20", "2014-03-21,2014-03-30", "2014-03-07,2014-03-30", Description = "Abutting intervals")]
    [TestCase("2014-03-07,2014-03-20", "2014-03-07,2014-03-20", "2014-03-07,2014-03-20", Description = "Equal intervals")]
    [TestCase("2014-03-07,2014-03-20", "2014-03-15,2014-03-23", "2014-03-07,2014-03-23", Description = "Overlapping intervals")]
    [TestCase("2014-03-07,2014-03-20", "2014-03-10,2014-03-15", "2014-03-07,2014-03-20", Description = "Interval completely contained in another")]
    public void Union(string first, string second, string expected)
    {
        DateInterval firstInterval = ParseInterval(first);
        DateInterval secondInterval = ParseInterval(second);
        DateInterval? expectedResult = ParseIntervalOrNull(expected);

        Assert.Multiple(() =>
        {
            Assert.AreEqual(expectedResult, firstInterval.Union(secondInterval), "First union failed.");
            Assert.AreEqual(expectedResult, secondInterval.Union(firstInterval), "Second union failed.");
        });
    }

    [TestCase("2018-05-04,2018-05-06", "2018-05-04", "2018-05-05", "2018-05-06", Description = "Multi-day")]
    [TestCase("2018-05-04,2018-05-04", "2018-05-04", Description = "Single date")]
    [TestCase("9999-12-29,9999-12-31", "9999-12-29", "9999-12-30", "9999-12-31", Description = "Max dates")]
    public void Iteration(string intervalText, params string[] expectedDatesText)
    {
        var interval = ParseInterval(intervalText);
        var expected = expectedDatesText.Select(x => LocalDatePattern.Iso.Parse(x).Value).ToList();
        var actual = interval.ToList();
        Assert.AreEqual(expected, actual);

        // Test the non-generic version too. The double cast makes sure we use the right GetEnumerator call.
        actual = ((IEnumerable) interval).Cast<object>().Cast<LocalDate>().ToList();
        Assert.AreEqual(expected, actual);
    }

    private DateInterval? ParseIntervalOrNull(string? textualInterval) =>
        textualInterval is null ? null : ParseInterval(textualInterval);

    private DateInterval ParseInterval(string textualInterval)
    {
        var parts = textualInterval.Split(new char[] { ',' });
        var start = LocalDatePattern.Iso.Parse(parts[0]).Value;
        var end = LocalDatePattern.Iso.Parse(parts[1]).Value;

        return new DateInterval(start, end);
    }
}