// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

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

        internal static bool HasAttribute(this ISymbol symbol, string attributeName)
        {
            // TODO: Check full name instead
            return symbol.GetAttributes().Any(x => x.AttributeClass.Name == attributeName);
        }

        internal static void ReportDiagnostic(this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
        }

        internal static void ReportDiagnostic(this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode syntaxNode,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, syntaxNode.GetLocation(), messageArgs));
        }

        internal static void ReportDiagnostic(this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            ISymbol symbol,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, symbol.Locations[0], messageArgs));
        }

        internal static void ReportDiagnostic(this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
        }

        internal static void ReportDiagnostic(this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxNode syntaxNode,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, syntaxNode.GetLocation(), messageArgs));
        }

        internal static void ReportDiagnostic(this SymbolAnalysisContext context,
            DiagnosticDescriptor descriptor,
            ISymbol symbol,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, symbol.Locations[0], messageArgs));
        }

        internal static DiagnosticDescriptor CreateWarning(string title, string messageFormat, Category category,
            [CallerMemberName] string id = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat, category.ToString(),
                DiagnosticSeverity.Warning, isEnabledByDefault: true);
        }

        internal static DiagnosticDescriptor CreateError(string title, string messageFormat, Category category,
            [CallerMemberName] string id = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat, category.ToString(),
                DiagnosticSeverity.Error, isEnabledByDefault: true);
        }

        internal static DiagnosticDescriptor CreateInfo(string title, string messageFormat, Category category,
            [CallerMemberName] string id = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat, category.ToString(),
                DiagnosticSeverity.Info, isEnabledByDefault: true);
        }
    }
}
