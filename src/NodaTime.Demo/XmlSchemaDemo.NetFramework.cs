using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;
using Microsoft.CSharp;
using NodaTime.Xml;

namespace NodaTime.Demo
{
    /// <summary>
    /// A schema importer extension for handling NodaTime types.
    /// </summary>
    /// <remarks>Adapted from the SchemaImporterExtension Technology Sample at https://docs.microsoft.com/en-us/dotnet/standard/serialization/schemaimporterextension-technology-sample</remarks>
    public class NodaTimeSchemaImporterExtension : SchemaImporterExtension
    {
        /// <inheritdoc />
        public override string ImportSchemaType(string name, string ns, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
        {
            if (ns == XmlSchemaDefinition.NodaTimeXmlNamespace.Namespace && char.IsUpper(name.FirstOrDefault()))
            {
                return nameof(NodaTime) + "." + name;
            }
            return base.ImportSchemaType(name, ns, context, schemas, importer, compileUnit, mainNamespace, options, codeProvider);
        }
    }

    public static class XmlTypeGenerator
    {
        public static Assembly CreateAssembly(string namespaceName, XmlSchemaSet schemaSet)
        {
            var codeNamespace = new CodeNamespace(namespaceName);
            codeNamespace.AddTypes(schemaSet);
            var assembly = codeNamespace.Compile();
            return assembly;
        }

        private static void AddTypes(this CodeNamespace codeNamespace, XmlSchemaSet schemaSet)
        {
            var schemas = new XmlSchemas();
            foreach (var schema in schemaSet.Schemas().Cast<XmlSchema>())
            {
                schemas.Add(schema);
            }
            var schemaImporter = new XmlSchemaImporter(schemas);
            schemaImporter.Extensions.Add(new NodaTimeSchemaImporterExtension());
            var codeExporter = new XmlCodeExporter(codeNamespace);
            foreach (var schemaObject in schemas.SelectMany(e => e.Items.Cast<XmlSchemaObject>()))
            {
                XmlTypeMapping mapping;
                switch (schemaObject)
                {
                    case XmlSchemaType schemaType:
                        mapping = schemaImporter.ImportSchemaType(schemaType.QualifiedName);
                        break;
                    case XmlSchemaElement schemaElement:
                        mapping = schemaImporter.ImportTypeMapping(schemaElement.QualifiedName);
                        break;
                    default:
                        continue;
                }
                codeExporter.ExportTypeMapping(mapping);
            }
            CodeGenerator.ValidateIdentifiers(codeNamespace);
        }

        private static Assembly Compile(this CodeNamespace codeNamespace)
        {
            var referencedAssemblies = new List<string> { typeof(Instant).Assembly.Location, "System.dll", "System.Xml.dll" };
            try
            {
                // mono needs the absolute path to netstandard.dll, but Assembly.Load("netstandard") fails on .NET Framework
                referencedAssemblies.Add(Assembly.Load("netstandard").Location);
            }
            catch (Exception)
            {
                // Using netstandard.dll without the absolute path works fine on .NET Framework
                referencedAssemblies.Add("netstandard.dll");
            }
            var compileUnit = new CodeCompileUnit { Namespaces = { codeNamespace } };
            compileUnit.ReferencedAssemblies.AddRange(referencedAssemblies.ToArray());
            var compilerParameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
            var codeProvider = new CSharpCodeProvider();
            var writer = new StringWriter();
            codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
            var result = codeProvider.CompileAssemblyFromDom(compilerParameters, compileUnit);
            if (result.Errors.HasErrors)
            {
                throw new AggregateException(result.Errors.Cast<CompilerError>().Select(e => new Exception($"[{e.ErrorNumber}] {e.Line}{e.Column}: {e.ErrorText}")));
            }
            return result.CompiledAssembly;
        }
    }
}