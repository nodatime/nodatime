@Title="Migrating from 1.x to 2.0"

Noda Time 2.0 contains a number of breaking changes. If you have a project which uses Noda Time
1.x and are considering upgrading to 2.0, please read the following migration guide carefully.
In particular, there are some changes which are changes to execution-time behaviour, and won't show up as compile-time errors.

Parameter names
====

Some parameters have been renamed for consistency. This should not affect code that uses positional
argument passing; code which uses named arguments will need to specify the new parameter name where
there are changes. This does not affect binary compatibility.

Obsolete members
====

A few members in 1.x were already marked as obsolete, and they have now been removed. Code using
these members will no longer compile. Two of these were simple typos in the name - fixing code
using these is simply a matter of using the correct name instead:

- `Era.AnnoMartyrm` should be `Era.AnnoMartyrum`
- `Period.FromMillseconds` should be `Period.FromMilliseconds`

In addition, `DateTimeZoneProviders.Default` has been removed. It wasn't the default in any Noda
Time code, and it's clearer to use the `DateTimeZoneProviders.Tzdb` member, which the `Default`
member was equivalent to anyway.

Support for the resource-based time zone database format was removed in Noda Time 2.0. In terms
of the public API, this just meant removing three `TzdbDateTimeZoneSource` constructors, and
removing some documented edge cases where the legacy resource format didn't include as much
information as the more recent "nzd" format. If you were previously using the resource format,
just move to the "nzd" format, using the static factory members of `TzdbDateTimeZoneSource`.

Removed, renamed or now private members
====

The `Instant(long)` constructor is now private; use `Instant.FromTicksSinceUnixEpoch` instead.
As the resolution of 2.0 is nanoseconds, a constructor taking a number of *ticks* since the
Unix epoch is confusing. The static method is self-describing, and this allows the constructor
to be rewritten for use within Noda Time itself.

The `LocalTime.LocalDateTime` property has been removed. It was rarely a good idea to
arbitrarily pick the Unix epoch as the date, and usually indicates a broken design. If you
still need this behaviour, you can easily construct a `LocalDate` for the Unix epoch and use
the addition operator instead.

`CalendarSystem.GetMaxMonth` has been renamed to `GetMonthsInYear`, to match the equivalent
method in `System.Globalization.Calendar`.

`CenturyOfEra` and `YearOfCentury` have both been removed. We considered it unlikely that they
were being used, and the subtle differences between the Gregorian and ISO calendar systems were
almost certainly not helpful. Users who wish to compute the century and year of century in a
particular form can do so reasonably easily in their own code. With this change in place, the
distinction between the ISO calendar system and Gregorian-4 is only maintained for simplicity,
compatibility and consistency; the two calendars behave identically.

`Duration.FromStandardWeeks` has been removed on the grounds that it was quite odd; it's unusual
to want a duration of a standard week - you can always just multiply by 7 and call `Duration.FromDays`
instead.

