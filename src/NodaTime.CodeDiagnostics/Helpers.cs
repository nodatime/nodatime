// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace NodaTime.CodeDiagnostics
{
    /// <summary>
    /// Helper methods used in multiple analyzers.
    /// </summary>
    internal static class Helpers
    {
        internal static bool IsNodaTime(Compilation compilation)
        {
            return compilation.AssemblyName == "NodaTime";
        }

        /// <summary>
        /// Registers a syntax node action that first filters by project.
        /// </summary>
        internal static void RegisterNodaTimeSyntaxNodeAction<TLanguageKindEnum>(this AnalysisContext context,
            Action<SyntaxNodeAnalysisContext> action, params TLanguageKindEnum[] syntaxKinds) where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(ctx =>
            {
                if (IsNodaTime(ctx.SemanticModel.Compilation))
                {
                    action(ctx);
                }
            }, syntaxKinds);
        }

        /// <summary>
        /// Registers a symbol action that first filters by project.
        /// </summary>
        internal static void RegisterNodaTimeSymbolAction(this AnalysisContext context,
            Action<SymbolAnalysisContext> action, params SymbolKind[] symbolKinds)
        {
            context.RegisterSymbolAction(ctx =>
            {
                if (IsNodaTime(ctx.Compilation))
                {
                    action(ctx);
                }
            }, symbolKinds);
        }

        internal static Location FirstLocation(this ISymbol symbol)
        {
            return symbol.Locations[0];
        }

        internal static bool HasAttribute(this ISymbol symbol, string attributeName)
        {
            // TODO: Check full name instead
            return symbol.GetAttributes().Any(x => x.AttributeClass.Name == attributeName);
        }
    }
}
