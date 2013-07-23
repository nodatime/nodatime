---
layout: userguide
title: Patterns for LocalDate values
category: text
weight: 84
---

The [`LocalDate`](noda-type://NodaTime.LocalDate) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `d`: Short format pattern.  
  This is the short date pattern as defined by the culture's [`DateTimeFormatInfo.ShortDatePattern`](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shortdatepattern.aspx) 
  For example, in the invariant culture this is "dddd, dd MMMM yyyy".

- `D`: Long format pattern.  
  This is the long date pattern as defined by the culture's [`DateTimeFormatInfo.LongDatePattern`](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longdatepattern.aspx) 
  For example, in the invariant culture this is "MM/dd/yyyy".

Custom Patterns
---------------

The following custom offset pattern characters are supported for local dates. See [custom pattern notes](text.html#custom-patterns)
for general notes on custom patterns, including characters used for escaping and text literals.

For the meanings of "absolute" years and text handling, see later details.

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
      <td><code>y</code> or <code>yy</code></td>
      <td>
        Two digit absolute year with an optional leading `-` sign; a single <code>y</code> allows up to two digits to be parsed,
		but formats only one digit where possible. When parsing, the "base century" is chosen from the template
		value; if the two-digit year is greater than 30, the corresponding year in the previous
		century is used. Note that when formatting, no checking
        is performed to ensure that the year will be parsed to
        the same value. (For example, 1725 would be formatted
        as 25 but parsed as 2025.) 
      </td>
      <td>
	    Assuming a template value of 2000 (the default):
        2012: <code>y</code> => <code>12</code> <br />
        2040: <code>y</code> => <code>40</code> - parsing "40" would give a date in 1940 <br />
        2004: <code>y</code> => <code>4</code> <br />
        2004: <code>yy</code> => <code>04</code> <br />
      </td>
    </tr>
    <tr>
      <td><code>yyy</code></td>
      <td>
        Three digit absolute year with optional leading `-`
        sign. This will parse up to five digits, but only format to as many as are
		required, with a minimum of three.
      </td>
      <td>
	    1984: parsed to 1984, formatted to "1984" <br />
		00123: parsed to year 123, formatted just to "123" <br />
      </td>
    </tr>
    <tr>
      <td><code>yyyy</code></td>
      <td>
        The absolute year as 4 or 5 digits with an optional leading `-` sign.
        If the absolute year is outside the range [-9999, 9999] the
        value will be formatted (with the excess digit), but
        the result may not be parsed back to the original value.
        If the next character in the pattern represents a literal
        non-digit, or a non-alphanumeric character, or this appears
        at the end of the pattern, then up to five digits will be
        parsed. Otherwise, only exactly 4 digits will be parsed. This is
        to avoid a pattern such as "yyyyMMdd" from becoming ambiguous or
        hard to parse, while allowing "yyyy-MM-dd" to handle 5-digit years
        in a convenient fashion. (The detection of "5 digits would be okay"
        is quite conservative; "yyyyVMMdd" wouldn't handle 5-digit years,
        but "yyyy'V'MMdd" would, even though the two patterns are otherwise
        equivalent. This algorithm may change over time.)
      </td>
    </tr>
    <tr>
      <td><code>yyyyy</code>/td>
      <td>
        The absolute year as exactly 5 digits with an optional leading `-` sign.
      </td>
      <td>
        2012: => <code>02012</code> <br />
        12345: => <code>12345</code> <br />
      </td>
    </tr>
	<tr>
	  <td><code>Y</code>, <code>YY</code>, <code>YYY</code>, <code>YYYY</code> or <code>YYYYY</code>
	  <td>
	    The year of era, zero-padded as necessary to the same number of characters as the number of 'Y' characters.
		See notes below.
      </td>
	  <td>
	    3 B.C.: <code>YYYY</code> => <code>0003</code>
	  </td>
	</tr>
	<tr>
	  <td><code>g</code> or <code>gg</code></td>
	  <td>
	    The name of the era. This is calendar and culture specific. See notes below.
	  </td>
	  <td>
	    13 B.C. (ISO calendar, en-US): <code>Y g</code> => <code>13 B.C.</code>
	  </td>
	</tr>
    <tr>
      <td><code>M</code> or <code>MM</code></td>
      <td>
        Month of year specified as a number. <code>MM</code> is zero-padded; <code>M</code> is not.
      </td>
      <td>
	    June: <code>M</code> => <code>6</code> <br />
	    June: <code>MM</code> => <code>06</code> <br />
	    December: <code>M</code> => <code>12</code> <br />
	    December: <code>MM</code> => <code>12</code> <br />
      </td>
    </tr>
    <tr>
      <td><code>MMM</code></td>
      <td>
	    Abbreviated month name, parsed case-insensitively. This is culture-sensitive.
      </td>
      <td>
	    (In an English locale.) <br />
	    June: <code>MMM</code> => <code>Jun</code> (can parse from "jun" or "JUN" etc.)<br />
	    December: <code>MM</code> => <code>Dec</code> (can parse from "dec" or "DEC" etc.)<br />
      </td>
    </tr>
    <tr>
      <td><code>MMMM</code></td>
      <td>
	    Full month name, parsed case-insensitively. This is culture-sensitive.
      </td>
      <td>
	    (In an English locale.) <br />
	    June: <code>MMM</code> => <code>Jun</code> (can parse from "june" or "JUNE" etc.)<br />
	    December: <code>MM</code> => <code>Dec</code> (can parse from "december" or "DECEMBER" etc.)<br />
      </td>
    </tr>
	<tr>
      <td><code>d</code> or <code>dd</code></td>
      <td>
        Day of month - <code>dd</code> is zero-padded; <code>d</code> is not.
      </td>
      <td>
	    1st of the month: <code>d</code> => <code>1</code> (would parse "01" as well)<br />
	    1st of the month: <code>dd</code> => <code>01</code><br />
	    21st of the month: <code>d</code> => <code>21</code><br />
	    21st of the month: <code>dd</code> => <code>21</code><br />
      </td>
	</tr>
    <tr>
      <td><code>ddd</code></td>
      <td>
	    Abbreviated day-of-week name, parsed case-insensitively. When parsing, the parsed day of week
		is validated against the computed date, but does not affect the calculations of that date.
		This value is culture-sensitive.
	  </td>
      <td>
	    February 4th 2012 (a Saturday)<br />
		en-US: <code>Sat</code>
		fr-FR: <code>sam.</code>
      </td>
    </tr>
    <tr>
      <td><code>dddd</code></td>
      <td>
	    Full day-of-week name, parsed case-insensitively. When parsing, the parsed day of week
		is validated against the computed date, but does not affect the calculations of that date.
      </td>
      <td>
	    February 4th 2012 (a Saturday)<br />
		en-US: <code>Saturday</code>
		fr-FR: <code>samedi</code>
      </td>
    </tr>
    <tr>
      <td><code>c</code></td>
      <td>
        The Noda-specific calendar system ID. This would rarely be appropriate
		for user-visible text, but allows exact round-tripping when serializing values via text.
      </td>
      <td><code>ISO</code><br />
	      <code>Coptic 3</code><br />
		  <code>Hijri Astronomical-Base16</code></td>
    </tr>
    <tr>
      <td><code>/</code></td>
      <td>
        The date separator for the format provider; slash in the invariant culture.
      </td>
      <td>en-US: <code>yyyy/MM/dd</code> => <code>2011/10/09</code><br />
          de-DE: <code>yyyy/MM/dd</code> => <code>2011.10.09</code></td>
    </tr>
  </tbody>
    
</table>

Notes
-----

**Absolute and era years**

Some calendars support multiple eras. For example, the ISO calendar supports the B.C. / B.C.E. and A.D. / C.E. eras.
A mapping is provided between "year within era" and "absolute" year - where an absolute year uniquely identifies the date,
and does not generally skip. In the ISO calendar, the absolute year 0 is deemed to be 1 B.C. and the absolute year 1 is
deemed to be 1 A.D. thus giving a simplified arithmetic system.

Negative absolute years can be both parsed and formatted - so "13 B.C." would be formatted as "-0012" using the "yyyy" format.

**Text sources**

Noda Time comes with its own limited set of era names, but month and day names are taken from the .NET framework.
Unfortunately these are not available on a per-calendar basis, so the same names are used for all calendars, based solely
on culture. It is hoped that future release of Noda Time may use information from the [Unicode CLDR](http://cldr.unicode.org/)
to provide a more comprehensive treatment.
