@Title="Patterns for AnnualDate values"

The [`AnnualDate`](noda-type://NodaTime.AnnualDate) type supports the following patterns:

Standard Patterns
-----------------

The following standard patterns are supported:

- `G`: General invariant ISO-like pattern. This corresponds to the custom pattern `'MM'-'dd`. This is the default format pattern.

Custom Patterns
---------------



The following custom format pattern characters are supported for annual dates. See [custom pattern notes](text#custom-patterns)
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
	    December: <code>MMM</code> => <code>Dec</code> (can parse from "dec" or "DEC" etc.)<br />
      </td>
    </tr>
    <tr>
      <td><code>MMMM</code></td>
      <td>
	    Full month name, parsed case-insensitively. This is culture-sensitive.
      </td>
      <td>
	    (In an English locale.) <br />
	    June: <code>MMMM</code> => <code>June</code> (can parse from "june" or "JUNE" etc.)<br />
	    December: <code>MMMM</code> => <code>December</code> (can parse from "december" or "DECEMBER" etc.)<br />
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
  </tbody>

</table>
