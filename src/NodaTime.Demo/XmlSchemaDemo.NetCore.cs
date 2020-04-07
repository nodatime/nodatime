using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NodaTime.Xml;
using XmlSchemaClassGenerator;

namespace NodaTime.Demo
{
    public class MemoryOutputWriter : OutputWriter
    {
        private readonly List<(string namespaceName, string source)> content = new List<(string namespaceName, string source)>();

        public IEnumerable<(string namespaceName, string source)> Content => content;

        public override void Write(CodeNamespace codeNamespace)
        {
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNamespace);

            using var writer = new StringWriter();
            Write(writer, compileUnit);
            content.Add((codeNamespace.Name, writer.ToString()));
        }
    }

    public class NodaTimeNamingProvider : NamingProvider
    {
        public NodaTimeNamingProvider(NamingScheme namingScheme) : base(namingScheme)
        {
        }

        protected override string QualifiedNameToTitleCase(XmlQualifiedName qualifiedName)
        {
            if (qualifiedName.Namespace == XmlSchemaDefinition.NodaTimeXmlNamespace.Namespace)
            {
                return qualifiedName.Name;
            }
            return base.QualifiedNameToTitleCase(qualifiedName);
        }
    }

    public static class XmlTypeGenerator
    {
        public static Assembly CreateAssembly(string namespaceName, XmlSchemaSet schemaSet)
        {
            var writer = new MemoryOutputWriter();
            var namespaceProvider = new Dictionary<NamespaceKey, string>
                {
                    [new NamespaceKey("")] = namespaceName,
                    [new NamespaceKey(XmlSchemaDefinition.NodaTimeXmlNamespace.Namespace)] = nameof(NodaTime),
                }
                .ToNamespaceProvider(new GeneratorConfiguration { NamespacePrefix = namespaceName }.NamespaceProvider.GenerateNamespace);
            var generator = new Generator
            {
                OutputWriter = writer,
                GenerateNullables = true,
                NamespaceProvider = namespaceProvider,
                NamingProvider = new NodaTimeNamingProvider(NamingScheme.PascalCase)
            };

            generator.Generate(schemaSet);

            var systemDependencies = new[] { "netstandard", "System.ComponentModel.Primitives", "System.Diagnostics.Tools", "System.Private.CoreLib", "System.Private.Xml", "System.Runtime", "System.Xml.XmlSerializer" };
            var references = systemDependencies.Select(e => Assembly.Load(e).Location)
                .Append(typeof(Instant).Assembly.Location)
                .Select(p => MetadataReference.CreateFromFile(p));
            var options = new CSharpParseOptions(kind: SourceCodeKind.Regular, languageVersion: LanguageVersion.Latest);
            var syntaxTrees = writer.Content.Where(e => e.namespaceName != nameof(NodaTime))
                .Select(e => CSharpSyntaxTree.ParseText(e.source, options));
            var compilation = CSharpCompilation.Create(namespaceName, syntaxTrees)
                .AddReferences(references)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var dllStream = new MemoryStream();
            var result = compilation.Emit(dllStream);
            if (result.Success)
                return Assembly.Load(dllStream.ToArray());
            throw new AggregateException(result.Diagnostics.Select(e => new Exception(e.ToString())));
        }
    }
}