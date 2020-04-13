using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ApprovalTests;
using ApprovalTests.Reporters;
using NodaTime.Calendars;
using NUnit.Framework;
using static NodaTime.Xml.XmlSchemaDefinition;

namespace NodaTime.Test.Xml
{
    [UseReporter(typeof(DiffReporter))]
    public class XmlSchemaTest
    {
        [Test]
        public void XmlSchema()
        {
            var xml = GetXmlSchemaString(NodaTimeXmlSchema);
            Approvals.VerifyWithExtension(xml, "xml");
        }

        [Test]
        public void XmlSchemaExporter()
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace(NodaTimeXmlNamespace.Name, NodaTimeXmlNamespace.Namespace);
            var xmlSchema = GetXmlSchema<NodaTimeObject>();
            var xml = GetXmlSchemaString(xmlSchema, namespaceManager);
            Approvals.VerifyWithExtension(xml, "xml");
        }

        [Test]
        public void ValidXmlDocumentWithAllTypes()
        {
            var xml = @"<?xml version=""1.0""?>
            <NodaTimeObject>
              <MyAnnualDate>01-01</MyAnnualDate>
              <MyDuration>0:00:00:00</MyDuration>
              <MyInstant>1970-01-01T00:00:00Z</MyInstant>
              <MyInterval start=""1970-01-01T00:00:00Z"" end=""1970-01-01T00:00:00Z"" />
              <MyLocalDate>0001-01-01</MyLocalDate>
              <MyLocalDateTime>0001-01-01T00:00:00</MyLocalDateTime>
              <MyLocalTime>00:00:00</MyLocalTime>
              <MyOffset>+00</MyOffset>
              <MyOffsetDate>0001-01-01Z</MyOffsetDate>
              <MyOffsetDateTime>0001-01-01T00:00:00Z</MyOffsetDateTime>
              <MyOffsetTime>00:00:00Z</MyOffsetTime>
              <MyPeriodBuilder>P</MyPeriodBuilder>
              <MyYearMonth>0001-01</MyYearMonth>
              <MyZonedDateTime zone=""UTC"">0001-01-01T00:00:00Z</MyZonedDateTime>
            </NodaTimeObject>";

            var errors = ValidateXml<NodaTimeObject>(xml);
            Assert.IsEmpty(errors);

            var nodaTimeObject = DeserializeXml<NodaTimeObject>(xml);
            Assert.IsNotNull(nodaTimeObject);
        }

