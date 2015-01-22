// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Security;

namespace NodaTime.CodeDiagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SecurityCriticalDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        // TODO: Check that in every interface implementation, if the
        // interface method has SecurityCriticalAttribute applied, the
        // implementation should too.

        internal static readonly DiagnosticDescriptor SecurityCriticalMethodImplementationsShouldBeSecurityCritical = Helpers.CreateWarning(
            "Any implementation of an interface method marked SecurityCritical should also be marked SecurityCritical",
            "Any implementation of an interface method marked SecurityCritical should also be marked SecurityCritical",
            Category.Correctness);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(SecurityCriticalMethodImplementationsShouldBeSecurityCritical);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterNodaTimeSyntaxNodeAction(CheckOriginalDefinitions, SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration);
        }

        // TODO: Ask the Roslyn team why this doesn't work. For some reason, ISerializable.GetObjectData doesn't
        // know about its SecurityCritical attribute.

        private void CheckOriginalDefinitions(SyntaxNodeAnalysisContext context)
        {
            var typeSyntax = (TypeDeclarationSyntax)context.Node;
            var model = context.SemanticModel;
            var typeSymbol = model.GetDeclaredSymbol(typeSyntax);
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                foreach (var member in iface.GetMembers().Where(m => m.HasAttribute(nameof(SecurityCriticalAttribute))))
                {
                    var implementation = typeSymbol.FindImplementationForInterfaceMember(member);
                    if (!implementation.HasAttribute(nameof(SecurityCriticalAttribute)))
                    {
                        context.ReportDiagnostic(SecurityCriticalMethodImplementationsShouldBeSecurityCritical, implementation);
                    }
                }
            }
        }
    }
}
