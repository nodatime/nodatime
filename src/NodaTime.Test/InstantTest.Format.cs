using System.Globalization;
using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class InstantTest
    {
        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_InvalidFormat()
        {
            Assert.Throws<InvalidPatternException>(() => NodaConstants.UnixEpoch.ToString("A", null));
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_MinValue()
        {
            TestToStringBase(Instant.MinValue, Instant.BeginningOfTimeLabel);
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_MaxValue()
        {
            TestToStringBase(Instant.MaxValue, Instant.EndOfTimeLabel);
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_UnixEpoch()
        {
            TestToStringBase(NodaConstants.UnixEpoch, "1970-01-01T00:00:00Z");
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_Padding()
        {
            TestToStringBase(Instant.FromUtc(1, 1, 1, 12, 34, 56), "0001-01-01T12:34:56Z");
        }

        private static void TestToStringBase(Instant value, string gvalue)
        {
            string actual = value.ToString();
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("G", null);
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("N", null);
            Assert.AreEqual(value.Ticks.ToString("N0"), actual);
            actual = value.ToString("N", CultureInfo.InvariantCulture);
            Assert.AreEqual(value.Ticks.ToString("N0", CultureInfo.InvariantCulture), actual);
            actual = value.ToString("G", CultureInfo.InvariantCulture);
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("D", null);
            Assert.AreEqual(value.Ticks.ToString("D"), actual);

            actual = string.Format("{0}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:G}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:N}", value);
            Assert.AreEqual(value.Ticks.ToString("N0"), actual);
            actual = string.Format("{0:D}", value);
            Assert.AreEqual(value.Ticks.ToString("D"), actual);
        }
    }
}
