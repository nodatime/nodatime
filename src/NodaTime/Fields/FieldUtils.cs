#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Potentially remove this. Can move VerifyValueBounds into DateTimeField, probably.
    /// </summary>
    public static class FieldUtils
    {
        internal static void VerifyValueBounds(DateTimeField field, long value, long lowerBound, long upperBound)
        {
            // TODO: i18n or decide whether we want our own custom type with lower and upper bounds
            if ((value < lowerBound) || (value > upperBound))
            {
                throw new ArgumentOutOfRangeException("value", value, "Value should be in range [" + lowerBound + "-" + upperBound + "]");
            }
        }

        /// <summary>
        /// Verifies the input value against the specified range, for the given field type.
        /// </summary>
        /// <param name="fieldType">The field type</param>
        /// <param name="value">The value to verify</param>
        /// <param name="lowerBound">The minimum valid value</param>
        /// <param name="upperBound">The maximum valid value</param>
        internal static void VerifyValueBounds(DateTimeFieldType fieldType, long value, long lowerBound, long upperBound)
        {
            // TODO: i18n or decide whether we want our own custom type with lower and upper bounds
            if ((value < lowerBound) || (value > upperBound))
            {
                throw new ArgumentOutOfRangeException("value", value,
                                                      "Value of type " + fieldType + " should be in range [" + lowerBound + "-" + upperBound + "]");
            }
        }

        /// <summary>
        /// Verifies the input value against the valid range of the calendar field.
        /// </summary>
        /// <param name="field">The calendar field definition.</param>
        /// <param name="name">The name of the field for the error message.</param>
        /// <param name="value">The value to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the given value is not in the valid range of the given calendar field.</exception>
        internal static void VerifyFieldValue(DateTimeField field, string name, long value)
        {
            VerifyFieldValue(field, name, value, false);
        }

        /// <summary>
        /// Verifies the input value against the valid range of the calendar field.
        /// </summary>
        /// <param name="field">The calendar field definition.</param>
        /// <param name="name">The name of the field for the error message.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="allowNegated">if set to <c>true</c> all the range of value to be the negative as well.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the given value is not in the valid range of the given calendar field.</exception>
        internal static void VerifyFieldValue(DateTimeField field, string name, long value, Boolean allowNegated)
        {
            bool failed = false;
            string range = "";
            long minimum = field.GetMinimumValue();
            long maximum = field.GetMaximumValue();
            if (allowNegated)
            {
                range = "[" + minimum + ", " + maximum + "] or [" + -maximum + ", " + -minimum + "]";
            }
            else
            {
                range = "[" + minimum + ", " + maximum + "]";
            }
            if (allowNegated && value < 0)
            {
                if (value < -maximum || -minimum < value)
                {
                    failed = true;
                }
            }
            else
            {
                if (value < minimum || maximum < value)
                {
                    failed = true;
                }
            }
            if (failed)
            {
                throw new ArgumentOutOfRangeException(name, value, name + " is not in the valid range: " + range);
            }
        }

        /// <summary>
        /// Utility method used by AddWrapField implementations to ensure the new
        /// value lies within the field's legal value range.
        /// </summary>
        /// <param name="currentValue">The current value of the data, which may lie outside the wrapped value range</param>
        /// <param name="wrapValue">The value to add to current value before wrapping. This may be negative.</param>
        /// <param name="minValue">The wrap range minimum value.</param>
        /// <param name="maxValue">The wrap range maximum value. This must be greater than minValue (checked by the method).</param>
        /// <returns>The wrapped value</returns>
        /// <exception cref="ArgumentException">If minValue is greater than or equal to maxValue</exception>
        public static int GetWrappedValue(int currentValue, int wrapValue, int minValue, int maxValue)
        {
            return GetWrappedValue(currentValue + wrapValue, minValue, maxValue);
        }

        /// <summary>
        /// Utility method that ensures the given value lies within the field's
        /// legal value range.
        /// </summary>
        /// <param name="value">The value to fit into the wrapped value range</param>
        /// <param name="minValue">Whe wrap range minimum value.</param>
        /// <param name="maxValue">The wrap range maximum value. This must be greater than minValue (checked by the method).</param>
        /// <returns>The wrapped value</returns>
        /// <exception cref="ArgumentException">If minValue is greater than or equal to maxValue</exception>        
        public static int GetWrappedValue(int value, int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("MIN > MAX");
            }

            int wrapRange = maxValue - minValue + 1;
            value -= minValue;

            if (value >= 0)
            {
                return (value % wrapRange) + minValue;
            }

            int remByRange = (-value) % wrapRange;

            if (remByRange == 0)
            {
                return 0 + minValue;
            }
            return (wrapRange - remByRange) + minValue;
        }
    }
}