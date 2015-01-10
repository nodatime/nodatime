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
    /// Diagnostic analyzer to ensure that ReadWriteForEfficiency is being used correctly
    /// - in other words, that assignments only occur within constructors.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class ReadWriteForEfficiencyDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor ReadWriteForEfficiencyAssignmentOutsideConstructor = Helpers.CreateWarning(
            "[ReadWriteForEfficiency] fields should only be assigned in constructors",
            "[ReadWriteForEfficiency] field {0} should only be assigned in constructors",
            Category.Correctness);

        internal static DiagnosticDescriptor ReadWriteForEfficiencyNotPrivate = Helpers.CreateWarning(
            "[ReadWriteForEfficiency] fields should be private",
            "[ReadWriteForEfficiency] field {0} should be private",
            Category.Correctness);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(ReadWriteForEfficiencyAssignmentOutsideConstructor,
                ReadWriteForEfficiencyNotPrivate);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.Attribute);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            AttributeSyntax attribute = (AttributeSyntax)context.Node;
            var model = context.SemanticModel;
            var attributeSymbol = model.GetSymbolInfo(attribute).Symbol?.ContainingSymbol;
            // TODO: Write a convenience method to do this properly, with namespace as well...
            if (attributeSymbol?.Name != "ReadWriteForEfficiencyAttribute")
            {
                return;
            }
            // ReadWriteForEfficiency can only be applied to fields, so the grandparent node must
            // be a field declaration syntax.
            var fieldDeclaration = (FieldDeclarationSyntax)attribute.Parent.Parent;
            foreach (var individualField in fieldDeclaration.Declaration.Variables)
            {
                var fieldSymbol = model.GetDeclaredSymbol(individualField);
                if (fieldSymbol.DeclaredAccessibility != Accessibility.Private)
                {
                    context.ReportDiagnostic(ReadWriteForEfficiencyNotPrivate, individualField, fieldSymbol.Name);
                }

                var invalidAssignments = fieldSymbol.ContainingType.DeclaringSyntaxReferences
                    .SelectMany(reference => reference.GetSyntax().DescendantNodes())
                    .OfType<AssignmentExpressionSyntax>()
                    .Where(n => IsAssignmentOutsideConstructor(n, fieldSymbol, model));
                foreach (var invalidAssignment in invalidAssignments)
                {
                    context.ReportDiagnostic(ReadWriteForEfficiencyAssignmentOutsideConstructor,
                        invalidAssignment, fieldSymbol.Name);
                }
            }
        }

        private static bool IsAssignmentOutsideConstructor(AssignmentExpressionSyntax assignment, ISymbol fieldSymbol, SemanticModel model)
        {
            var assignedSymbol = model.GetSymbolInfo(assignment.Left);
            if (assignedSymbol.Symbol != fieldSymbol)
            {
                return false;
            }
            // Method (or whatever) enclosing the assignment
            var enclosingSymbol = model.GetEnclosingSymbol(assignment.SpanStart) as IMethodSymbol;
            var isCtor = enclosingSymbol?.MethodKind == MethodKind.Constructor;
            return !isCtor;
        }
    }
}
