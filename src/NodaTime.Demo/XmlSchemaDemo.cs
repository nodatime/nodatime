using System;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using NodaTime.Xml;
using NUnit.Framework;

namespace NodaTime.Demo
{
    public class XmlSchemaDemo
    {
        [Test]
        public void DynamicCodeGeneration()
        {
            var schemaXmlReader = new StringReader(@"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns:nodatime=""https://nodatime.org/api/"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:import namespace=""https://nodatime.org/api/"" />
  <xs:element name=""Person"" nillable=""true"" type=""Person"" />
  <xs:complexType name=""Person"">
    <xs:sequence>
      <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Name"" type=""xs:string"" />
      <xs:element minOccurs=""1"" maxOccurs=""1"" name=""BirthDate"" type=""nodatime:LocalDate"" />
    </xs:sequence>
  </xs:complexType>
</xs:schema>");
            var personXmlReader = new StringReader(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Person>
  <Name>Jon Skeet</Name>
  <BirthDate>1976-06-19</BirthDate>
</Person>");

            var personXmlSchema = XmlSchema.Read(schemaXmlReader, (sender, args) => throw new Exception($"[{args.Severity}] {args.Message}"));
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(personXmlSchema);
            schemaSet.Add(XmlSchemaDefinition.NodaTimeXmlSchema);
            const string namespaceName = nameof(XmlSchemaDemo);
            var assembly = XmlTypeGenerator.CreateAssembly(namespaceName, schemaSet);
            var personElement = personXmlSchema.Elements.Values.Cast<XmlSchemaElement>().Single();
            var personType = assembly.GetExportedTypes().Single(t => t.FullName == $"{namespaceName}.{personElement.Name}");
            var serializer = new XmlSerializer(personType);
            dynamic person = serializer.Deserialize(personXmlReader);
            Assert.IsInstanceOf<string>(person.Name);
            Assert.AreEqual("Jon Skeet", person.Name);
            Assert.IsInstanceOf<LocalDate>(person.BirthDate);
            Assert.AreEqual(new LocalDate(1976, 06, 19), person.BirthDate);
        }
    }
}