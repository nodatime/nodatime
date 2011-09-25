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
using NUnit.Framework;
using NodaTime.Properties;
using NodaTime.Text;

#endregion

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Provides various formatting and parsing test helper values and methods.
    /// </summary>
    public abstract class FormattingTestSupport
    {
        #region Delegates
        /// <summary>
        /// Provides a delegate that takes an input value and converts if to an output
        /// type and store it in an out variable.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="T">The type of the converted value.</typeparam>
        /// <param name="pattern">The pattern text or string array.</param>
        /// <param name="obj">The converted value.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> otherwise.</returns>
        public delegate bool OutFunc<TInput, T>(TInput pattern, out T obj);
        #endregion

        /// <summary>
        /// A non-breaking space.
        /// </summary>
        public const string Nbsp = "\u00a0";

        public static readonly CultureInfo EnUs = new CultureInfo("en-US");
        public static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        public static readonly CultureInfo ItIt = new CultureInfo("it-IT");
        // Bengali: uses a time separator of "."
        public static readonly CultureInfo BnBd = new CultureInfo("bn-BD");

        /// <summary>
        /// Runs the format test. If the format string contains a '\0' character (which indicates that
        /// this is a multiple format string test) then it is ignored as formatting does not support
        /// multiple format strings. The thread culture is set (and restored) during the test.
        /// </summary>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        public static void RunFormatTest<T>(AbstractFormattingData<T> data, Func<string> test)
        {
            if (data.P != null)
            {
                if (data.P.Split('\0').Length != 1)
                {
                    return;
                }
            }
            Func<string> doit = () =>
            {
                using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                {
                    return test();
                }
            };
            if (data.Exception == null)
            {
                Assert.AreEqual(data.S, doit());
            }
            else if (data.Message != null)
            {
                var message = string.Format(data.Message, data.Parameters.ToArray());
                Assert.Throws(Is.TypeOf(data.Exception).And.Message.EqualTo(message), () => doit());
            }
            else
            {
                Assert.Throws(Is.TypeOf(data.Exception), () => doit());
            }
        }

        /// <summary>
        /// Runs a parse test that accepts a single format string.
        /// </summary>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        public static void RunParseSingleTest<T>(AbstractFormattingData<T> data, Func<string, T> test)
        {
            if (data.P != null)
            {
                if (data.P.Split('\0').Length != 1)
                {
                    return;
                }
            }
            DoRunParseTest(data, test, data.P, false);
        }

        /// <summary>
        /// Runs a parse test that accepts multiple format strings (which includes a list of one string).
        /// </summary>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        public static void RunParseMultipleTest<T>(AbstractFormattingData<T> data, Func<string[], T> test)
        {
            string[] formats = null;
            if (data.P != null)
            {
                formats = data.P.Split('\0');
            }
            Type oldException = data.Exception;
            string oldMessage = data.Message;
            // TODO: See if we can clean this up...
            // ValueStringEmpty is an failure mode which aborts immediately - there's no point in continuing with multiple patterns.
            if (data.Exception == typeof(UnparsableValueException) && data.Message != Messages.Parse_ValueStringEmpty)
            {
                data.Message = Messages.Parse_NoMatchingFormat;
            }
            try
            {
                DoRunParseTest(data, test, formats, true);
            }
            finally
            {
                data.Exception = oldException;
                data.Message = oldMessage;
            }
        }

        /// <summary>
        /// Performs the actual parse test after setting the correct environment.
        /// </summary>
        /// <typeparam name="TInput">The type of the format string.</typeparam>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        /// <param name="pattern">The pattern text or string list.</param>
        /// <param name="isMulti"><c>true</c> if this is a multiple format string method.</param>
        private static void DoRunParseTest<TInput, T>(AbstractFormattingData<T> data, Func<TInput, T> test, TInput pattern, bool isMulti)
        {
            Func<TInput, T> doit = s =>
            {
                using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                {
                    return test(s);
                }
            };
            if (data.Exception == null)
            {
                Assert.AreEqual(data.PV, doit(pattern));
            }
            else if (data.Message != null)
            {
                var message = string.Format(data.Message, data.Parameters.ToArray());
                Assert.Throws(Is.TypeOf(data.Exception).And.Message.EqualTo(message), () => doit(pattern));
            }
            else
            {
                Assert.Throws(Is.TypeOf(data.Exception), () => doit(pattern));
            }
        }

        /// <summary>
        /// Runs a try parse test that accepts a single format string.
        /// </summary>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        public static void RunTryParseSingleTest<T>(AbstractFormattingData<T> data, OutFunc<string, T> test)
        {
            if (data.P != null)
            {
                if (data.P.Split('\0').Length != 1)
                {
                    return;
                }
            }
            DoRunTryParseTest(data, test, data.P);
        }

        /// <summary>
        /// Runs a try parse test that accepts multiple format strings.
        /// </summary>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        public static void RunTryParseMultipleTest<T>(AbstractFormattingData<T> data, OutFunc<string[], T> test)
        {
            string[] formats = null;
            if (data.P != null)
            {
                formats = data.P.Split('\0');
            }
            Type oldException = data.Exception;
            string oldMessage = data.Message;
            // TODO: See if we can clean this up...
            // ValueStringEmpty is an failure mode which aborts immediately - there's no point in continuing with multiple patterns.
            if (data.Exception == typeof(UnparsableValueException) && data.Message != Messages.Parse_ValueStringEmpty)
            {
                data.Message = Messages.Parse_NoMatchingFormat;
            }
            try
            {
                DoRunTryParseTest(data, test, formats);
            }
            finally
            {
                data.Exception = oldException;
                data.Message = oldMessage;
            }
        }

        /// <summary>
        /// Performs the actual try parse test after setting the correct environment.
        /// </summary>
        /// <typeparam name="TInput">The type of the format string.</typeparam>
        /// <typeparam name="T">The type being tested.</typeparam>
        /// <param name="data">The test data.</param>
        /// <param name="test">The method under test.</param>
        /// <param name="pattern">The pattern text or string list.</param>
        private static void DoRunTryParseTest<TInput, T>(AbstractFormattingData<T> data, OutFunc<TInput, T> test, TInput pattern)
        {
            OutFunc<TInput, T> doit = (TInput s, out T value) =>
            {
                using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
                {
                    return test(s, out value);
                }
            };
            bool isSuccess = data.Exception == null;
            T result;
            Assert.IsTrue(isSuccess == doit(pattern, out result));
            Assert.AreEqual(data.PV, result);
        }
    }
}
