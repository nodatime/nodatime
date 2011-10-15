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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Alternative to AbstractFormattingData for tests which deal with patterns directly. This does not
    /// include properties which are irrelevant to the pattern tests but which are used by the BCL-style
    /// formatting tests (e.g. thread culture).
    /// </summary>
    public abstract class PatternTestData<T>
    {
        private readonly T value;

        internal T Value { get { return value; } }

        protected abstract T DefaultTemplate { get; }

        /// <summary>
        /// Culture of the pattern.
        /// </summary>
        internal CultureInfo Culture { get; set; }

        /// <summary>
        /// Pattern text.
        /// </summary>
        internal string Pattern { get; set; }

        /// <summary>
        /// String value to be parsed, and expected result of formatting.
        /// </summary>
        internal string Text { get; set; }

        /// <summary>
        /// Template value to specify in the pattern
        /// </summary>
        internal T Template { get; set; }

        /// <summary>
        /// Extra description for the test case
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Message format to verify for exceptions.
        /// </summary>
        internal string Message { get; set; }

        private readonly List<object> parameters = new List<object>();

        /// <summary>
        /// Message parameters to verify for exceptions.
        /// </summary>
        internal List<object> Parameters { get { return parameters; } }

        internal PatternTestData(T value)
        {
            this.value = value;
            Culture = CultureInfo.InvariantCulture;
            Template = DefaultTemplate;
        }

        internal abstract IPattern<T> CreatePattern();

        public void TestParse()
        {
            Assert.IsNull(Message);
            IPattern<T> pattern = CreatePattern();
            var result = pattern.Parse(Text);
            var actualValue = result.Value;
            Assert.AreEqual(Value, actualValue);
        }

        public void TestFormat()
        {
            Assert.IsNull(Message);
            IPattern<T> pattern = CreatePattern();
            Assert.AreEqual(Text, pattern.Format(Value));
        }

        public void TestInvalidPattern()
        {
            string expectedMessage = string.Format(Message, parameters.ToArray());
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
            string expectedMessage = string.Format(Message, parameters.ToArray());
            IPattern<T> pattern = CreatePattern();
            var result = pattern.Parse(Text);
            Assert.IsFalse(result.Success);
            try
            {
                result.GetValueOrThrow();
                Assert.Fail("Expected UnparsableValueException");
            }
            catch (UnparsableValueException e)
            {
                // Expected... now let's check the message
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }

        public override string  ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Value={0}; ", Value);
            builder.AppendFormat("Pattern={0}; ", Pattern);
            builder.AppendFormat("Text={0}; ", Text);
            if (Culture != CultureInfo.InvariantCulture)
            {
                builder.AppendFormat("Culture={0}; ", Culture);
            }
            if (Description != null)
            {
                builder.AppendFormat("Description={0}; ", Description);
            }
            if (!Template.Equals(DefaultTemplate))
            {
                builder.AppendFormat("Template={0};", Template);
            }
            return builder.ToString();
        }
    }
}
