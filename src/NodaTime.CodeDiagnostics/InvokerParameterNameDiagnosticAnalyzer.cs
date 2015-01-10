// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace NodaTime.CodeDiagnostics
{
    /// <summary>
    /// Diagnostic analyzer to ensure that InvokerParameterName is being used correctly - that
    /// all arguments to parameters with the attribute are in fact parameter names, using the
    /// nameof operator.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InvokerParameterNameDiagnosticAnalyzer : DiagnosticAnalyzer
    {

        internal static readonly DiagnosticDescriptor InvokerParameterName = Helpers.CreateWarning(
            "Argument should be a parameter using nameof",
            "Argument should be a parameter of the calling member, specified using the nameof operator",
            Category.Correctness);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(InvokerParameterName);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax) context.Node;
            var model = context.SemanticModel;
            var symbol = model.GetSymbolInfo(invocation).Symbol;
            var definition = symbol?.OriginalDefinition;
            if (definition?.Kind != SymbolKind.Method)
            {
                return;
            }
            var method = (IMethodSymbol) definition;
            // TODO: Handle named arguments etc
            var parameters = method.Parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (!parameter.HasAttribute("InvokerParameterNameAttribute"))
                {
                    continue;
                }
                // TODO: Handle named arguments. (It's unclear how to get the argument that maps
                // to a particular parameter.)
                var argument = invocation.ArgumentList.Arguments[i];
                if (argument.Expression.CSharpKind() != SyntaxKind.NameOfExpression)
                {
                    context.ReportDiagnostic(InvokerParameterName, argument);
                    return;
                }

                var nameOf = (NameOfExpressionSyntax) argument.Expression;
                // The same identifier might matches parameters, instance variables, method names, types etc.
                // We're fine so long as at least one of them is a parameter - if a parameter is one of the
                // candidates, then the compiler has found an in-scope parameter, and we're okay.
                var symbolInfo = model.GetSymbolInfo(nameOf.Argument);
                if (symbolInfo.Symbol?.Kind != SymbolKind.Parameter && !symbolInfo.CandidateSymbols.Select(cs => cs.Kind).Contains(SymbolKind.Parameter))
                {
                    context.ReportDiagnostic(InvokerParameterName, argument);
                    return;
                }
            }
        }
    }
}
