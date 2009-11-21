#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    internal static class FieldUtils
    {
        public static void VerifyValueBounds(IDateTimeField field, long value, long lowerBound, long upperBound)
        {
            // TODO: i18n or decide whether we want our own custom type with lower and upper bounds
            if ((value < lowerBound) || (value > upperBound))
            {
                throw new ArgumentOutOfRangeException("value", value, "Value should be in range [" + lowerBound + "-" + upperBound + "]");
            }
        }
    }
}
