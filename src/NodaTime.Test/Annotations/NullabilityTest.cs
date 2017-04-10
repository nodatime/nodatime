// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Annotations
{
    public class NullabilityTest
    {
        [Test]
        public void ReturnAnnotations()
        {
            // Any method, property, indexer or operator that returns a reference type should
            // be annotated with [CanBeNull] or [NotNull]
            var types = typeof(Instant).GetTypeInfo().Assembly
                .DefinedTypes
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .OrderBy(t => t.Name)
                .ToList();
            var properties = types
                .SelectMany(t => t.DeclaredProperties)
                .Where(p => (p.GetMethod?.IsPublic ?? false) || (p.SetMethod?.IsPublic ?? false))
                .Where(p => CanBeReferenceType(p.PropertyType) == true)
                .Where(m => !IsAnnotated(m));
            var methods = types
                .SelectMany(t => t.DeclaredMethods)
                .Where(m => m.Name != "ToString" && m.Name != "BeginInvoke")
                .Where(m => !IsPropertyGetterOrSetter(m))
                .Where(m => m.IsPublic)
                .Where(m => CanBeReferenceType(m.ReturnType) == true)
                .Where(m => !IsAnnotated(m));

            var unannotatedMembers = properties.Concat<MemberInfo>(methods).ToList();
                                        
            Assert.IsEmpty(unannotatedMembers, $"Unannotated members ({unannotatedMembers.Count}): " + string.Join(", ", unannotatedMembers.Select(m => $"{m.DeclaringType.Name}.{m.Name}")));
        }

        [Test]
        public void AnnotationsAreForReturnTypesNotAppliedToValueTypes()
        {
            var types = typeof(Instant).GetTypeInfo().Assembly
                .DefinedTypes
                .OrderBy(t => t.Name)
                .ToList();
            var properties = types
                .SelectMany(t => t.DeclaredProperties)
                .Where(IsAnnotated)
                .Where(p => CanBeReferenceType(p.PropertyType) == false);
            var methods = types
                .SelectMany(t => t.DeclaredMethods)
                .Where(IsAnnotated)
                .Where(m => CanBeReferenceType(m.ReturnType) == false);

            var badMembers = properties.Concat<MemberInfo>(methods).ToList();

            Assert.IsEmpty(badMembers, $"Inappropriately annotated members ({badMembers.Count}): " + string.Join(", ", badMembers.Select(m => $"{m.DeclaringType.Name}.{m.Name}")));
        }

        private static bool IsPropertyGetterOrSetter(MethodInfo method) =>
            method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("end_"));

        private static bool IsAnnotated(MemberInfo member) =>
            member.IsDefined(typeof(NotNullAttribute)) || member.IsDefined(typeof(CanBeNullAttribute));

        private static bool? CanBeReferenceType(Type type)
        {
            var ti = type.GetTypeInfo();
            if (ti.IsValueType)
            {
                return false;
            }
            if (ti.IsGenericParameter)
            {
                if (ti.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                {
                    return false;
                }
                if (ti.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                {
                    return true;
                }
                return null;
            }
            if (ti.IsClass || ti.IsInterface)
            {
                return true;
            }
            throw new InvalidOperationException($"Need to handle {type}");
        }
    }
}