The word `Standard` has been removed from the members of `NodaConstants` and also from `Duration.FromStandardDays`
(so that's now `Duration.FromDays`). If it was annoying for the Noda Time developers, it was probably annoying
for users too... the meaning is exactly the same, and the documentation still talks about "standard" days/weeks,
but having it in the names was a bit obnoxious, particularly in code which used a lot of constants.

Factory methods in `CalendarSystem` which either didn't take any parameters (`GetPersianCalendar`) or which
no longer support those parameters (`GetCopticCalendar`, `GetJulianCalendar`) have been converted into properties.
So for example, the equivalent of `CalendarSystem.GetJulianCalendar(4)` is now just `CalendarSystem.Julian`.

Methods and properties on `Instant` to do with the Unix epoch have been renamed to be consistent with
methods introduced in `DateTimeOffset` in .NET 4.6:

- `FromSecondsSinceUnixEpoch()` is now `FromUnixTimeSeconds()`
- `FromMillisecondsSinceUnixEpoch()` is now `FromUnixTimeMilliseconds()`
- `FromTicksSinceUnixEpoch()` is now `FromUnixTimeTicks()`
- The `Ticks` property is now `ToUnixTimeTicks()`
- There are two new methods, `ToUnixTimeSeconds()` and `ToUnixTimeMilliseconds()`

Static properties on the pattern classes have been renamed to remove the `Pattern` suffix. For example,
`LocalDateTimePattern.ExtendedIsoPattern` is now just `LocalDateTimePattern.ExtendedIso`.

The `IsoDayOfWeek` properties in `LocalDate`, `LocalDateTime`, `OffsetDateTime`
and `ZonedDateTime` are now just called `DayOfWeek`. The previous numeric `DayOfWeek` properties
have been removed, but in all cases if you were actually calling them, you can just cast the `IsoDayOfWeek`
to `int` and always get the same result, as all calendar systems use ISO days of the week.

The `LocalTime` constructors accepting tick values (tick-of-second and tick-of-millisecond)
have been converted to static factory methods (`FromHourMinuteSecondTick` and `FromHourMinuteSecondMillisecondTick`).

The `LocalDateTime` constructors accepting tick values have been removed. To construct a `LocalDateTime`
value with a value more fine-grained than milliseconds, either construct separate `LocalDate` and `LocalTime`
values and add them together, or construct a `LocalDateTime` to the nearest second (or millisecond) and use
`PlusTicks` or `PlusNanoseconds` to construct the final one.

Properties related to week-years (e.g. `WeekOfWeekYear`) have been removed, in favour of a more
flexible system. See the [week-years guide](weekyears) for more information. In most cases,
the fix is to use `WeekYearRules.Iso.GetWeekOfWeekYear(date)` etc..

The `IClock.Now` property has been replaced by `IClock.GetCurrentInstant()`. This is both
clearer in meaning, and a more appropriate member type. The two calls are absolutely equivalent
in meaning.

`IDateTimeZoneSource.MapTimeZoneId` has been removed, and `IDateTimeZoneSource.GetSystemDefaultId`
has been introduced instead. `MapTimeZoneId` was only particularly useful when mapping a Windows
ID to a TZDB ID; to do that, you should now look up the Windows ID in
`TzdbDateTimeZoneSource.Default.WindowsMapping.PrimaryMapping`.

Period
====

The `Years`, `Months`, `Weeks` and `Days` properties of `Period` (and `PeriodBuilder`) are
now `int` rather than `long` properties. Any property for those units outside the range of `int`
could never be added to a date anyway, as it would immediately go out of range. This change just
makes that clearer, and embraces the new "`int` for dates, `long` for times" approach which
applies throughout Noda Time 2.0. The indexer for `PeriodBuilder` is still of type `long`, but
it will throw an `ArgumentOutOfRangeException` for values outside the range of `int` when
setting date-based units.

Normalization of a period which has time units which add up to a "days" range outside the range
of `int` will similarly fail.

Offset
====

In Noda Time 1.x, `Offset` was implemented as a number-of-milliseconds.
Sub-second time zone offsets aren't used in practice (modulo a _very_ small
number of historical cases), and in any case aren't supported by the TZDB or
BCL source data that we are able to use, so we supported more precision than
was useful.

In Noda Time 2.0, `Offset` is implemented as a number-of-seconds. This should
be mostly transparent, though `Offset.FromMilliseconds()` will now effectively
truncate to the whole number of seconds.  (Similarly, `Offset.FromTicks()` will
now truncate to the whole number of seconds rather than a whole number of
milliseconds.) The range of `Offset` is also reduced from +/- 23:59:59.999 to
+/- 18:00:00. The range reduction should have no practical consequence for real
situations, but test code which tried to use offsets between 18 and 24 hours
ahead of or behind UTC will need to be adjusted.

As a consequence of this change, offset formatting and parsing patterns no
longer support the `f` or `F` custom patterns, nor the `f` (full) standard
pattern.  Attempting to use these will generate an error, and attempting to
parse `Offset` (or `OffsetDateTime`) values containing fractional second
offsets will fail (though as mentioned above, these values do not tend to exist
in practice).

Calendars
====

The Coptic and Julian calendars no longer support variants based on "minimum number of days in the first week of the week
year" - it was felt this was really only important for the Gregorian calendar. This affects the calls used to fetch
Coptic/Julian calendars, the ID in formatted text, and also serialized values.

As noted above (when talking about removed members), the ISO calendar is now equivalent to the Gregorian 4 calendar.

Serialization
====

Binary serialization: there is no compatibility between 1.x and 2.0. If binary data has been persisted,
it will need to be converted to the 2.0 format. We don't currently provide any tools to simplify this,
but if it turns out to be a significant problem, please post on the mailing list and we'll see what we can
do.

Default values
====

The default values of some structs have changed, from returning the Unix epoch to returning January 1st 1 CE (at midnight where applicable):

- `LocalDate`
- `LocalDateTime`
- `ZonedDateTime` (in UTC, as in 1.x)
- `OffsetDateTime` (with an offset of 0, as in 1.x)

We recommend that you avoid relying on the default values at all - partly for the sake of clarity.

Text handling
====

Year specifiers
---

In version 1.x, the `y` format specifier meant "absolute year" (which may be negative) and the `Y` format
specifier meant "year of era". Unfortunately, this is not compatible with the BCL, where `y` really means year
of era. Under most calendars this is irrelevant in the BCL, as most only support a single era - but in Noda Time,
the default calendar system (Gregorian) supports dates before 1 CE. (Even so, most users will never create or use
dates before 1 CE.)