        [Test]
        public void UnknownFutureZoneIdIsValid()
        {
            var xml = @"<?xml version=""1.0""?><ZonedDateTime zone=""UnknownFutureZoneId"">0001-01-01T00:00:00Z</ZonedDateTime>";

            var errors = ValidateXml<ZonedDateTime>(xml);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void AnnualDateMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyAnnualDate = new AnnualDate(1, 1) });
        }

        [Test]
        public void AnnualDateMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyAnnualDate = new AnnualDate(12, 31) });
        }

        [Test]
        public void DurationMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyDuration = Duration.MinValue });
        }

        [Test]
        public void InstantMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyInstant = Instant.MinValue });
        }

        [Test]
        public void InstantMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyInstant = Instant.MaxValue });
        }

        [Test]
        public void IntervalMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyInterval = new Interval(Instant.MinValue, Instant.MinValue) });
        }

        [Test]
        public void IntervalMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyInterval = new Interval(Instant.MaxValue, Instant.MaxValue) });
        }

        [Test]
        public void LocalDateMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalDate = LocalDate.MinIsoValue });
        }

        [Test]
        public void LocalDateMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalDate = LocalDate.MaxIsoValue });
        }

        [Test]
        public void LocalDateTimeMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalDateTime = new LocalDateTime(LocalDate.MinIsoValue, LocalTime.MinValue) });
        }

        [Test]
        public void LocalDateTimeMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalDateTime = new LocalDateTime(LocalDate.MaxIsoValue, LocalTime.MaxValue) });
        }

        [Test]
        public void LocalTimeMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalTime = LocalTime.MinValue });
        }

        [Test]
        public void LocalTimeMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyLocalTime = LocalTime.MaxValue });
        }

        [Test]
        public void OffsetMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffset = Offset.MinValue });
        }

        [Test]
        public void OffsetMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffset = Offset.MaxValue });
        }

        [Test]
        public void OffsetDateMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetDate = new OffsetDate(LocalDate.MinIsoValue, Offset.MinValue) });
        }

        [Test]
        public void OffsetDateMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetDate = new OffsetDate(LocalDate.MaxIsoValue, Offset.MaxValue) });
        }

        [Test]
        public void OffsetDateTimeMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetDateTime = new OffsetDateTime(new LocalDateTime(LocalDate.MinIsoValue, LocalTime.MinValue), Offset.MinValue) });
        }

        [Test]
        public void OffsetDateTimeMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetDateTime = new OffsetDateTime(new LocalDateTime(LocalDate.MaxIsoValue, LocalTime.MaxValue), Offset.MaxValue) });
        }

        [Test]
        public void OffsetTimeMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetTime = new OffsetTime(LocalTime.MinValue, Offset.MinValue) });
        }

        [Test]
        public void OffsetTimeMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyOffsetTime = new OffsetTime(LocalTime.MaxValue, Offset.MaxValue) });
        }

        [Test]
        public void PeriodMinValue()
        {
            var period = new Period(int.MinValue, int.MinValue, int.MinValue, int.MinValue, long.MinValue, long.MinValue, long.MinValue, long.MinValue, long.MinValue, long.MinValue);
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyPeriodBuilder = new PeriodBuilder(period) });
        }

        [Test]
        public void PeriodMaxValue()
        {
            var period = new Period(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue);
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyPeriodBuilder = new PeriodBuilder(period) });
        }

        [Test]
        public void YearMonthMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyYearMonth = new YearMonth(GregorianYearMonthDayCalculator.MinGregorianYear, 1) });
        }

        [Test]
        public void YearMonthMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyYearMonth = new YearMonth(GregorianYearMonthDayCalculator.MaxGregorianYear, 12) });
        }

        [Test]
        public void ZonedDateTimeMinValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyZonedDateTime = new ZonedDateTime(Instant.MinValue, DateTimeZone.Utc) });
        }

        [Test]
        public void ZonedDateTimeMaxValue()
        {
            AssertValidSerializedNodaTimeObject(new NodaTimeObject { MyZonedDateTime = new ZonedDateTime(Instant.MaxValue, DateTimeZone.Utc) });
        }

        public class NodaTimeObject
        {
            public AnnualDate MyAnnualDate { get; set; }
            public Duration MyDuration { get; set; }
            public Instant MyInstant { get; set; }
            public Interval MyInterval { get; set; }
            public LocalDate MyLocalDate { get; set; }
            public LocalDateTime MyLocalDateTime { get; set; }
            public LocalTime MyLocalTime { get; set; }
            public Offset MyOffset { get; set; }
            public OffsetDate MyOffsetDate { get; set; }
            public OffsetDateTime MyOffsetDateTime { get; set; }
            public OffsetTime MyOffsetTime { get; set; }
            public PeriodBuilder MyPeriodBuilder { get; set; } = new PeriodBuilder();
            public YearMonth MyYearMonth { get; set; }
            public ZonedDateTime MyZonedDateTime { get; set; }
        }

        private static void AssertValidSerializedNodaTimeObject(NodaTimeObject value)
        {
            var xml = SerializeXml<NodaTimeObject>(value);
            var errors = ValidateXml<NodaTimeObject>(xml);
            Assert.IsEmpty(errors);
        }

        private static XmlSchema GetXmlSchema<T>()
        {
            var schemas = new XmlSchemas();
            var xmlImporter = new XmlReflectionImporter();
            var xmlExporter = new XmlSchemaExporter(schemas);
            xmlExporter.ExportTypeMapping(xmlImporter.ImportTypeMapping(typeof(T)));
            return schemas.Single();
        }

        private static T DeserializeXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader);
        }

        private static string SerializeXml<T>(object value)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        private static IEnumerable<string> ValidateXml<T>(string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);
            document.Schemas.Add(GetXmlSchema<T>());
            document.Schemas.Add(NodaTimeXmlSchema);
            var errorMessages = new List<string>();
            document.Validate((sender, args) => errorMessages.Add($"[{args.Severity}] {args.Message}"));
            return errorMessages;
        }

        private static string GetXmlSchemaString(XmlSchema xmlSchema, XmlNamespaceManager? namespaceManager = null)
        {
            var encoding = new UTF8Encoding();
            var memoryStream = new MemoryStream();
            var settings = new XmlWriterSettings { Indent = true, NewLineChars = "\n", Encoding = encoding };
            using var xmlWriter = XmlWriter.Create(memoryStream, settings);
            xmlSchema.Write(xmlWriter, namespaceManager);
            return encoding.GetString(memoryStream.ToArray());
        }
    }
}