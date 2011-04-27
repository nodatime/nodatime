#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
namespace NodaTime.Format
{
    /// <summary>
    /// Provides an enumeration of failure type that can occur when parsing values.
    /// </summary>
    [Flags]
    public enum ParseFailureKind
    {
        TypeFormatError = 0x1000,
        None,
        ArgumentNull = 1,
        EscapeAtEndOfString = 3 | TypeFormatError,
        DoubleAssigment = 4 | TypeFormatError,
        MissingEndQuote = 5 | TypeFormatError,
        RepeatCountExceeded = 6,
        CannotParseValue = 7,
        ValueStringEmpty = 8 | TypeFormatError,
        FormatStringEmpty = 9 | TypeFormatError,
        FormatInvalid = 10 | TypeFormatError,
        FormatElementInvalid = 12 | TypeFormatError,
        EmptyFormatsArray = 13 | TypeFormatError,
        ExtraValueCharacters = 14,
        PercentDoubled = 15 | TypeFormatError,
        PercentAtEndOfString = 16 | TypeFormatError,
        QuotedStringMismatch = 17,
        EscapedCharacterMismatch = 18,
        MissingDecimalSeparator = 19,
        TimeSeparatorMismatch = 20,
        MismatchedNumber = 21,
        MismatchedSpace = 22,
        MismatchedCharacter = 23,
        UnknownStandardFormat = 24 | TypeFormatError,
        Hour12PatternNotSupported = 25 | TypeFormatError,
        NoMatchingFormat = 26,
        ValueOutOfRange = 27,
        MissingSign = 28 | TypeFormatError,
        PositiveSignInvalid = 29 | TypeFormatError,
        PrecisionNotSupported = 30 | TypeFormatError,
        StandardFormatWhitespace = 31 | TypeFormatError,
    }
}