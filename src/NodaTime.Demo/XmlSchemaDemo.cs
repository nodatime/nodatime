// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;

namespace NodaTime.Demo
{
    public partial class XmlSchemaDemo
    {
        /// <summary>
        /// Provides mapping from <see cref="XmlSchemaSet"/> types to C# types.
        /// </summary>
        private interface IXmlSchemaAssemblyCreator
        {
            /// <summary>
            /// An assembly with types defined in the given XML schema set mapped to C# types.
            /// </summary>
            /// <param name="namespaceName">The name of the C# namespace in which types will be created.</param>
            /// <param name="schemaSet">A <see cref="XmlSchemaSet"/> defining a set of types.</param>
            /// <returns>An assembly with types defined in the <paramref name="schemaSet"/> mapped to C# types.</returns>
            Assembly CreateAssembly(string namespaceName, XmlSchemaSet schemaSet);
        }

        [Test]
        [TestCase(typeof(XmlCodeExporterAssemblyCreator))]
        [TestCase(typeof(XmlSchemaClassGeneratorAssemblyCreator))]
        public void DynamicCodeGeneration(Type assemblyGeneratorType)
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
            schemaSet.Add(Xml.XmlSchemaDefinition.NodaTimeXmlSchema);
            const string namespaceName = nameof(XmlSchemaDemo);
            var assemblyGenerator = (IXmlSchemaAssemblyCreator)Activator.CreateInstance(assemblyGeneratorType)!;
            Assembly assembly;
            try
            {
                assembly = assemblyGenerator.CreateAssembly(namespaceName, schemaSet);
            }
            catch (PlatformNotSupportedException exception) when (exception.Message.Contains(assemblyGeneratorType.Name))
            {
                Assert.Ignore(exception.Message);
                return;
            }
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