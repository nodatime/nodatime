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
        Format = 2,
        ParseEscapeAtEndOfString = 3 | TypeFormatError,
        ParseDoubleAssigment = 4 | TypeFormatError,
        ParseMissingEndQuote = 5 | TypeFormatError,
        ParseRepeatCountExceeded = 6,
        ParseCannotParseValue = 7,
        ParseValueStringEmpty = 8 | TypeFormatError,
        ParseFormatStringEmpty = 9 | TypeFormatError,
        ParseFormatInvalid = 10 | TypeFormatError,
        ParseFormatElementInvalid = 12 | TypeFormatError,
        ParseEmptyFormatsArray = 13 | TypeFormatError,
        ParseExtraValueCharacters = 14,
        ParsePercentDoubled = 15 | TypeFormatError,
        ParsePercentAtEndOfString = 16 | TypeFormatError,
        ParseQuotedStringMismatch = 17,
        ParseEscapedCharacterMismatch = 18,
        ParseMissingDecimalSeparator = 19,
        ParseTimeSeparatorMismatch = 20,
        ParseMismatchedNumber = 21,
        ParseMismatchedSpace = 22,
        ParseMismatchedCharacter = 23,
        ParseUnknownStandardFormat = 24 | TypeFormatError,
        Parse12HourPatternNotSupported = 25 | TypeFormatError,
        ParseNoMatchingFormat = 26,
        ParseValueOutOfRange = 27,
        ParseMissingSign = 28 | TypeFormatError,
        ParsePositiveSignInvalid = 29 | TypeFormatError,
    }
}