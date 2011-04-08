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

        public delegate TResult OutFunc<T, TResult>(out T obj);

        /// <summary>
        ///   Runs the format test.
        /// </summary>
        /// <param name = "data">The data.</param>
        /// <param name = "test">The test.</param>
        public static void RunFormatTest<T>(AbstractFormattingData<T> data, Func<string> test)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                switch (data.Kind)
                {
                    case ParseFailureKind.None:
                        Assert.AreEqual(data.S, test());
                        break;
                    case ParseFailureKind.ArgumentNull:
                        Assert.Throws<ArgumentNullException>(() => test());
                        break;
                    default:
                        Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), () => test());
                        break;
                }
            }
        }

        /// <summary>
        ///   Tests the parse exact_single.
        /// </summary>
        /// <param name = "data">The data.</param>
        /// <param name = "test"></param>
        public static void RunParseTest<T>(AbstractFormattingData<T> data, Func<T> test)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                switch (data.Kind)
                {
                    case ParseFailureKind.None:
                        Assert.AreEqual(data.V, test());
                        break;
                    case ParseFailureKind.ArgumentNull:
                        Assert.Throws<ArgumentNullException>(() => test());
                        break;
                    default:
                        Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), () => test());
                        break;
                }
            }
        }

        public static void RunTryParse<T>(AbstractFormattingData<T> data, OutFunc<T, bool> test)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                T result;
                Assert.IsTrue(isSuccess == test(out result));
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, result);
                }
            }
        }
    }
}