Patterns for LocalTime values

The [`LocalTime`](noda-type://NodaTime.LocalTime) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `t`: Short format pattern.  
  This is the short time pattern as defined by the culture's [`DateTimeFormatInfo.ShortTimePattern`](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.shorttimepattern.aspx) 
  For example, in the invariant culture this is "HH:mm".

- `T`: Long format pattern.  
  This is the long time pattern as defined by the culture's [`DateTimeFormatInfo.LongTimePattern`](http://msdn.microsoft.com/en-us/library/system.globalization.datetimeformatinfo.longtimepattern.aspx) 
  For example, in the invariant culture this is "HH:mm:ss".

- `r`: Round-trip pattern.  
  This always uses a pattern of "HH:mm:ss.FFFFFFF", but with the culture-specific time separator.

Custom Patterns
---------------

The following custom offset pattern characters are supported for local times. See [custom pattern notes](text.html#custom-patterns)
for general notes on custom patterns, including characters used for escaping and text literals.

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
      <td><code>H</code> or <code>HH</code></td>
      <td>
        The hour of day in the 24-hour clock; a value 0-23.

        <p>Note that when parsing local date/time values, a value of `24`
        may be exceptionally permitted to allow
        <a href="localdatetime-patterns.html">specification of a following day's
        midnight</a>.
      </td>
      <td>
        7.30am: <code>H:mm</code> => <code>7:30</code> <br />
        7.30am: <code>HH:mm</code> => <code>07:30</code>
      </td>
    </tr>
    <tr>
      <td><code>h</code> or <code>hh</code></td>
      <td>
        The hour of day in the 12-hour clock; a value 1-12. When parsing, if no
		am/pm designator is specified, the parsed value is in the morning.
      </td>
      <td>
        8.30pm: <code>H:mm</code> => <code>7:30</code> <br />
        8.30pm: <code>HH:mm</code> => <code>07:30</code>
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
        5 seconds: <code>s.fff</code> => <code>5.000</code> <br />
        5 seconds: <code>ss.fff</code> => <code>05.000</code>
      </td>
    </tr>
    <tr>
      <td><code>f</code>, <code>ff</code> ... up to <code>fffffff</code></td>
      <td>
        The fractional second part of the offset, using exactly the specified number of characters
		(up to 7, for a representation accurate to a tick).
      </td>
      <td>
        1 second, 340 milliseconds: <code>s.fff</code> => <code>1.340</code> <br />
        1 second, 340 milliseconds: <code>s.ff</code> => <code>1.34</code> <br />
        1 second, 340 milliseconds: <code>s.f</code> => <code>1.3</code> <br />
      </td>
    </tr>
    <tr>
      <td><code>F</code>, <code>FF</code> ... up to <code>FFFFFFF</code></td>
      <td>
        The fractional second part of the offset, using at most the specified number of characters
		(up to 7, for a representation accurate to a tick).
		Trailing digits are truncated towards zero, and trailing insignificant zeroes are truncated.
		If this comes after a period (".") and the value is zero, the period is
		also truncated.
      </td>
      <td>
        1 second, 340 milliseconds: <code>s.FFF</code> => <code>1.34</code> <br />
        1 second, 340 milliseconds: <code>s.FF</code> => <code>1.34</code> <br />
        1 second, 340 milliseconds: <code>s.F</code> => <code>1.3</code> <br />
        Exactly 1 second: <code>s.F</code> => <code>1</code> <br />
      </td>
    </tr>
	<tr>
      <td><code>t</code> or <code>tt</code></td>
	  <td>
	    The culture-specific AM/PM designator, either in full (for <code>tt</code>) or just the first character
		(for <code>t</code>).
	  </td>
      <td>
	    13:10: <code>h:mm tt</code> => <code>1:10 PM</code>
	    13:10: <code>h:mm:sst</code> => <code>1:10:00P</code>
      </td>
	</tr>
	<tr>
	  <td><code>.</code></td>
	  <td>
	    This is *always* a period ("."); not a culture-sensitive decimal separator as one might expect. This
		follows the example of other standard libraries, however odd it may appear. The only difference
		between a period and any other literal character is that when followed by a series of "F" characters,
		the period will be removed if there are no fractional seconds.
      </td>
	  <td>
	    12 seconds, 500 milliseconds (en-US): <code>ss.FFF</code> => <code>12.5</code> <br />
	    12 seconds, 500 milliseconds (fr-FR): <code>ss.FFF</code> => <code>12.5</code>
      </td>
	</tr>
    <tr>
      <td><code>;</code></td>
      <td>
        This is always *formatted* as a period, but can *parse* either a period or a comma.
        In all other respects it behaves as the period custom specifier. The purpose of
        this specifier is to properly parse ISO-8601 times, where a comma is allowed as
        the separator for subsecond values.
      </td>
      <td>
        Pattern <code>ss;fff</code> parses <code>53,123</code> and <code>53.123</code>
        identically.
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
