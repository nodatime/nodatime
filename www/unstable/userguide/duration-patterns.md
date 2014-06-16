---
layout: userguide
title: Patterns for Duration values
category: text
weight: 2020
---

The [`Duration`](noda-type://NodaTime.Duration) type supports the following patterns:

Standard Patterns
-----------------

The following standard pattern is supported:

- `o`: Round-trip pattern, which always uses the invariant culture and a pattern string of `-D:hh:mm:ss.FFFFFFF`.
  This is the default format pattern.

Custom Patterns
---------------

The following custom pattern characters are supported for durations. See [custom pattern notes](text.html#custom-patterns)
for general notes on custom patterns, including characters used for escaping and text literals.

The pattern letters basically split into two categories:
- "Total" values, which represent as much of the duration as possible. For example, 1 day and 2 hours has a "total hours" value of 26. 
- "Partial" values, which represent part of a duration within a larger unit. For example, 1 day and 2 hours has an "hours of day" value of 2.

A pattern can only have a single "total" value, and typically will have exactly one total value, which would be the largest unit represented. You would normally want to then use each successive "partial" unit until you've got to the precision you're interested in. For example, useful patterns are:

- `-D:hh:mm:ss` - days, hours, minutes and seconds
- `-H:mm:ss.fff` - hours, minutes, seconds and milliseconds
- `M:ss` - just minutes and seconds (not terribly useful for very long durations, or negative ones)  

Bad (but legal) patterns would be:

- `hh:MM:ss` - total minutes, but only partial hours!
- `HH:ss` - total hours, partial seconds... but no partial minutes

It's possible that a future release will be detect "bad" patterns and reject them more aggressively.

Every total letter can be repeated up to 10 times, indicating the level of zero-padding applied.
Total letter values are parsed for up to 10 digits.
Partial letters for hours, minutes and seconds can be repeated once or twice
(so 'h' and 'hh' are valid, but 'hhh' is not); again, this is for zero-padding, so a value of 3 hours
and 2 minutes formatted with 'H:m' would simply be "3:2", whereas formatted with 'H:mm' it would be "3:02".
We recommend using the repeated form in most cases.

<table>
  <thead>
    <tr>
      <td>Character</td>
      <td>Meaning</td>
      <td>Example</td>
    </tr>
  </thead>
  <tbody>    
    <tr>
      <td><code>D</code> (<code>DD</code> etc)</td>
      <td>Total days</td>
      <td>54 hours with a pattern of <code>D:hh:mm</code> => 2:06:00</td>
    </tr>
    <tr>
      <td><code>H</code> (<code>HH</code> etc)</td>
      <td>Total hours</td>
      <td>54 hours with a pattern of <code>H:mm</code> => 54:00</td>
    </tr>
    <tr>
      <td><code>h</code> or <code>hh</code></td>
      <td>Hours within a day (0-23)</td>
      <td>54 hours with a pattern of <code>D:hh</code> => 2:06</td>
    </tr>
    <tr>
      <td><code>M</code> (<code>MM</code> etc)</td>
      <td>Total minutes</td>
      <td>3 hours and 10 minutes with a pattern of <code>M:ss</code> => 190:00</td>
    </tr>
    <tr>
      <td><code>m</code> or <code>mm</code></td>
      <td>Minutes within an hour (0-59)</td>
      <td>3 hours and 10 minutes with a pattern of <code>H:mm:ss</code> => 3:10:00</td>
    </tr>
    <tr>
      <td><code>S</code> (<code>SS</code> etc)</td>
      <td>Total seconds</td>
      <td>2 minutes and 10 seconds with a pattern of <code>S.fff</code> => 70.000</td>
    </tr>
    <tr>
      <td><code>s</code> or <code>ss</code></td>
      <td>Seconds within a minute (0-59)</td>
      <td>2 minutes and 10 seconds with a pattern of <code>M:ss.fff</code> => 2:10.000</td>
    </tr>
    <tr>
      <td><code>f</code> ... <code>fffffff</code>
      <td>
        The fractional second part of the offset, using exactly the specified number of characters.
		Trailing digits are truncated towards zero.
      </td>
      <td>
        1 second, 1234500 ticks: <code>s.fffffff</code> => <code>1.1234500</code> <br />
        Exactly 1 second: <code>s.f</code> => <code>1.0</code> <br />
      </td>
    </tr>
    <tr>
      <td><code>F</code> ... <code>FFFFFFF</code></td>
      <td>
        The fractional second part of the offset, using at most the specified number of characters (up to 7).
		Trailing digits are truncated towards zero, and trailing insignificant zeroes are truncated.
		If this comes after a decimal separator and the value is zero, the decimal separator is
		also truncated.
      </td>
      <td>
        1 second, 1234500 ticks: <code>s.FFFFFFF</code> => <code>1.12345</code> <br />
        Exactly 1 second: <code>s.F</code> => <code>1</code> <br />
      </td>
    </tr>
    <tr>
      <td><code>+</code></td>
      <td>
        The sign of the value, always specified whether positive or negative.
        The character used will depend on the format provider; <code>+</code> and <code>-</code> are used by the invariant culture.
      </td>
      <td>
        Positive value: <code>+HH:mm</code> => <code>+07:30</code> <br />
        Negative value: <code>+HH:mm</code> => <code>-07:30</code>
      </td>
    </tr>
    <tr>
      <td><code>-</code></td>
      <td>
        The sign of the value, only specified when the value is negative.
        The character used will depend on the format provider; <code>-</code> is
        used by the invariant culture.
      </td>
      <td>
        Positive value: <code>-HH:mm</code> => <code>07:30</code> <br />
        Negative value: <code>-HH:mm</code> => <code>-07:30</code>
      </td>
    </tr>
    <tr>
	  <td><code>.</code></td>
	  <td>
	    This is <em>always</em> a period ("."); not a culture-sensitive decimal separator as one might expect. This
		follows the example of other standard libraries, however odd it may appear. The only difference
		between a period and any other literal character is that when followed by a series of "F" characters,
		the period will be removed if there are no fractional seconds.
      </td>
    </tr>
    <tr>
      <td><code>:</code></td>
      <td>
        The time separator for the format provider; colon in the invariant culture.
      </td>
      <td><code>HH:mm</code> => <code>07:30</code></td>
    </tr>
  </tbody>    
</table>
