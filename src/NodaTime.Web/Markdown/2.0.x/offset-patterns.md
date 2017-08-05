@Title="Patterns for Offset values"

The [`Offset`](noda-type://NodaTime.Offset) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `l`: Long format, displaying information down to the second.
  Typical pattern text: `+HH:mm:ss`
- `m`: Medium format, displaying information down to the minute.
  Typical pattern text: `+HH:mm`
- `s`: Short format, displaying information down to the hour.
  Typical pattern text: `+HH`
- `L`: Long format without punctuation, displaying information down to the second.
  Typical pattern text: `+HHmmss`
- `M`: Medium format without punctuation, displaying information down to the minute.
  Typical pattern text: `+HHmm`
- `S`: Short format without punctuation, displaying information down to the minute.
  Typical pattern text: `+HH` (so equivalent to `s` in many cases, but can vary by culture)
- `g`: General pattern. Formatting depends on the value passed in. If the offset has seconds, the long
  format is used; otherwise, if the offset has minutes, the medium format is used; otherwise the short format is used. When parsing, the other standard format patterns are tried one at a time. This is the default format pattern.
- `G`: As `g`, but using `Z` for an offset of 0, as if it were Z-prefixed. (See below.)
- `i`: As `g`, but using the colon-free formats (`L`, `M`, `S`)
- `I`: As `i`, but using `Z` for an offset of 0, as if it were Z-prefixed. (See below.)


Custom Patterns
---------------

The following custom pattern characters are supported for offsets. See [custom pattern notes](text#custom-patterns)
for general notes on custom patterns, including characters used for escaping and text literals.

<table>
  <thead>
    <tr>
      <td class="pattern-char">Character</td>
      <td class="pattern-description">Meaning</td>
      <td class="pattern-example">Example</td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>Z</code></td>
      <td>
        This can only occur at the start of a pattern, as a prefix to a normal pattern. When it's
		used, an offset of zero is always formatted as "Z", and "Z" will be parsed as a zero offset.
		When formatting, a non-zero offset falls back to the remainder of the pattern. When parsing,
		a non-Z value is always parsed by the remainder of the pattern, and a result of a zero offset
		is still acceptable. (So a pattern of "ZHH:mm" can still parse a value of "00:00" even though
		it would be formatted as "Z".)
      </td>
      <td>
        Zero: <code>ZHH:mm</code> => <code>Z</code> <br />
        5 hours: <code>ZHH:mm</code> => <code>05:00</code>
      </td>
    </tr>
    <tr>
      <td><code>H</code> or <code>HH</code></td>
      <td>
        Number of hours in the offset. <code>HH</code> is zero-padded; <code>H</code> is not.
      </td>
      <td>
        <code>H:mm</code> => <code>7:30</code> <br />
        <code>HH:mm</code> => <code>07:30</code>
      </td>
    </tr>
    <tr>
      <td><code>m</code> or <code>mm</code></td>
      <td>
        Number of minutes within the hour. <code>mm</code> is zero-padded; <code>m</code> is not.
      </td>
      <td>
        5 minutes: <code>m:ss</code> => <code>5:00</code> <br />
        5 minutes: <code>mm:ss</code> => <code>05:00</code>
      </td>
    </tr>
    <tr>
      <td><code>s</code> or <code>ss</code></td>
      <td>
        Number of seconds within the minute. <code>ss</code> is zero-padded; <code>s</code> is not.
      </td>
      <td>
        5 seconds: <code>s</code> => <code>5</code> <br />
        5 seconds: <code>ss</code> => <code>05</code>
      </td>
    </tr>
    <tr>
      <td><code>+</code></td>
      <td>
        The sign of the value, always specified whether positive or negative.
        <code>+</code> and <code>-</code> are always used as the symbols, regardless
        of the culture used when constructing the pattern. A positive offset is used
        when local time is ahead of UTC (e.g. Europe) and a negative offset is used
        when local time is behind UTC (e.g. America).
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
        <code>+</code> and <code>-</code> are always used as the symbols, regardless
        of the culture used when constructing the pattern.
      </td>
      <td>
        Positive value: <code>-HH:mm</code> => <code>07:30</code> <br />
        Negative value: <code>-HH:mm</code> => <code>-07:30</code>
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
