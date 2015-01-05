// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NodaTime.CodeDiagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MutabilityDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor AllPublicClassesShouldBeMutableOrImmutable = Helpers.CreateWarning(
            "All public non-static classes should be mutable or immutable",
            "Public non-static class {0} is neither mutable nor immutable",
            Category.Correctness);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            AllPublicClassesShouldBeMutableOrImmutable);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSymbolAction(CheckPublicClasses, SymbolKind.NamedType);
        }

        private void CheckPublicClasses(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol) context.Symbol;
            if (symbol.TypeKind == TypeKind.Class &&
                symbol.DeclaredAccessibility == Accessibility.Public &&
                !symbol.IsStatic &&
                !symbol.HasAttribute("MutableAttribute") &&
                !symbol.HasAttribute("ImmutableAttribute"))
            {
                context.ReportDiagnostic(AllPublicClassesShouldBeMutableOrImmutable, symbol, symbol.Name);
            }
        }
    }
}
