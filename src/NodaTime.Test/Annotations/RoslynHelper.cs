// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Test.Annotations
{
    internal static class RoslynHelper
    {
        private static readonly Lazy<Compilation> compilation = new Lazy<Compilation>(CompileNodaTime);

        internal static Compilation Compilation => compilation.Value;

        private static Compilation CompileNodaTime()
        {
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync("../../../NodaTime/NodaTime.csproj").Result;
            return project.GetCompilationAsync().Result;
        }

        internal static IEnumerable<T> GetSyntaxNodes<T>() where T : SyntaxNode
        {
            return Compilation.SyntaxTrees
                .SelectMany(tree => tree.GetRoot().DescendantNodesAndSelf())
                .OfType<T>();
        }

        internal static SemanticModel GetModel(this SyntaxNode node)
        {
            return Compilation.GetSemanticModel(node.SyntaxTree);
        }

        internal static ISymbol GetSymbol(this SyntaxNode node)
        {
            return node.GetModel().GetSymbolInfo(node).Symbol;
        }

        internal static ISymbol GetDeclaredSymbol(this SyntaxNode node)
        {
            return node.GetModel().GetDeclaredSymbol(node);
        }

        internal static ISymbol GetEnclosingSymbol(this SyntaxNode node)
        {
            return node.GetModel().GetEnclosingSymbol(node.SpanStart);
        }

        internal static bool HasAttribute(this ISymbol symbol, string attributeName)
        {
            // TODO: Check full name instead
            return symbol != null && symbol.GetAttributes().Any(x => x.AttributeClass.Name == attributeName);
        }
    }
}
