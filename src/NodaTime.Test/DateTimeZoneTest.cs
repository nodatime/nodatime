using System.Linq;
using NodaTime.TimeZones;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for code in DateTimeZone and code which will be moving out
    /// of DateTimeZones into DateTimeZone over time.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneTest
    {
        [SetUp]
        public void Setup()
        {
            DateTimeZone.SetUtcOnly(false); // Side-effect of reseting the cache
        }

        [TearDown]
        public void TearDown()
        {
            DateTimeZone.SetUtcOnly(false); // Side-effect of reseting the cache
        }

        [Test(Description = "Test for issue 7 in bug tracker")]
        public void IterateOverIds()
        {
            // According to bug, this would go bang
            int count = DateTimeZone.Ids.Count();

            Assert.IsTrue(count > 1);
            int utcCount = DateTimeZone.Ids.Count(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(1, utcCount);
        }

        [Test]
        public void UtcIsNotNull()
        {
            Assert.IsNotNull(DateTimeZone.Utc);
        }

        [Test]
        public void SystemDefaultIsNotNull()
        {
            Assert.IsNotNull(DateTimeZone.SystemDefault);
        }

        [Test]
        public void CurrentDefaultsToNotNull()
        {
            Assert.IsNotNull(DateTimeZone.Current);
        }

        [Test]
        public void CurrentCanBeSet()
        {
            DateTimeZone.Current = DateTimeZone.Utc;
            Assert.IsNotNull(DateTimeZone.Current);
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZone.Current);
        }

        [Test]
        public void TestForId_nullId()
        {
            Assert.IsNull(DateTimeZone.ForId(null));
        }

        [Test]
        public void TestForId_UtcId()
        {
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZone.ForId(DateTimeZone.UtcId));
        }

        [Test]
        public void TestForId_InvalidId()
        {
            Assert.IsNull(DateTimeZone.ForId("not a known id"));
        }

        [Test]
        public void TestForId_AmericaLosAngeles()
        {
            const string americaLosAngeles = "America/Los_Angeles";
            var actual = DateTimeZone.ForId(americaLosAngeles);
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(DateTimeZone.Utc, actual);
            Assert.AreEqual(americaLosAngeles, actual.Id);
        }

        [Test]
        public void TestIds_UtcOnly()
        {
            DateTimeZone.SetUtcOnly(true); // Side-effect of reseting the cache
            var actual = DateTimeZone.Ids;
            var actualCount = actual.Count();
            Assert.IsTrue(actualCount == 1, "actualCount == 1");
            var first = actual.First();
            Assert.AreEqual(DateTimeZone.UtcId, first);
        }

        [Test]
        public void TestIds_All()
        {
            var actual = DateTimeZone.Ids;
            var actualCount = actual.Count();
            Assert.IsTrue(actualCount > 1, "actualCount > 1");
            var utc = actual.Single(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(DateTimeZone.UtcId, utc);
        }

        [Test]
        public void TestAddProvider_once()
        {
            DateTimeZone.SetUtcOnly(true); // Side-effect of reseting the cache
            var provider = new TestProvider();
            DateTimeZone.AddProvider(provider);
            ExcerciseProvider(provider);
        }

        [Test]
        public void TestAddProvider_twice()
        {
            DateTimeZone.SetUtcOnly(true); // Side-effect of reseting the cache
            var provider = new TestProvider();
            DateTimeZone.AddProvider(provider);
            DateTimeZone.AddProvider(provider);
            ExcerciseProvider(provider);
        }

        [Test]
        public void TestRemoveProvider_DefaultNotPresent()
        {
            DateTimeZone.SetUtcOnly(true); // Side-effect of reseting the cache
            Assert.IsFalse(DateTimeZone.RemoveProvider(DateTimeZone.DefaultDateTimeZoneProvider));
        }

        [Test]
        public void TestRemoveProvider_Default()
        {
            Assert.IsTrue(DateTimeZone.RemoveProvider(DateTimeZone.DefaultDateTimeZoneProvider));
            Assert.IsFalse(DateTimeZone.RemoveProvider(DateTimeZone.DefaultDateTimeZoneProvider));
        }

        private static void ExcerciseProvider(TestProvider provider)
        {
            var ids = DateTimeZone.Ids;
            var idsCount = ids.Count();
            Assert.IsTrue(idsCount == 1, "idsCount == 1");
            Assert.AreEqual("Ids\r\n", provider.ToString());
            var unknown = DateTimeZone.ForId("an unknown id");
            Assert.IsNull(unknown);
            Assert.AreEqual("Ids\r\nForId(an unknown id)\r\n", provider.ToString());
        }

        private class TestProvider : IDateTimeZoneProvider
        {
            private readonly StringBuilder builder = new StringBuilder();
            private readonly string[] list = new string[0];

            public IEnumerable<string> Ids
            {
                get
                {
                    builder.AppendLine("Ids");
                    return list;
                }
            }

            public DateTimeZone ForId(string id)
            {
                builder.AppendLine("ForId(" + id + ")");
                return null;
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }
    }
}
