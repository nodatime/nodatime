---
layout: userguide
title: Text handling
category: text
weight: 2010
---

There are two options for text handling in Noda Time. For some elements of
formatting, you can follow the "normal" approach from the .NET Base Class
Library (BCL) - in particular, most of the core Noda Time types implements
`IFormattable`. However, no parsing support is provided in this way. (It used
to be, but the whole approach is so convoluted that documenting it accurately
proved too great an overhead.)

The preferred approach is to use the "pattern" classes such as `LocalDatePattern`
and so forth. This leads to clearer, more robust code, and performs better. The formatting
support present in the BCL style is mostly present to work well with compound format strings,
where you may wish to mix several values of different types in a single formatting call.

All the types responsible for text in Noda Time are in the
[NodaTime.Text][3] namespace.

The pattern-based API
---------------------

A *pattern* is an object capable of *parsing* from text to a specific
type, and *formatting* a value to text. Parsing and formatting don't
take any other options: the pattern knows everything about how to
map between the value and text. In particular, internationalization
is handled by having the pattern hold a [`CultureInfo`][2].

Whereas using the BCL approach the format
information has to be specified on every call, using the pattern
approach the format information is fixed for any particular pattern.
Convenience methods are provided to create new pattern instances
based on existing ones but with different internationalization
information or other options.

Each core Noda type has its own pattern type such as
[`OffsetPattern`](noda-type://NodaTime.Text.OffsetPattern). All
these patterns implement the
[`IPattern<T>`](noda-type://NodaTime.Text.IPattern_1) interface,
which has simple `Format` and `Parse` methods taking just the value
and text respectively. The result of `Parse` is a
[`ParseResult<T>`](noda-type://NodaTime.Text.ParseResult_1) which
encapsulates both success and failure results.

The BCL-based API
-----------------

Most of the core Noda Time types ([`LocalDateTime`][4],
[`Instant`][5] etc) provide methods with the
following signatures:

- `ToString()`: Formats the value using the default pattern for the
current thread's format provider.
- `ToString(string, IFormatProvider)`: Formats the value with the
given pattern and format provider. The pattern text for this call is
exactly the same as when creating a pattern object with the preferred API.

Pattern text
------------

Each type has its own separate pattern text documentation. The
available patterns are as consistent as possible within reason, but
documenting each separately avoids confusion with some field
specifiers being available for some types but not others.

- [Duration patterns](duration-patterns.html)
- [Offset patterns](offset-patterns.html)
- [Instant patterns](instant-patterns.html)
- [LocalTime patterns](localtime-patterns.html)
- [LocalDate patterns](localdate-patterns.html)
- [LocalDateTime patterns](localdatetime-patterns.html)
- [OffsetDateTime patterns](offsetdatetime-patterns.html)
- [ZonedDateTime patterns](zoneddatetime-patterns.html)
- [Period patterns](period-patterns.html)

<a name="custom-patterns"></a>Custom patterns
---------------

All custom patterns support the following characters:

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
      <td><code>%</code></td>
      <td>Escape to force a single-character custom pattern to be treated as such.</td>
      <td><code>%H</code> => <code>5</code></td>
    </tr>
    <tr>
      <td><code>'</code></td>
      <td>
        Open and close a text literal, which can include
        double quotes.
      </td>
      <td><code>HH'H'mm'M'</code> => <code>07H30M</code></td>
    </tr>
    <tr>
      <td><code>"</code></td>
      <td>
        Open and close a text literal, which can include
        single quotes.
      </td>
      <td><code>HH"'"mm</code> => <code>07'30</code></td>
    </tr>
    <tr>
      <td><code>\</code></td>
      <td>
        Escapes the following character.
      </td>
      <td><code>HH\'mm</code> => <code>07'30</code></td>
    </tr>
  </tbody>
</table>

Additionally:

- Where valid, `:` always refers to the culture-specific time separator (a colon in the invariant culture)
- Where valid, `/` always refers to the culture-specific date separator (a forward slash in the invariant culture)

Any characters within a custom format which *don't* have a specific
meaning are treated as text literals (when parsing, they must be
matched exactly; when formatting they are reproduced exactly). This
is supported mostly for the sake of compatibility. We **strongly
recommend** that you quote any text literals, to avoid nasty
surprises if extra characters take on special meanings in later
versions.

### Related fields

In general, a field may only occur once in a pattern in any form. For example, a pattern of "dd MM '('MMM')' yyyy" is invalid as it specifies the month twice, even though it specifies it in different forms. This restriction *may* be relaxed in the future, but it would always be invalid to have a value with inconsistencies.

In some cases, fields may be related without being the same. The most obvious example here is day-of-week and the other date fields. When parsing, the day-of-week field is only used for validation: in itself, it doesn't provide enough information to specify a date. (The week-year/week-of-week-year/day-of-week scheme is not currently supported in text handling.) If the day-of-week is present but does not concur with the other values, parsing will fail.

In other cases, there can be multiple fields specifying the same information - such as "year-of-era" and "absolute-year". In these cases either field is actually enough to determine the information, but when parsing the field values are validated for consistency.

Template values
---------------

Many patterns allow a *template value* to be specified - for date/time values this is typically midnight on January 1st 2000. This value is used to provide values for fields which aren't specified elsewhere. For example, if you create a `LocalDateTimePattern` with a custom pattern of "dd HH:mm:ss" then that doesn't specify the year or month - those will be picked from the template value. Template values can be specified for both standard and custom patterns, although standard patterns will rarely use them.

The century in the template value is also used when a pattern specifies a two-digit year ("yy"), although such patterns are generally discouraged anyway.

Advice on choosing text patterns
--------------------------------

Often you don't have much choice about how to parse or format text: if you're interoperating with another system which provides or expects the data in a particular format, you just have to go with their decision. However, often you *do* have a choice. A few points of guidance:

- You need to decide whether this text is going to be parsed by *humans* or *computers* primarily. For humans, you probably want to use their culture - for computers, you should almost always use the invariant culture.
- Custom patterns are rarely appropriate for arbitrary cultures. They generally useful for either the invariant culture or for specific cultures that you have knowledge of. (If you're writing an app which is only used in one country, for example, you have a lot more freedom than if you'll be dealing with cultures you don't have experience of, where the standard patterns are generally a better bet.)
- If you're logging timestamps, think very carefully before you decide to log them in *any* time zone other than UTC. It's the one time zone that everyone else can work with, and you never need to worry about daylight saving time.
- When designing a custom pattern:
  - Consider sortability. A pattern such as `yyyy-MM-dd` is naturally sortable in the text form (assuming you never need years outside the range [0-9999]), whereas neither `dd-MM-yyyy` or `MM-dd-yyyy` is sortable.
  - Avoid two-digit years. Aside from anything else, the meaning of "2009-10-11" is a lot more obvious than "09-10-11".
  - Think about what precision you need to go down to.
  - Think about whether a fixed-width pattern would be useful or whether you want to save space by removing sub-second insignficant zeroes.
  - Try to use a pattern which is ISO-friendly where possible; it'll make it easier to interoperate with other systems in the future.
  - Quote all non-field values other than spaces.

  [2]: http://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.aspx
  [3]: noda-ns://NodaTime.Text
  [4]: noda-type://NodaTime.LocalDateTime
  [5]: noda-type://NodaTime.Instant
