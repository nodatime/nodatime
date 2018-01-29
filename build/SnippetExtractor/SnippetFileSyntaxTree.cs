// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetExtractor
{
    /// <summary>
    /// Wrapper around a Roslyn syntax tree representing a file within a snippet project.
    /// </summary>
    public sealed class SnippetFileSyntaxTree
    {
        private readonly SyntaxTree tree;
        private readonly SemanticModel model;
        private readonly INamedTypeSymbol snippetType;

        public SnippetFileSyntaxTree(SyntaxTree tree, SemanticModel model)
        {
            this.tree = tree;
            this.model = model;
            snippetType = model.Compilation.GetTypeByMetadataName("NodaTime.Demo.Snippet");
        }

        public IEnumerable<SourceSnippet> GetSnippets() =>
            tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().SelectMany(GetSnippets);

        private IEnumerable<SourceSnippet> GetSnippets(MethodDeclarationSyntax method)
        {
            // Note: this won't get using directives in namespace declarations, but hey...
            var usings = method.SyntaxTree.GetCompilationUnitRoot().Usings.Select(uds => uds.ToString());
            var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var snippetFor in invocations.Where(IsSnippetMethod("For")))
            {
                var arg = snippetFor.ArgumentList.Arguments[0].Expression;
                var targetSymbol = model.GetSymbolInfo(arg).Symbol;
                if (targetSymbol == null)
                {
                    throw new Exception($"Couldn't get a symbol for Snippet.For argument: {snippetFor.ToString()}");
                }
                // docfx UIDs don't have the M: (etc) prefix.
                var uid = targetSymbol.GetDocumentationCommentId().Substring(2);
                var block = snippetFor.Ancestors().OfType<BlockSyntax>().First();
                yield return new SourceSnippet(uid, block.GetLines(), usings);
            }
        }

        private Func<InvocationExpressionSyntax, bool> IsSnippetMethod(string name) =>
            syntax =>
            {
                var symbol = model.GetSymbolInfo(syntax).Symbol;
                return symbol?.ContainingType == snippetType && symbol.Name == name;
            };

        public static async Task<SnippetFileSyntaxTree> CreateAsync(Document document)
        {
            var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
            var model = await document.GetSemanticModelAsync().ConfigureAwait(false);
            return new SnippetFileSyntaxTree(tree, model);
        }
    }
}
