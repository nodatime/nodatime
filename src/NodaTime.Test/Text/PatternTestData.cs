// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Alternative to AbstractFormattingData for tests which deal with patterns directly. This does not
    /// include properties which are irrelevant to the pattern tests but which are used by the BCL-style
    /// formatting tests (e.g. thread culture).
    /// </summary>
    public abstract class PatternTestData<T>
    {
        internal T Value { get; }

        protected abstract T DefaultTemplate { get; }

        /// <summary>
        /// Culture of the pattern.
        /// </summary>
        internal CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Standard pattern, expected to format/parse the same way as Pattern.
        /// </summary>
        internal IPattern<T>? StandardPattern { get; set; }

        /// <summary>
        /// Pattern text.
        /// </summary>
        internal string? Pattern { get; set; }

        /// <summary>
        /// String value to be parsed, and expected result of formatting.
        /// </summary>
        internal string? Text { get; set; }

        /// <summary>
        /// Template value to specify in the pattern
        /// </summary>
        internal T Template { get; set; }

        /// <summary>
        /// Extra description for the test case
        /// </summary>
        internal string? Description { get; set; }

        /// <summary>
        /// Message format to verify for exceptions.
        /// </summary>
        internal string? Message { get; set; }

        /// <summary>
        /// Message parameters to verify for exceptions.
        /// </summary>
        internal List<object> Parameters { get; } = new List<object>();

        internal PatternTestData(T value)
        {
            this.Value = value;
            Template = DefaultTemplate;
        }

        internal abstract IPattern<T> CreatePattern();

        internal void TestParse()
        {
            Assert.IsNull(Message);
            IPattern<T> pattern = CreatePattern();
            var result = pattern.Parse(Text!);
            var actualValue = result.Value;
            Assert.AreEqual(Value, actualValue);

            if (StandardPattern != null)
            {
                Assert.AreEqual(Value, StandardPattern.Parse(Text!).Value);
            }
        }

        internal void TestFormat()
        {
            Assert.IsNull(Message);
            IPattern<T> pattern = CreatePattern();
            Assert.AreEqual(Text, pattern.Format(Value));

            if (StandardPattern != null)
            {
                Assert.AreEqual(Text, StandardPattern.Format(Value));
            }
        }

        internal void TestParsePartial()
        {
            var pattern = CreatePartialPattern();
            Assert.IsNull(Message);
            var cursor = new ValueCursor($"^{Text}#");
            // Move to the ^
            cursor.MoveNext();
            // Move to the start of the text
            cursor.MoveNext();
            var result = pattern.ParsePartial(cursor);
            var actualValue = result.Value;
            Assert.AreEqual(Value, actualValue);
            Assert.AreEqual('#', cursor.Current);
        }

        internal virtual IPartialPattern<T> CreatePartialPattern()
        {
            throw new NotImplementedException();
        }

        internal void TestAppendFormat()
        {
            Assert.IsNull(Message);
            var pattern = CreatePattern();
            var builder = new StringBuilder("x");
            pattern.AppendFormat(Value, builder);
            Assert.AreEqual($"x{Text}", builder.ToString());
        }

        internal void TestInvalidPattern()
        {
            string expectedMessage = FormatMessage(Message!, Parameters.ToArray());
            try
            {
                CreatePattern();
                Assert.Fail("Expected InvalidPatternException");
            }
            catch (InvalidPatternException e)
            {
                // Expected... now let's check the message
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }

        public void TestParseFailure()
        {
            string expectedMessage = FormatMessage(Message!, Parameters.ToArray());
            IPattern<T> pattern = CreatePattern();
            var result = pattern.Parse(Text!);
            Assert.IsFalse(result.Success, "Expected parsing to fail, but it succeeded");
            try
            {
                result.GetValueOrThrow();
                Assert.Fail("Expected UnparsableValueException");
            }
            catch (UnparsableValueException e)
            {
                // Expected... now let's check the message *starts* with the right part.
                // We're not currently validating the bit that reproduces the bad value.
                Assert.IsTrue(e.Message.StartsWith(expectedMessage),
                    "Expected message to start with {0}; was actually {1}", expectedMessage, e.Message);

                Assert.AreEqual(Text, e.Value, "Expected exception to include the text value");
            }
        }

        public override string ToString()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                string valueText =
                    Value is IFormattable formattable ? formattable.ToString(ValuePatternText, CultureInfo.InvariantCulture)
                    : Value?.ToString() ?? "";
                builder.AppendFormat("Value={0};", valueText);
                builder.AppendFormat("Pattern={0};", Pattern);
                builder.AppendFormat("Text={0};", Text);
                if (Culture != CultureInfo.InvariantCulture)
                {
                    builder.AppendFormat("Culture={0};", Culture);
                }
                if (Description != null)
                {
                    builder.AppendFormat("Description={0};", Description);
                }
                if (!Template!.Equals(DefaultTemplate))
                {
                    builder.AppendFormat("Template={0};", Template);
                }
                // Trim the trailing semi-colon
                builder.Length--;
                return NamedWrapper<T>.SanitizeName(builder.ToString());
            }
            catch (Exception)
            {
                return "Formatting of test name failed";
            }
        }

        /// <summary>
        /// Returns the pattern text to use when formatting the value for the test name.
        /// Defaults to null, implying the default pattern for the type, but can be overridden to provide
        /// a more fine-grained pattern. This property is only used if the value implements IFormattable.
        /// </summary>
        protected virtual string? ValuePatternText => null;

        /// <summary>
        /// Formats a message, giving a *useful* error message on failure. It can be a pain checking exactly what
        /// the message format is when writing tests...
        /// </summary>
        private static string FormatMessage(string message, object[] parameters)
        {
            try
            {
                return string.Format(message, parameters);
            }
            catch (FormatException)
            {
                throw new FormatException($"Failed to format string '{message}' with {parameters.Length} parameters");
            }
        }
    }
}
