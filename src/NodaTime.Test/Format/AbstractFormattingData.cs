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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    /// <summary>
    ///   Provides a base class for containers of test data for testing format and parse functions.
    /// </summary>
    /// <remarks>
    ///   Each format and parse test must have a object value, a strnig value, and a format string (although these
    ///   may be empty or null to test error conditions). Format tests will take the value and format it and validate
    ///   it against the string value, parse tests will do the opposite.
    /// </remarks>
    /// <typeparam name = "T">The type being tested.</typeparam>
    public abstract class AbstractFormattingData<T> : ITestCaseData
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "AbstractFormattingData&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        protected AbstractFormattingData(T value)
        {
            V = value;
            PV = value;
            Styles = DateTimeParseStyles.None;
            Parameters = new List<object>();
            ThreadCulture = CultureInfo.InvariantCulture;
            ThreadUiCulture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        ///   Gets or sets the value being tested.
        /// </summary>
        /// <value>
        ///   The value.
        /// </value>
        public T V { get; set; }

        /// <summary>
        ///   Gets or sets the parsed value. Often this will be the same as the value but a format string
        ///   is not required to express all of a value's parts so the parsed value may be different than
        ///   the value e.g. offset "+5:6:7.890" can be formatted as "+5:06" which is parsed to "+5:6:0.000".
        ///   This will automatically be set to the same value a V in the constructor and only needs to be 
        ///   specified if different.
        /// </summary>
        /// <value>
        ///   The parsed value.
        /// </value>
        public T PV { get; set; }

        /// <summary>
        ///   Gets or sets the string value.
        /// </summary>
        /// <value>
        ///   The string value.
        /// </value>
        public string S { get; set; }

        /// <summary>
        ///   Gets or sets the format string.
        /// </summary>
        /// <value>
        ///   The format string.
        /// </value>
        public string F { get; set; }

        /// <summary>
        ///   Gets or sets the culture info to use when formatting or parsing.
        /// </summary>
        /// <value>
        ///   The <see cref = "CultureInfo" /> to use.
        /// </value>
        public CultureInfo C { get; set; }

        /// <summary>
        ///   Gets or sets the styles to use for parsing.
        /// </summary>
        /// <value>
        ///   The parsing styles.
        /// </value>
        public DateTimeParseStyles Styles { get; set; }

        /// <summary>
        ///   Gets or sets the optional test name. This name is appended to the generated test name
        ///   if some differentiation is needed or desired.
        /// </summary>
        /// <value>
        ///   The test name.
        /// </value>
        public string Name { get; set; }

        public Type Exception { get; set; }
        public string Message { get; set; }

        /// <summary>
        ///   Gets or sets the name of the argument. If the expected failure is <see cref = "ParseFailureKind.ArgumentNull" />
        ///   then this is the name of the null argument, otherwise this is ignored.
        /// </summary>
        /// <value>
        ///   The name of the null argument.
        /// </value>
        public string ArgumentName { get; set; }

        /// <summary>
        ///   Gets or sets the optional failure parameters. If these are present and Kind is set then these
        ///   are checked against the failure parameters if any if possible.
        /// </summary>
        /// <value>
        ///   The failure parameters.
        /// </value>
        public List<object> Parameters { get; set; }

        /// <summary>
        ///   Gets or sets the thread culture. If not set then the culture is set to one that always throws
        ///   exceptions as the thread culture should not be used by the formatting and parsing code
        ///   unless it is explicitly passed in.
        /// </summary>
        /// <value>
        ///   The thread culture.
        /// </value>
        public CultureInfo ThreadCulture { get; set; }

        /// <summary>
        ///   Gets or sets the thread UI culture. If not set then the culture is set to one that always throws
        ///   exceptions as the thread culture should not be used by the formatting and parsing code
        ///   unless it is explicitly passed in.
        /// </summary>
        /// <value>
        ///   The thread UI culture.
        /// </value>
        public CultureInfo ThreadUiCulture { get; set; }

        #region ITestCaseData Members
        /// <summary>
        ///   Gets the argument list to be provided to the test
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return new object[] { this };
            }
        }

        /// <summary>
        ///   Gets the name to be used for the test
        /// </summary>
        public string TestName
        {
            get
            {
                string formatted = S ?? "null";
                string format;
                if (F == null)
                {
                    format = "null";
                }
                else
                {
                    var parts = F.Split('\0');
                    if (parts.Length == 1)
                    {
                        format = "[" + F + "]";
                    }
                    else
                    {
                        format = "{ [" + string.Join("], [", parts) + "] }";
                    }
                }

                var builder = new StringBuilder();
                var culture = C ?? Thread.CurrentThread.CurrentCulture;
                builder.Append(culture.Name);
                builder.Append(", ");
                builder.Append(String.Format("value: [{0}]", ValueLabel(V)));
                if (!Equals(PV, V))
                {
                    builder.Append(String.Format(", parsed value: [{0}]", ValueLabel(PV)));
                }
                builder.Append(String.Format(", string: [{0}], format: {1}", formatted, format));

                if (Styles != DateTimeParseStyles.None)
                {
                    builder.Append(", Styles = ");
                    builder.Append(Styles);
                }
                if (Exception != null)
                {
                    builder.Append(", Exception = ");
                    builder.Append(Exception.Name);
                }
                if (Name != null)
                {
                    builder.Append(", ");
                    builder.Append(Name);
                }
                return builder.ToString();
            }
            set
            {
                if (value != null)
                {
                    Name = value;
                }
            }
        }

        /// <summary>
        ///   Gets the description of the test
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets the expected exception Type
        /// </summary>
        public Type ExpectedException { get; set; }

        /// <summary>
        ///   Gets the FullName of the expected exception
        /// </summary>
        public string ExpectedExceptionName { get; set; }

        /// <summary>
        ///   Gets the ignore reason.
        /// </summary>
        /// <value>
        ///   The ignore reason.
        /// </value>
        public string IgnoreReason { get; set; }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "T:NUnit.Framework.ITestCaseData" /> is ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ignored; otherwise, <c>false</c>.
        /// </value>
        public bool Ignored { get; set; }

        /// <summary>
        ///   Gets the expected result
        /// </summary>
        public object Result { get; set; }
        #endregion

        /// <summary>
        ///   Returns a string representation of the given value. This will usually not call the ToString()
        ///   method as that is problably being tested. The returned string is only used in test code and
        ///   labels so it doesn't have to be beautiful. Must handle <c>null</c> if the type is a reference
        ///   type. This should not throw an exception.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <returns>The string representation.</returns>
        protected abstract string ValueLabel(T value);
    }
}