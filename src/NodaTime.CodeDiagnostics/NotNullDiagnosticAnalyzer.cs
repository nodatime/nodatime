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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class NotNullDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor FirstNotNullParameterUseIsMemberAccess = Helpers.CreateWarning(
            "First use of parameter is member access; check for nullity first",
            "First use of parameter is member access; check for nullity first",
            Category.Correctness);

        internal static readonly DiagnosticDescriptor NotNullCheckWithoutParameter = Helpers.CreateWarning(
            "Preconditions.CheckNotNull should check an untrusted [NotNull] parameter",
            "Preconditions.CheckNotNull should check an untrusted [NotNull] parameter",
            Category.Clarity);

        internal static readonly DiagnosticDescriptor DebugCheckNotNullWithoutParameter = Helpers.CreateWarning(
            "Preconditions.DebugCheckNotNull should check a trusted [NotNull] parameter",
            "Preconditions.DebugCheckNotNull should check a trusted [NotNull] parameter",
            Category.Clarity);

        internal static readonly DiagnosticDescriptor NotNullParameterIsNotChecked = Helpers.CreateWarning(
            "[NotNull] parameter unchecked",
            "[NotNull] parameter {0} is not checked for nullity",
            Category.Correctness);

        internal static readonly DiagnosticDescriptor NotNullParameterCheckedWithWrongName = Helpers.CreateWarning(
            "[NotNull] parameter checked with the wrong name",
            "[NotNull] parameter {0} is passed as an argument for a [NotNull] parameter {1}",
            Category.Correctness);

        internal static readonly DiagnosticDescriptor ParameterImplicitlyNotNullCheckedWithoutAttribute = Helpers.CreateWarning(
            "Nullable parameter is used for call as [NotNull] argument",
            "Parameter {0} is passed as an argument for a [NotNull] parameter; it should be marked as [NotNull]",
            Category.Clarity);

        internal static readonly DiagnosticDescriptor NotNullParameterIsValueType = Helpers.CreateWarning(
            "[NotNull] should not be applied to value type parameters",
            "Parameter {0} is marked as [NotNull] but is of type {1} which is a value type",
            Category.Inconsistency);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                NotNullCheckWithoutParameter,
                DebugCheckNotNullWithoutParameter,
                NotNullParameterIsNotChecked,
                NotNullParameterCheckedWithWrongName,
                ParameterImplicitlyNotNullCheckedWithoutAttribute,
                NotNullParameterIsValueType,
                FirstNotNullParameterUseIsMemberAccess);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSyntaxNodeAction(PreconditionsCheckNotNullArgumentsShouldBeNotNullParameteters,
                SyntaxKind.InvocationExpression);
            context.RegisterNodaTimeSyntaxNodeAction(UberCheckParameter, SyntaxKind.Parameter);
            context.RegisterNodaTimeSyntaxNodeAction(NotNullParametersShouldNotBeValueTypes, SyntaxKind.Parameter);
            // This never gets called, for some reason...
            context.RegisterSymbolAction(NotNullParametersShouldNotBeValueTypes, SymbolKind.Parameter);
        }

        /// <summary>
        /// Single method to have a look at how a parameter is used within a method (often stopping
        /// at the first usage) and then checking whether that usage is appropriate for the
        /// attributes applied to the parameter.
        /// </summary>
        private void UberCheckParameter(SyntaxNodeAnalysisContext context)
        {
            var parameterSyntax = (ParameterSyntax)context.Node;
            var model = context.SemanticModel;
            var parameterSymbol = model.GetDeclaredSymbol(parameterSyntax);
            if (parameterSymbol.Type.IsValueType)
            {
                return;
            }

            bool notNull = HasNotNullAttribute(parameterSymbol);
            var container = parameterSymbol.ContainingSymbol;

            if ((container as IMethodSymbol)?.MethodKind == MethodKind.AnonymousFunction)
            {
                // Be lenient for anonymous functions. Not entirely sure whether this is a good idea,
                // admittedly.
                return;
            }

            if ((container as IMethodSymbol)?.IsAbstract == true)
            {
                // Abstract methods can't check their parameters
                return;
            }

            var firstUse = container.DeclaringSyntaxReferences
                .SelectMany(syntaxRef => syntaxRef.GetSyntax().DescendantNodes())
                .Select(node => ParameterUsage.ForNode(node, model, parameterSymbol))
                .FirstOrDefault(usage => usage != null);
            if (firstUse == null)
            {
                // No "interesting" usage that could have been some other kind of check. (Could
                // be used in an assignment etc.)
                if (notNull)
                {
                    context.ReportDiagnostic(NotNullParameterIsNotChecked, parameterSyntax.Identifier, parameterSymbol.Name);
                }
                return;
            }
            // First use is parameter.Foo or parameter.Foo(). This might always be invalid... see how much
            // noise it causes. Also consider extension methods, where it's valid to be null.
            if (firstUse.CorrespondingParameter == null)
            {
                if (container.DeclaredAccessibility == Accessibility.Public && container.ContainingType.DeclaredAccessibility == Accessibility.Public)
                {
                    context.ReportDiagnostic(FirstNotNullParameterUseIsMemberAccess, firstUse.UsageNode);
                }
                return;
            }

            // Okay, it's an invocation (method, operator or whatever). It might be a CheckNotNull call, in which case we'll let the other
            // rule handle it.

            // TODO: Do we need to check that the right argument is being used here?
            if (IsCheckNotNull(firstUse.InvokedMember))
            {
                return;
            }

            // If the first usage is an equality check, let's assume it's being handled correctly. Examples:
            // - Checking inequality against non-null values (e.g. in xxx)
            // - Checking equality against the null literal (e.g. in Period.Compare)
            if (firstUse.InvokedMember.Name == "op_Equality" || firstUse.InvokedMember.Name == "op_Inequality")
            {
                return;
            }

            if (HasNotNullAttribute(firstUse.CorrespondingParameter))
            {
                // TODO: Consider the interplay of Trusted here.
                if (!notNull)
                {
                    context.ReportDiagnostic(ParameterImplicitlyNotNullCheckedWithoutAttribute, firstUse.UsageNode,
                        parameterSymbol.Name);
                }
                if (firstUse.CorrespondingParameter.Name != parameterSymbol.Name)
                {
                    context.ReportDiagnostic(NotNullParameterCheckedWithWrongName, firstUse.UsageNode,
                        parameterSymbol.Name, firstUse.CorrespondingParameter.Name);
                }
                return;
            }
            if (notNull)
            {
                // First usage doesn't check for nullity, despite this being a NotNull parameter.
                context.ReportDiagnostic(NotNullParameterIsNotChecked, parameterSyntax.Identifier, parameterSymbol.Name);
            }

            // TODO: Check later uses?
        }

        private void PreconditionsCheckNotNullArgumentsShouldBeNotNullParameteters(SyntaxNodeAnalysisContext context)
        {
            var invocationSyntax = (InvocationExpressionSyntax)context.Node;
            var model = context.SemanticModel;
            var invokedSymbol = model.GetSymbolInfo(invocationSyntax).Symbol;
            var invokedDefinition = invokedSymbol?.OriginalDefinition as IMethodSymbol;
            if (invokedDefinition?.Kind != SymbolKind.Method)
            {
                return;
            }

            if (!IsCheckNotNull(invokedDefinition))
            {
                return;
            }

            // TODO: Is there really no simpler way than this?
            var callingMethod = model.GetEnclosingSymbol(invocationSyntax.GetLocation().SourceSpan.Start) as IMethodSymbol;
            if (callingMethod == null)
            {
                // Okay, this is odd... but let's not warn.
                // TODO: Maybe we should?
                return;
            }
            if (callingMethod.MethodKind == MethodKind.AnonymousFunction)
            {
                // TODO: Anonymous functions can't specify attributes in the parameters,
                // so we should really look at the delegate type it's being converted to.
                // I've no idea how to get that information though...
                // Maybe methodSymbol.AssociatedAnonymousDelegate?
                return;
            }
            // TODO: Identify the "thing to check for not being null" better.
            var argument = invocationSyntax.ArgumentList.Arguments[0];
            var argumentSymbol = model.GetSymbolInfo(argument.Expression);
            if (argumentSymbol.Symbol?.Kind != SymbolKind.Parameter)
            {
                context.ReportDiagnostic(NotNullCheckWithoutParameter, context.Node);
                return;
            }
            var symbolToCheck = argumentSymbol.Symbol;
            if (callingMethod.MethodKind == MethodKind.PropertySet && argumentSymbol.Symbol.Name == "value")
            {
                // For a setter, if we're in the setter, then the "value" parameter's nullability is
                // determined by the property itself.
                symbolToCheck = callingMethod.AssociatedSymbol;
            }
            if (!HasNotNullAttribute(symbolToCheck))
            {
                context.ReportDiagnostic(NotNullCheckWithoutParameter, context.Node);
            }

            // Finally, check that we're calling the right variant (debug or not) based on whether
            // the parameter is trusted.
            var trusted = symbolToCheck.HasAttribute(TrustedDiagnosticAnalyzer.TrustedAttributeName);
            if (trusted && invokedDefinition.Name == "CheckNotNull")
            {
                context.ReportDiagnostic(NotNullCheckWithoutParameter, context.Node);
            }
            else if (!trusted && invokedDefinition.Name == "DebugCheckNotNull")
            {
                context.ReportDiagnostic(DebugCheckNotNullWithoutParameter, context.Node);
            }
        }

        private void NotNullParametersShouldNotBeValueTypes(SyntaxNodeAnalysisContext context)
        {
            var parameter = context.SemanticModel.GetDeclaredSymbol((ParameterSyntax) context.Node);
            if (HasNotNullAttribute(parameter) && parameter.Type.IsValueType)
            {
                context.ReportDiagnostic(NotNullParameterIsValueType, parameter, parameter.Name, parameter.Type.Name);
            }
        }

        private void NotNullParametersShouldNotBeValueTypes(SymbolAnalysisContext context)
        {
            var parameter = (IParameterSymbol) context.Symbol;
            if (HasNotNullAttribute(parameter) && parameter.Type.IsValueType)
            {
                context.ReportDiagnostic(NotNullParameterIsValueType, parameter, parameter.Name, parameter.Type.Name);
            }
        }

        private static bool HasNotNullAttribute(ISymbol symbol)
        {
            return symbol.HasAttribute("NotNullAttribute");
        }

        private static bool IsCheckNotNull(IMethodSymbol method)
        {
            // TODO: Validate that DebugCheckNotNull is only used with Trusted?
            return method != null &&
                   (method.Name == "CheckNotNull" || method.Name == "DebugCheckNotNull") &&
                   method.ContainingType?.Name == "Preconditions" &&
                   // TODO: How do I get NodaTime.Utility? Surely there's a way of getting the full namespace name...
                   method.ContainingType?.ContainingNamespace?.Name == "Utility";
        }
    }
}
