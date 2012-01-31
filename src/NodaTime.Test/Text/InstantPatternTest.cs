using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public partial class InstantPatternTest
    {
        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(Data data)
        {
            data.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(Data data)
        {
            data.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(Data data)
        {
            data.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(Data data)
        {
            data.TestFormat();
        }
    }
}
