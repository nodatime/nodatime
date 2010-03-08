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
using System.Collections.Generic;
using System.Text;
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// Exception thrown when attempting to set a field outside its supported range.
    /// </summary>
    public class FieldValueException:Exception
    {
        private readonly DateTimeFieldType dateTimefieldType;
        private readonly DurationFieldType? durationFieldType;
        private readonly string fieldName;
        private readonly int numberValue;
        private readonly string stringValue;
        private readonly long? lowerBound;
        private readonly long? upperBound;

        public FieldValueException(DateTimeFieldType fieldType, String value)
            :base(CreateMessage(fieldType.ToString(), value))
        {
            dateTimefieldType = fieldType;
            durationFieldType = null;
            fieldName = fieldType.ToString();
            stringValue = value;
            numberValue = 0;
            lowerBound = null;
            upperBound = null;
    }   

        public FieldValueException(DateTimeFieldType fieldType, int value, long? lowerBound, long? upperBound)
            :base(CreateMessage(fieldType.ToString(), value, lowerBound, upperBound, null))
        {
            dateTimefieldType = fieldType;
            durationFieldType = null;
            fieldName = fieldType.ToString();
            numberValue = value;
            stringValue = null;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }


        private static String CreateMessage(String fieldName, String value)
        {
            StringBuilder sb = new StringBuilder().Append("Value ");

            if (value == null)
            {
                sb.Append("null");
            }
            else
            {
                sb.Append('"');
                sb.Append(value);
                sb.Append('"');
            }

            sb.Append(" for ").Append(fieldName).Append(' ').Append("is not supported");

            return sb.ToString();
        }

        private static String CreateMessage(string fieldName, int value,
                                        long? lowerBound, long? upperBound, String explain)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Value ").Append(value)
                .Append(" for ").Append(fieldName)
                .Append(' ');

            if (lowerBound == null)
            {
                if (upperBound == null)
                {
                    sb.Append("is not supported");
                }
                else
                {
                    sb.Append("must not be larger than ").Append(upperBound.Value);
                }
            }
            else if (upperBound == null)
            {
                sb.Append("must not be smaller than ").Append(lowerBound.Value);
            }
            else
            {
                sb.Append("must be in the range [")
                    .Append(lowerBound.Value)
                    .Append(',')
                    .Append(upperBound.Value)
                    .Append(']');
            }
            if (explain != null)
            {
                sb.Append(": ").Append(explain);
            }

            return sb.ToString();
        }

        public void PrependMessage(String message)
        {
            //if (this.Message == null)
            //{
            //    this.Message = message;
            //}
            //else if (this.Message != null)
            //{
            //    this.Message = this.Message + ": " + message;
            //}
        }
    }
}
