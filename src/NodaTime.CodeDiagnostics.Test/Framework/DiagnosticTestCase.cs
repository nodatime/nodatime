// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace NodaTime.CodeDiagnostics.Test.Framework
{
    public sealed class DiagnosticTestCase
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromAssembly(typeof(object).Assembly);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromAssembly(typeof(Enumerable).Assembly);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromAssembly(typeof(CSharpCompilation).Assembly);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromAssembly(typeof(Compilation).Assembly);

        private readonly List<SourceFile> files;
        private readonly string projectName;
        private readonly DiagnosticAnalyzer analyzer;

        private readonly List<FileLocation> locations;

        private readonly List<DescriptorAndMessage> descriptors;

        public DiagnosticTestCase(DiagnosticAnalyzer analyzer, string projectName, char expectedLocationMarker, string[] sources)
        {
            this.analyzer = analyzer;
            this.projectName = projectName;
            files = new List<SourceFile>();
            locations = new List<FileLocation>();
            descriptors = new List<DescriptorAndMessage>();
            for (int i = 0; i < sources.Length; i++)
            {
                string source = sources[i];
                string path = string.Format("Test{0:00}.cs", i);
                locations.AddRange(GetLocationsFromSource(path, source, expectedLocationMarker));
                files.Add(new SourceFile(content: source.Replace(expectedLocationMarker.ToString(), ""), path: path));
            }
        }

        public DiagnosticTestCase ExpectDiagnostic(DiagnosticDescriptor descriptor, params object[] arguments)
        {
            descriptors.Add(new DescriptorAndMessage(descriptor, string.Format(descriptor.MessageFormat, arguments)));
            return this;
        }

        public void Verify()
        {
            Assert.AreEqual(descriptors.Count, locations.Count, "Source code specifies {0} locations; {1} diagnostics provided", locations.Count, descriptors.Count);
            var project = CreateProject();
            
            var compilation = project.GetCompilationAsync().Result;
            var driver = AnalyzerDriver.Create(compilation, ImmutableArray.Create(analyzer), null, out compilation, CancellationToken.None);
            // TODO: Work out why this is needed.
            var discarded = compilation.GetDiagnostics();
            var diags = driver.GetDiagnosticsAsync().Result;
            // TODO: Work out what the code in DiagnosticVerifier was doing here... it seems unnecessarily complicated.

            // TODO: Handle diagnostics with no location?
            var actual = diags
                .Select(SimplifiedDiagnostic.FromDiagnostic)
                .OrderBy(sd => sd.Location)
                .ToList();
            var expected = descriptors.Zip(locations, (d, l) => d.WithLocation(l)).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        private static IEnumerable<FileLocation> GetLocationsFromSource(string file, string source,
            char expectedLocationMarker)
        {
            var lines = BreakIntoLines(source).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int index;
                while ((index = line.IndexOf(expectedLocationMarker)) != -1)
                {
                    yield return new FileLocation(file, i + 1, index + 1);
                    line = line.Remove(index, 1);
                }
            }
        }

        private static IEnumerable<string> BreakIntoLines(string source)
        {
            using (var reader = new StringReader(source))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private class DescriptorAndMessage
        {
            private readonly DiagnosticDescriptor descriptor;
            private readonly string message;

            internal DescriptorAndMessage(DiagnosticDescriptor descriptor, string message)
            {
                this.descriptor = descriptor;
                this.message = message;
            }

            public SimplifiedDiagnostic WithLocation(FileLocation fileLocation)
            {
                return new SimplifiedDiagnostic(descriptor.Id, fileLocation, message);
            }
        }

        private Project CreateProject()
        {
            var projectId = ProjectId.CreateNewId(debugName: projectName);

            var solution = new CustomWorkspace()
                .CurrentSolution
                .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference);

            foreach (var file in files)
            {
                var documentId = DocumentId.CreateNewId(projectId, debugName: file.Path);
                solution = solution.AddDocument(documentId, file.Path, SourceText.From(file.Content));
            }
            return solution.GetProject(projectId);
        }
    }
}