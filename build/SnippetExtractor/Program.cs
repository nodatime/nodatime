// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetExtractor
{
    partial class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Arguments: <project file> <referenced assembly paths> <output directory>");
                return 1;
            }
            try
            {
                ExtractSnippets(args[0], args[1], args[2]).Wait();
                return 0;
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"Error: {e.InnerException}");
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return 1;
            }
        }

        private static async Task ExtractSnippets(string projectFile, string assemblyPaths, string outputDirectory)
        {
            // Make sure snippets run in an invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // TODO: Work out why it doesn't work when targeting netcoreapp1.0
            var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["TargetFramework"] = "net451" });
            var project = await workspace.OpenProjectAsync(projectFile);

            var compilation = await project.GetCompilationAsync();
            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
            if (errors.Any())
            {
                throw new Exception("Project doesn't build:\r\n" + string.Join("\r\n", errors.Select(e => e.GetMessage())));
            }

            // TODO: Sort out better ways of doing async + LINQ. This must be a solved problem.
            // System.Interactive.Async perhaps?
            var snippetFileTasks = project.Documents
                .Select(doc => SnippetFileSyntaxTree.CreateAsync(doc))
                .ToList();
            var snippetFiles = await Task.WhenAll(snippetFileTasks);
            var snippets = snippetFiles
                .SelectMany(file => file.GetSnippets())
                .ToList();

            // TODO: Revisit all this. No need to add to the existing project; just use
            // a fresh CSharpCompilation per snippet.
            // Or possibly a CSharpScriptCompilation, and don't even bother creating a method?

            // Add new documents to the project - we need to make sure that each time we
            // add a document, we add it to the "latest" project. We keep track of the document
            // IDs so we can find them all in the project later, using the DocFX UID in the snippet as the key.
            var uidToDocId = new Dictionary<string, DocumentId>();
            int index = 0;
            foreach (var snippet in snippets)
            {
                index++;
                Document newDoc = project.AddDocument($"Snippet{index}.cs", snippet.ToSeparateFile($"Method{index}"));
                newDoc = await RewriteInvocationsAsync(newDoc);
                newDoc = await RemoveUnusedImportsAsync(newDoc);
                // This prevents more than one snippet per UID, handily...
                uidToDocId.Add(snippet.Uid, newDoc.Id);
                project = newDoc.Project;
            }

            using (var writer = File.CreateText(Path.Combine(outputDirectory, "snippets.md")))
            {
                foreach (var entry in uidToDocId)
                {
                    var uid = entry.Key;
                    var doc = project.GetDocument(entry.Value);
                    var root = await doc.GetSyntaxRootAsync();
                    var tree = root.SyntaxTree;
                    var usings = tree.GetCompilationUnitRoot().Usings.Select(uds => uds.ToString()).ToList();
                    var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
                    var lines = Trim(method.Body.GetLines());

                    writer.WriteLine("---");
                    writer.WriteLine($"uid: {uid}");
                    // TODO: This will overwrite any existing remarks. We'd like a separate "example"
                    // section, but the example tag is expected to be a string[].
                    writer.WriteLine("snippet: *content");
                    writer.WriteLine("---");
                    writer.WriteLine();
                    writer.WriteLine("```csharp");

                    usings.ForEach(writer.WriteLine);
                    writer.WriteLine("...");
                    lines.ForEach(writer.WriteLine);
                    writer.WriteLine("```");

                    var script = string.Join("\r\n", usings.Concat(lines));
                    var options = ScriptOptions.Default
                        .AddReferences(project.MetadataReferences)
                        .AddReferences(assemblyPaths.Split(';').Select(p => Path.GetFullPath(p)));
                    var outputWriter = new StringWriter();
                    var originalOutput = Console.Out;
                    Console.SetOut(outputWriter);
                    try
                    {
                        await CSharpScript.EvaluateAsync(script, options);
                    }
                    finally
                    {
                        Console.SetOut(originalOutput);
                    }
                    writer.WriteLine();
                    writer.WriteLine("Output:");
                    writer.WriteLine();
                    writer.WriteLine("```text");
                    writer.WriteLine(outputWriter.ToString());
                    writer.WriteLine("```");
                    writer.WriteLine();
                }
            }
        }

        private async static Task<Document> RewriteInvocationsAsync(Document document)
        {
            ExpressionSyntax consoleWriteLineExpression =
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Console"),
                    SyntaxFactory.IdentifierName("WriteLine"));
            var model = await document.GetSemanticModelAsync();
            var errors = model.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
            if (errors.Any())
            {
                throw new Exception("Snippet doesn't build:\r\n" + string.Join("\r\n", errors.Select(e => e.GetMessage())));
            }

            var assertType = model.Compilation.GetTypeByMetadataName("NUnit.Framework.Assert");
            var snippetType = model.Compilation.GetTypeByMetadataName("NodaTime.Demo.Snippet");
            var root = await document.GetSyntaxRootAsync();
            return document.WithSyntaxRoot(root.ReplaceNodes(root.DescendantNodes().OfType<InvocationExpressionSyntax>(),
                ReplaceNode));

            SyntaxNode ReplaceNode(InvocationExpressionSyntax oldNode, InvocationExpressionSyntax newNode)
            {
                var symbol = model.GetSymbolInfo(oldNode).Symbol;
                switch (symbol)
                {
                    case IMethodSymbol method when method.ContainingType == assertType && method.Name == "AreEqual":
                        return newNode
                            .WithExpression(consoleWriteLineExpression)
                            .WithArgumentList(SyntaxFactory.ArgumentList().AddArguments(newNode.ArgumentList.Arguments[1]))
                            .WithTriviaFrom(newNode);
                    case IMethodSymbol method when method.ContainingType == assertType && method.Name == "True":
                        return newNode
                            .WithExpression(consoleWriteLineExpression)
                            .WithArgumentList(SyntaxFactory.ArgumentList().AddArguments(newNode.ArgumentList.Arguments[0]))
                            .WithTriviaFrom(newNode);
                    case IMethodSymbol method when method.ContainingType == snippetType && method.Name == "For":
                        return newNode.ArgumentList.Arguments[0].Expression;
                    default: return newNode;
                }
            }
        }

        private async static Task<Document> RemoveUnusedImportsAsync(Document document)
        {
            // TODO: See if there's a better way of doing this.
            var compilation = await document.Project.GetCompilationAsync();
            var tree = await document.GetSyntaxTreeAsync();
            var root = tree.GetRoot();
            var unusedImportNodes = compilation.GetDiagnostics()
                .Where(d => d.Id == "CS8019")
                .Where(d => d.Location?.SourceTree == tree)
                .Select(d => root.FindNode(d.Location.SourceSpan))
                .ToList();
            return document.WithSyntaxRoot(root.RemoveNodes(unusedImportNodes, SyntaxRemoveOptions.KeepNoTrivia));
        }

        private static List<string> Trim(IEnumerable<string> lines)
        {
            // Trim leading and trailing empty lines
            var list = lines
                .SkipWhile(string.IsNullOrWhiteSpace)
                .Reverse()
                .SkipWhile(string.IsNullOrWhiteSpace)
                .Reverse()
                .ToList();

            var leadingWhitespace = list
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Min(line => line.Length - line.TrimStart().Length);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = string.IsNullOrWhiteSpace(list[i]) ? "" : list[i].Substring(leadingWhitespace);
            }
            return list;
        }
    }
}