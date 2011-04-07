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
    public abstract class AbstractFormattingData<T> : ITestCaseData,
                                                      IFormattingData
    {
        public static readonly CultureInfo Failing = new FailingCultureInfo();

        private CultureInfo culture;

        protected AbstractFormattingData()
        {
            Styles = DateTimeParseStyles.None;
            Kind = ParseFailureKind.None;
            Parameters = new List<object>();
            ThreadCulture = Failing;
            ThreadUiCulture = Failing;
        }

        public T V { get; set; }
        public string S { get; set; }
        public string F { get; set; }
        public DateTimeParseStyles Styles { get; set; }
        public string Name { get; set; }
        public ParseFailureKind Kind { get; set; }
        public string ArgumentName { get; set; }
        public List<object> Parameters { get; set; }
        public CultureInfo ThreadCulture { get; set; }
        public CultureInfo ThreadUiCulture { get; set; }

        public CultureInfo C
        {
            get
            {
                return culture ?? Thread.CurrentThread.CurrentUICulture;
            }
            set { culture = value; }
        }

        #region ITestCaseData Members
        public object[] Arguments
        {
            get
            {
                return new object[] { this };
            }
        }

        public string TestName
        {
            get
            {
                string label = V != null ? ValueLabel(V) : "null";
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
                builder.Append(C.Name);
                builder.Append(", ");
                builder.Append(String.Format("value: [{0}], formatted: [{1}], format: {2}", label, formatted, format));

                if (Styles != DateTimeParseStyles.None)
                {
                    builder.Append(", Styles = ");
                    builder.Append(Styles);
                }
                if (Kind != ParseFailureKind.None)
                {
                    builder.Append(", Kind = ");
                    builder.Append(Kind);
                    if (Kind == ParseFailureKind.ArgumentNull)
                    {
                        builder.Append(", Argument = \"");
                        builder.Append(ArgumentName);
                        builder.Append("\"");
                    }
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

        public string Description { get; set; }

        public Type ExpectedException { get; set; }

        public string ExpectedExceptionName { get; set; }

        public string IgnoreReason { get; set; }

        public bool Ignored { get; set; }

        public object Result { get; set; }
        #endregion

        internal void Validate(ParseInfo parseInfo)
        {
            Assert.AreEqual(Kind, parseInfo.Failure, "Failure kind mismatch");
            if (Kind != ParseFailureKind.None)
            {
                Assert.AreEqual(Parameters.ToArray(), parseInfo.FailureMessageParameters, "Failure message parameters mismatch");
                if (ArgumentName == null)
                {
                    Assert.IsNull(parseInfo.FailureArgumentName, "failure argument name should be null");
                }
                else
                {
                    Assert.AreEqual(ArgumentName, parseInfo.FailureArgumentName, "Failure argument name mismatch");
                }
            }
        }

        protected abstract string ValueLabel(T value);
    }
}