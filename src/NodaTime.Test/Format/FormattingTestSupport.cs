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
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    public class FormattingTestSupport
    {
        public const string Nbsp = "\u00a0";

        public const DateTimeParseStyles LeadingSpace = DateTimeParseStyles.AllowLeadingWhite;
        public const DateTimeParseStyles TrailingSpace = DateTimeParseStyles.AllowTrailingWhite;
        public const DateTimeParseStyles InnerSpace = DateTimeParseStyles.AllowInnerWhite;
        public const DateTimeParseStyles SurroundingSpace = DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite;
        public const DateTimeParseStyles AllSpace = DateTimeParseStyles.AllowWhiteSpaces;

        public static readonly CultureInfo EnUs = new CultureInfo("en-US");
        public static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        public static readonly CultureInfo ItIt = new CultureInfo("it-IT");

        public delegate TResult OutFunc<TInput, T, out TResult>(TInput format, out T obj);

        /// <summary>
        ///   Runs the format test.
        /// </summary>
        /// <param name = "data">The data.</param>
        /// <param name = "test">The test.</param>
        public static void RunFormatTest<T>(AbstractFormattingData<T> data, Func<string> test)
        {
            Func<string> doit = () =>
                                {
                                    using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                                    {
                                        return test();
                                    }
                                };
            switch (data.Kind)
            {
                case ParseFailureKind.None:
                    Assert.AreEqual(data.S, doit());
                    break;
                case ParseFailureKind.ArgumentNull:
                    Assert.Throws<ArgumentNullException>(() => doit());
                    break;
                default:
                    Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), () => doit());
                    break;
            }
        }

        /// <summary>
        ///   Tests the parse exact_single.
        /// </summary>
        /// <param name = "data">The data.</param>
        /// <param name = "test"></param>
        public static void RunParseMultipleTest<T>(AbstractFormattingData<T> data, Func<string[], T> test)
        {
            string[] formats = null;
            if (data.F != null)
            {
                formats = data.F.Split('\0');
            }
            DoRunParseTest(data, test, formats);
        }

        public static void RunParseSingleTest<T>(AbstractFormattingData<T> data, Func<string, T> test)
        {
            if (data.F != null)
            {
                if (data.F.Split('\0').Length != 1)
                {
                    return;
                }
            }
            DoRunParseTest(data, test, data.F);
        }

        private static void DoRunParseTest<TInput, T>(AbstractFormattingData<T> data, Func<TInput, T> test, TInput format)
        {
            Func<TInput, T> doit = s =>
            {
                using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                {
                    return test(s);
                }
            };
            switch (data.Kind)
            {
                case ParseFailureKind.None:
                    Assert.AreEqual(data.PV, doit(format));
                    break;
                case ParseFailureKind.ArgumentNull:
                    Assert.Throws<ArgumentNullException>(() => doit(format));
                    break;
                default:
                    ParseFailureKind kind = data.MultiKind == ParseFailureKind.None ? data.Kind : data.MultiKind;
                    Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(kind), () => doit(format));
                    break;
            }
        }

        public static void RunTryParseSingleTest<T>(AbstractFormattingData<T> data, OutFunc<string, T, bool> test)
        {
            if (data.F != null)
            {
                if (data.F.Split('\0').Length != 1)
                {
                    return;
                }
            }
            DoRunTryParseTest(data, test, data.F);
        }

        public static void RunTryParseMultipleTest<T>(AbstractFormattingData<T> data, OutFunc<string[], T, bool> test)
        {
            string[] formats = null;
            if (data.F != null)
            {
                formats = data.F.Split('\0');
            }
            DoRunTryParseTest(data, test, formats);
        }

        private static void DoRunTryParseTest<TInput, T>(AbstractFormattingData<T> data, OutFunc<TInput, T, bool> test, TInput format)
        {
            OutFunc<TInput, T, bool> doit = (TInput s, out T value) =>
            {
                using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                {
                    return test(s, out value);
                }
            };
            bool isSuccess = data.Kind == ParseFailureKind.None;
            T result;
            Assert.IsTrue(isSuccess == doit(format, out result));
            if (isSuccess)
            {
                Assert.AreEqual(data.V, result);
            }
        }
    }
}