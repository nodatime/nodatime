// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetExtractor
{
    public sealed class SnippetRewriter
    {
        // Additional imports, for Console.WriteLine and to make
        // up for the fact that the script isn't in a namespace
        private static readonly string[] ExtraUsings =
        {
             "using System;",
             "using NodaTime;",
             "using NodaTime.Demo;"
        };

        private readonly ScriptOptions options;

        public SnippetRewriter(Project project)
        {
            options = ScriptOptions.Default
                .AddReferences(project.MetadataReferences)
                .AddReferences(new[] { MetadataReference.CreateFromFile(project.OutputFilePath) });
        }

        public async Task<RewrittenSnippet> RewriteSnippetAsync(SourceSnippet snippet)
        {
            var usings = snippet.Usings.Concat(ExtraUsings).Distinct().OrderBy(x => x.TrimEnd(';'));
            var text = string.Join("\r\n", usings.Concat(new[] { "" }).Concat(Trim(snippet.Lines)));
            var tree = CSharpSyntaxTree.ParseText(text, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));

            Compilation compilation = CSharpCompilation.Create("Foo", new[] { tree }, options.MetadataReferences)
                .CheckSuccessful();
            // TODO: Replace var with explicit declarations?
            compilation = RewriteInvocations(compilation).CheckSuccessful();
            compilation = RemoveUnusedImports(compilation).CheckSuccessful();
            var script = CSharpScript.Create(compilation.SyntaxTrees.Single().ToString(), options);
            var output = await RunScriptAsync(script);
            return new RewrittenSnippet(script.Code, output, snippet.Uid);
        }

        private static Compilation RewriteInvocations(Compilation compilation)
        {
            ExpressionSyntax consoleWriteLineExpression =
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Console"),
                    SyntaxFactory.IdentifierName("WriteLine"));
            var tree = compilation.SyntaxTrees.Single();
            var model = compilation.GetSemanticModel(tree);

            var assertType = model.Compilation.GetTypeByMetadataName("NUnit.Framework.Assert");
            var snippetType = model.Compilation.GetTypeByMetadataName("NodaTime.Demo.Snippet");
            var root = tree.GetRoot();
            var newRoot = root.ReplaceNodes(root.DescendantNodes().OfType<InvocationExpressionSyntax>(), ReplaceNode);
            // Force it back to have a kind of Script... not sure why this is required.
            var newTree = newRoot.SyntaxTree.WithRootAndOptions(newRoot, tree.Options);
            return compilation.ReplaceSyntaxTree(tree, newTree);

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

        private static Compilation RemoveUnusedImports(Compilation compilation)
        {
            // TODO: See if there's a better way of doing this.
            // There's Document.GetLanguageService<IOrganizeImportsService>, but that
            // requires a Document.
            var tree = compilation.SyntaxTrees.Single();
            var root = tree.GetRoot();
            var unusedImportNodes = compilation.GetDiagnostics()
                .Where(d => d.Id == "CS8019")
                .Where(d => d.Location?.SourceTree == tree)
                .Select(d => root.FindNode(d.Location.SourceSpan))
                .ToList();
            var newRoot = root.RemoveNodes(unusedImportNodes, SyntaxRemoveOptions.KeepNoTrivia);
            // Force it back to have a kind of Script... not sure why this is required. 
            var newTree = newRoot.SyntaxTree.WithRootAndOptions(newRoot, tree.Options);
            return compilation.ReplaceSyntaxTree(tree, newTree);
        }

        /// <summary>
        /// Trims common leading whitespace from the given lines. Whitespace-only lines
        /// are not considered when determining how much to trim.
        /// </summary>
        private static List<string> Trim(IEnumerable<string> lines)
        {
            // TODO: Use Microsoft.CodeAnalysis.Formatting instead? That requires a workspace...
            // which we do have, admittedly, but the script we want to format isn't in that workspace.
            // Also, it means reformatting the code, whereas we may want it exactly as written in the snippet.

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

        /// <summary>
        /// Runs the given script, capturing its console output.
        /// </summary>
        private static async Task<string> RunScriptAsync(Script script)
        {
            var outputWriter = new StringWriter();
            var originalOutput = Console.Out;
            Console.SetOut(outputWriter);
            try
            {
                await script.RunAsync();
            }
            finally
            {
                Console.SetOut(originalOutput);
            }
            return outputWriter.ToString();
        }
    }
}
