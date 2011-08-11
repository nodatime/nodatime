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

namespace NodaTime.Fields
{
    /// <summary>
    /// Field which skips zero. In Joda Time this is more general (SkipDateTimeField)
    /// but it never skips any value *other* than zero, so that's hard-coded here.
    /// </summary>
    internal class SkipZeroDateTimeField : DelegatedDateTimeField
    {
        private readonly long minValue;

        internal SkipZeroDateTimeField(DateTimeField field) : base(field)
        {
            long min = base.GetMinimumValue();
            minValue = min <= 0 ? min - 1 : min;
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            int value = base.GetValue(localInstant);
            return value <= 0 ? value - 1 : value;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            long value = base.GetInt64Value(localInstant);
            return value <= 0 ? value - 1 : value;
        }

        internal override long GetMinimumValue()
        {
 	        return minValue;
        }

        // Joda Time doesn't actually override this... not sure why.
        internal override long GetMinimumValue(LocalInstant localInstant)
        {
            long value = base.GetMinimumValue(localInstant);
            return value <= 0 ? value - 1 : value;
        }

        internal override LocalInstant  SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, minValue, GetMaximumValue());
            if (value <= 0) {
                if (value == 0) {
                    throw new ArgumentException("Value cannot be 0", "value");
                }
                value++;
            }
            return base.SetValue(localInstant, value);
        }
    }
}
