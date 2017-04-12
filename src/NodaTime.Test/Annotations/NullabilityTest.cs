// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Annotations;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Annotations
{
    public class NullabilityTest
    {
        // TODO: None of this checks for explicit interface implementation. We really need something
        // along the lines of "Is this going to be visible somehow?"

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
                .Where(m => m.Name != "ToString" && m.Name != "BeginInvoke") // Skip commonly-understood methods (and async delegate methods)
                .Where(m => !IsPropertyGetterOrSetter(m))
                .Where(m => m.IsPublic)
                .Where(m => CanBeReferenceType(m.ReturnType) == true)
                .Where(m => !IsAnnotated(m));

            var unannotatedMembers = properties.Concat<MemberInfo>(methods);
            TestHelper.AssertNoFailures(unannotatedMembers, m => $"{m.DeclaringType.Name}.{m.Name}");
        }

        [Test]
        public void ReturnAnnotationsAreValid()
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

            var badMembers = properties.Concat<MemberInfo>(methods);
            TestHelper.AssertNoFailures(badMembers, m => $"{m.DeclaringType.Name}.{m.Name}");
        }

        [Test]
        public void ParameterAnnotations()
        {
            // Skip commonly-understood methods (and async delegate methods)
            string[] wellKnownMethods =
            {
                "ToString",
                "BeginInvoke",
                "EndInvoke",
                "Equals",
                "op_Equality",
                "op_Inequality"
            };
            var types = typeof(Instant).GetTypeInfo().Assembly
                .DefinedTypes
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .OrderBy(t => t.Name)
                .ToList();
            var propertyParameters = types
                .SelectMany(t => t.DeclaredProperties)
                .Where(p => p.GetMethod?.IsPublic ?? false)
                .SelectMany(p => p.GetMethod.GetParameters());
            var methodParameters = types
                .SelectMany(t => t.DeclaredMethods)
                .Where(m => !wellKnownMethods.Contains(m.Name))
                .Where(m => !IsPropertyGetterOrSetter(m))
                .Where(m => m.IsPublic)
                .SelectMany(m => m.GetParameters());

            var badParameters = propertyParameters
                .Concat(methodParameters)
                .Where(p => CanBeReferenceType(p.ParameterType) == true)
                .Where(p => !IsAnnotated(p));

            TestHelper.AssertNoFailures(badParameters, p => $"{p.Member.DeclaringType.Name}.{p.Member.Name}({p.Name})");
        }

        [Test]
        public void ParameterAnnotationsAreValid()
        {
            var types = typeof(Instant).GetTypeInfo().Assembly
                .DefinedTypes
                .OrderBy(t => t.Name)
                .ToList();
            var propertyParameters = types
                .SelectMany(t => t.DeclaredProperties)
                .SelectMany(p => p.GetMethod.GetParameters());
            var methodParameters = types
                .SelectMany(t => t.DeclaredMethods)
                .SelectMany(m => m.GetParameters());

            var badParameters = propertyParameters
                .Concat(methodParameters)
                .Where(IsAnnotated)
                .Where(p => CanBeReferenceType(p.ParameterType) == false);

            TestHelper.AssertNoFailures(badParameters, p => $"{p.Member.DeclaringType.Name}.{p.Member.Name}({p.Name})");
        }

        private static bool IsPropertyGetterOrSetter(MethodInfo method) =>
            method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"));

        private static bool IsAnnotated(MemberInfo member) =>
            member.IsDefined(typeof(NotNullAttribute)) || member.IsDefined(typeof(CanBeNullAttribute));

        private static bool IsAnnotated(ParameterInfo member) =>
            member.IsDefined(typeof(NotNullAttribute)) ||
            member.IsDefined(typeof(CanBeNullAttribute)) ||
            member.IsDefined(typeof(SpecialNullHandlingAttribute));

        private static bool? CanBeReferenceType(Type type)
        {
            var ti = type.GetTypeInfo();
            if (ti.IsValueType)
            {
                return false;
            }
            if (ti.IsByRef)
            {
                return CanBeReferenceType(ti.GetElementType());
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
