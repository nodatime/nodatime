// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text
{
    /// <summary>
    /// Centralized location for error messages around text handling.
    /// </summary>
    internal static class TextErrorMessages
    {
        internal const string AmbiguousLocalTime = "The local date/time is ambiguous in the target time zone.";
        internal const string CalendarAndEra = "The era specifier cannot be specified in the same pattern as the calendar specifier.";
        internal const string DateFieldAndEmbeddedDate = "Custom date specifiers cannot be specified in the same pattern as an embedded date specifier";
        internal const string DateSeparatorMismatch = "The value string does not match a date separator in the format string.";
        internal const string DayOfMonthOutOfRange = "The day {0} is out of range in month {1} of year {2}.";
        internal const string DayOfMonthOutOfRangeNoYear = "The day {0} is out of range in month {1}.";
        internal const string EmptyPeriod = "The specified period was empty.";
        internal const string EmptyZPrefixedOffsetPattern = "The Z prefix for an Offset pattern must be followed by a custom pattern.";
        internal const string EndOfString = "Input string ended unexpectedly early.";
        internal const string EraWithoutYearOfEra = "The era specifier cannot be used without the \"year of era\" specifier.";
        internal const string EscapeAtEndOfString = "The format string has an escape character (backslash '\') at the end of the string.";
        internal const string EscapedCharacterMismatch = "The value string does not match an escaped character in the format string: \"{0}\"";
        internal const string ExpectedEndOfString = "Expected end of input, but more data remains.";
        internal const string ExtraValueCharacters = "The format matches a prefix of the value string but not the entire string. Part not matching: \"{0}\".";
        internal const string FieldValueOutOfRange = "The value {0} is out of range for the field '{1}' in the {2} type.";
        internal const string FormatOnlyPattern = "This pattern is only capable of formatting, not parsing.";
        internal const string FormatStringEmpty = "The format string is empty.";
        internal const string Hour12PatternNotSupported = "The 'h' pattern flag (12 hour format) is not supported by the {0} type.";
        internal const string InconsistentDayOfWeekTextValue = "The specified day of the week does not matched the computed value.";
        internal const string InconsistentMonthTextValue = "The month values specified as text and numbers are inconsistent.";
        internal const string InconsistentValues2 = "The individual values for the fields '{0}' and '{1}' created an inconsistency in the {2} type.";
        internal const string InvalidEmbeddedPatternType = "The type of embedded pattern is not supported for this type.";
        internal const string InvalidHour24 = "24 is only valid as an hour number when the units smaller than hours are all 0.";
        internal const string InvalidOffset = "The specified offset is invalid for the given date/time.";
        internal const string InvalidRepeatCount = "The number of consecutive copies of the pattern character \"{0}\" in the format string ({1}) is invalid.";
        internal const string InvalidUnitSpecifier = "The period unit specifier '{0}' is invalid.";
        internal const string IsoMonthOutOfRange = "The month {0} is out of range in the ISO calendar.";
        internal const string MismatchedCharacter = "The value string does not match a simple character in the format string \"{0}\".";
        internal const string MismatchedNumber = "The value string does not match the required number from the format string \"{0}\".";
        internal const string MismatchedText = "The value string does not match the text-based field '{0}'.";
        internal const string MisplacedUnitSpecifier = "The period unit specifier '{0}' appears at the wrong place in the input string.";
        internal const string MissingAmPmDesignator = "The value string does not match the AM or PM designator for the culture at the required place.";
        internal const string MissingEmbeddedPatternEnd = "The pattern has an embedded pattern which is missing its closing character ('{0}').";
        internal const string MissingEmbeddedPatternStart = "The pattern has an embedded pattern which is missing its opening character ('{0}').";
        internal const string MissingEndQuote = "The format string is missing the end quote character \"{0}\".";
        internal const string MissingNumber = "The value string does not include a number in the expected position.";
        internal const string MissingSign = "The required value sign is missing.";
        internal const string MonthOutOfRange = "The month {0} is out of range in year {1}.";
        internal const string MultipleCapitalDurationFields = "Only one of \"D\", \"H\", \"M\" or \"S\" can occur in a duration format string.";
        internal const string NoMatchingCalendarSystem = "The specified calendar id is not recognized.";
        internal const string NoMatchingFormat = "None of the specified formats matches the given value string.";
        internal const string NoMatchingZoneId = "The specified time zone identifier is not recognized.";
        internal const string OverallValueOutOfRange = "Value is out of the legal range for the {0} type.";
        internal const string PercentAtEndOfString = "A percent sign (%) appears at the end of the format string.";
        internal const string PercentDoubled = "A percent sign (%) is followed by another percent sign in the format string.";
        internal const string PositiveSignInvalid = "A positive value sign is not valid at this point.";
        internal const string QuotedStringMismatch = "The value string does not match a quoted string in the pattern.";
        internal const string RepeatCountExceeded = "There were more consecutive copies of the pattern character \"{0}\" than the maximum allowed ({1}) in the format string.";
        internal const string RepeatedFieldInPattern = "The field \"{0}\" is specified multiple times in the pattern.";
        internal const string RepeatedUnitSpecifier = "The period unit specifier '{0}' appears multiple times in the input string.";
        internal const string SkippedLocalTime = "The local date/time is skipped in the target time zone.";
        internal const string TimeFieldAndEmbeddedTime = "Custom time specifiers cannot be specified in the same pattern as an embedded time specifier";
        internal const string TimeSeparatorMismatch = "The value string does not match a time separator in the format string.";
        internal const string UnexpectedNegative = "The value string includes a negative value where only a non-negative one is allowed.";
        internal const string UnknownStandardFormat = "The standard format \"{0}\" is not valid for the {1} type. If the pattern was intended to be a custom format, escape it with a percent sign: \"%{0}\".";
        internal const string UnparsableValue = "{0} Value being parsed: '{1}'. (^ indicates error position.)";
        internal const string UnparsableValuePostParse = "{0} Value being parsed: '{1}'.";
        internal const string UnquotedLiteral = "The character {0} is not a format specifier, and should be quoted to act as a literal.";
        internal const string ValueOutOfRange = "The value {0} is out of the legal range for the {1} type.";
        internal const string ValueStringEmpty = "The value string is empty.";
        internal const string YearOfEraOutOfRange = "The year {0} is out of range for the {1} era in the {2} calendar.";
        internal const string ZPrefixNotAtStartOfPattern = "The Z prefix for an Offset pattern must occur at the beginning of the pattern.";
    }
}
