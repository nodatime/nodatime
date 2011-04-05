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
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    public class ParseData<T> : TestCaseData
    {
        public ParseData(string value, string format, T result, string what)
            : this(value, format, result, DateTimeParseStyles.None, what)
        {
        }

        public ParseData(string value, string format, T result, DateTimeParseStyles styles, string what)
            : base(value, format, result, styles)
        {
            string name = String.Format("value: [{0}], format: [{1}], {2}", value ?? "null", format ?? "null", what);
            SetName(name);
        }
    }
}