In version 2.0, `y` means "year of era", and `u` means "absolute year". This use of `u` is in line with
[Unicode TR-35](http://unicode.org/reports/tr35/tr35-dates.html#Date_Format_Patterns) although
there are other aspects of format specifiers which aren't - 2.0 is not embracing TR-35 universally, preferring BCL-compatibility,
but the BCL doesn't have any "absolute year" specifier.

For most users this change will be invisible, as it is anticipated that most users only use values where
absolute year and year of era are the same value. However, anyone using `Y` in their format patterns will
see an exception thrown, and anyone using `y` but formatting a value where the year is *not* the same as
the year of era will see different results.

One unfortunate side-effect of this is that the normal BCL handling - using the patterns specified by the BCL -
gives ambiguous values. For example, `new LocalDate(-50, 1, 1).ToString()` will return something like "01 January 0051"
instead of the previous "01 January -0050". Users whose applications are likely to encounter
dates before 1 CE should consider using custom format patterns with the `u` specifier instead of `y`.

Noda Time's ISO-8601 pattern handling will provide the same text values as before, as the patterns have been
updated to use `u`. This includes the patterns used in `NodaTime.Serialization.JsonNet`.

In summary:

<table>
  <thead>
    <tr>
      <td>Value</td>
      <td>Noda Time 1.x</td>
	  <td>Noda Time 2+</td>
      <td>BCL</td>
    </tr>
  </thead>
  <tbody>
    <tr>
	  <td>Year of era</td>
	  <td><code>Y</code></td>
	  <td><code>y</code></td>
	  <td><code>y</code></td>
    </tr>
    <tr>
	  <td>Absolute year</td>
	  <td><code>y</code></td>
	  <td><code>u</code></td>
	  <td>n/a</td>
    </tr>
  </tbody>
</table>

Formatting changes
---

- Text formatting and parsing always uses `+` and `-` now for the positive and negative signs,
  instead of asking the `NumberFormatInfo` from the culture. (These are the characters used by
  all standard cultures, so this will only change behavior when using a custom culture.)

Other changes
---

The numeric standard patterns for `Instant` and `Offset` have been removed, with no direct equivalent.
These were not known to be useful, felt "alien" in various ways, and cause issues within the
implementation. If you need these features - possibly in a specialized way - please contact the
mailing list and we may be able to suggest alternative implementations to meet your specific
requirements.

Patterns no longer allow ASCII letters (a-z, A-Z) to act as literals when they're not escaped or quoted.
Quoting make the intention more explicit, and avoids unintended use of a literal when a specifier was
expected (e.g. a date pattern of "yyyy-mm-dd"). One exception here is 'T', which is allowed for date/time
patterns only - so a date/time pattern of "yyyy-MM-ddTHH:mm:ss" is still acceptable for ISO-8601, for example.
If this change breaks your code, simply escape or quote the literals within the pattern.

When specifying a BCL `IFormatProvider`, only `CultureInfo` and `DateTimeFormatInfo` values can be used;
any other non-null reference will now throw an exception. When a `DateTimeFormatInfo` is provided,
the invariant culture is used for resource lookups and text comparisons. The previous behavior was to
use the current culture for anything other than a `CultureInfo` value. To obtain the equivalent behavior, simply
pass `provider as CultureInfo` to end up with a null argument for non-CultureInfo values, which will still
use the current culture.

Lenient resolver changes
===
In 2.0, the `LenientResolver`, which is used by `DateTimeZone.AtLeniently` and `LocalDateTime.InZoneLeniently`,
was changed to more closely match real-world usage.

- For ambiguous values, the lenient resolver used to return the later of the two possible instants.
  It now returns the *earlier* of the two possible instants.  For example, if 01:00 is ambiguous, it used to return
  1:00 standard time and it now will return 01:00 *daylight* time.

- For skipped values, the lenient resolver used to return the instant corresponding to the first possible local time
  following the "gap".  It now returns the instant that would have occurred if the gap had not existed.  This
  corresponds to a local time that is shifted forward by the duration of the gap.  For example, if values from
  02:00 to 02:59 were skipped, a value of 02:30 used to return 03:00 and it will now return 03:30.

If you require the behavior of the 1.x implementation, you can create a custom resolver that combines the `ReturnLater`
and `ReturnStartOfIntervalAfter` resolvers.  For example:

```csharp
var resolver = Resolvers.CreateMappingResolver(
    Resolvers.ReturnLater, Resolvers.ReturnStartOfIntervalAfter);
```

You can use this resolver as an argument to `LocalDateTime.InZone` instead of calling `LocalDateTime.InZoneLeniently`,
or to `DateTimeZone.ResolveLocal` instead of calling `DateTimeZone.AtLeniently`

We would strongly encourage you to carefully evaluate whether you truly need the old behavior or not before making these
compatibility changes, as we have found that the new behavior aligns more closely with most real-world scenarios.
