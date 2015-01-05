// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace NodaTime.CodeDiagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class TrustedDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        internal const string TrustedAttributeName = "TrustedAttribute";

        internal static readonly DiagnosticDescriptor PublicMethodWithTrustedParameter = Helpers.CreateWarning(
            "[Trusted] attribute should not appear on public method parameters",
            "Parameter {0} of public method {1} of public type {2} is [Trusted]",
            Category.Correctness);

        internal static readonly DiagnosticDescriptor UntrustedParameterIsTrusted = Helpers.CreateWarning(
            "Untrusted parameters should not be used for arguments to [Trusted] parameters",
            "Parameter {0} of method {1} is passed for a [Trusted] parameter",
            Category.Correctness);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(PublicMethodWithTrustedParameter);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSyntaxNodeAction(PublicMethodParametersShouldNotBeTrusted, SyntaxKind.MethodDeclaration);
            context.RegisterNodaTimeSyntaxNodeAction(FirstUseOfUntrustedParameterShouldNotBeArgumentToTrustedParameter,
                SyntaxKind.Parameter);
        }

        private void PublicMethodParametersShouldNotBeTrusted(SyntaxNodeAnalysisContext context)
        {
            var model = context.SemanticModel;
            IMethodSymbol method = (IMethodSymbol) model.GetDeclaredSymbol(context.Node);
            if (method.DeclaredAccessibility != Accessibility.Public ||
                method.ContainingSymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.HasAttribute(TrustedAttributeName))
                {
                    context.ReportDiagnostic(PublicMethodWithTrustedParameter,
                        parameter, parameter.Name, method.Name, method.ContainingSymbol.Name);
                }
            }
        }

        // TODO: Ideally, a load of static code analysis here, but just checking the first use would be a start.
        private void FirstUseOfUntrustedParameterShouldNotBeArgumentToTrustedParameter(SyntaxNodeAnalysisContext context)
        {
            var parameterSyntax = (ParameterSyntax)context.Node;
            var model = context.SemanticModel;
            var parameter = model.GetDeclaredSymbol(parameterSyntax);
            if (parameter.HasAttribute(TrustedAttributeName))
            {
                return;
            }
            var container = parameter.ContainingSymbol;
            if ((container as IMethodSymbol)?.MethodKind == MethodKind.AnonymousFunction)
            {
                // Be lenient for anonymous functions. Not entirely sure whether this is a good idea,
                // admittedly.
                return;
            }
            var firstUse = container.DeclaringSyntaxReferences
                .SelectMany(syntaxRef => syntaxRef.GetSyntax().DescendantNodes())
                .Select(node => ParameterUsage.ForNode(node, model, parameter))
                .FirstOrDefault(usage => usage != null);
            if (firstUse?.CorrespondingParameter == null)
            {
                return;
            }
            if (firstUse.CorrespondingParameter.HasAttribute(TrustedAttributeName))
            {
                context.ReportDiagnostic(UntrustedParameterIsTrusted, firstUse.UsageNode, parameter.Name, container.Name);
            }
        }
    }
}
