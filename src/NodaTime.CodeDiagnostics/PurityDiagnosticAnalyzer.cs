// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NodaTime.CodeDiagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PurityDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableArray<string> ImplicitlyPureMethods = ImmutableArray.Create(
            "Equals", "ToString", "CompareTo", "GetHashCode");
        // TODO: Check that every method in a public value type is
        // explicitly Pure too, except for a few which are implicitly pure.

        internal const string PureAttributeName = "PureAttribute";

        public static readonly DiagnosticDescriptor PureTypeMethodsMustBePure = Helpers.CreateWarning(
            "[Pure] types must not contain public impure methods",
            "[Pure] type {0} contains impure method {1}",
            Category.Correctness);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(PureTypeMethodsMustBePure);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSymbolAction(CheckMethod, SymbolKind.Method);
        }

        private void CheckMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol) context.Symbol;
            var type = method.ContainingType;
            if (!method.IsStatic &&
                method.MethodKind == MethodKind.Ordinary &&
                !ImplicitlyPureMethods.Contains(method.Name) &&
                type.IsValueType &&
                !method.HasAttribute(PureAttributeName) &&
                method.DeclaredAccessibility == Accessibility.Public &&
                type.DeclaredAccessibility == Accessibility.Public)
            {
                context.ReportDiagnostic(PureTypeMethodsMustBePure, context.Symbol, type.Name, method.Name);
            }
        }
    }
}
