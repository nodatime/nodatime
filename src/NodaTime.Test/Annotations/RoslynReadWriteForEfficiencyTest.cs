// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NodaTime.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Annotations
{
    [TestFixture]
    public class RoslynReadWriteForEfficiencyTest
    {
        private static IEnumerable<IFieldSymbol> GetFieldsWithAttribute()
        {
            var compilation = RoslynHelper.Compilation;
            return RoslynHelper.GetSyntaxNodes<FieldDeclarationSyntax>()
                .SelectMany(fds => fds.Declaration.Variables)
                .Select(vds => (IFieldSymbol) vds.GetDeclaredSymbol())
                .Where(field => field.HasAttribute(nameof(ReadWriteForEfficiencyAttribute)));
        }

        [Test]
        public void AttributeOnlyAppliedToWritableFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => field.IsReadOnly)
                .ToList();
            Assert.IsEmpty(badFields);
        }

        [Test]
        public void AttributeOnlyAppliedToValueTypeFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => !field.Type.IsValueType)
                .ToList();
            Assert.IsEmpty(badFields);
        }

        [Test]
        public void AttributeOnlyAppliedToPrivateFields()
        {
            var badFields = GetFieldsWithAttribute()
                .Where(field => field.DeclaredAccessibility != Accessibility.Private)
                .ToList();
            Assert.IsEmpty(badFields);
        }

        [Test]
        public void NoAssignmentsOutsideConstructors()
        {
            var badAssignments =
                from aes in RoslynHelper.GetSyntaxNodes<AssignmentExpressionSyntax>()
                let target = aes.Left.GetSymbol()
                where target.HasAttribute(nameof(ReadWriteForEfficiencyAttribute))
                where (aes.GetEnclosingSymbol() as IMethodSymbol).MethodKind != MethodKind.Constructor
                select aes;
            Assert.IsEmpty(badAssignments.ToList());
        }
    }
}
