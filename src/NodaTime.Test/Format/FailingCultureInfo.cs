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

#region usings
using System;
using System.Globalization;
#endregion

namespace NodaTime.Test.Format
{
    /// <summary>
    /// Throws an exception if called. This forces the testing code set or pass a valid culture in all
    /// tests. The tests cannot be guaranteed to work if the culture is not set as formatting and parsing
    /// are culture dependent.
    /// </summary>
    public class FailingCultureInfo : CultureInfo
    {
        private const string CultureNotSet = "The formatting and parsing code tests should have set the correct culture.";

        public static FailingCultureInfo Instance = new FailingCultureInfo();

        public FailingCultureInfo()
            : base("en-US")
        {
        }

        public override DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                throw new NotSupportedException(CultureNotSet);
            }
            set
            {
                throw new NotSupportedException(CultureNotSet);
            }
        }

        public override NumberFormatInfo NumberFormat
        {
            get
            {
                throw new NotSupportedException(CultureNotSet);
            }
            set
            {
                throw new NotSupportedException(CultureNotSet);
            }
        }

        public override string Name
        {
            get
            {
                return "Failing";
            }
        }

        public override object GetFormat(Type formatType)
        {
            throw new NotSupportedException(CultureNotSet);
        }
    }
}
