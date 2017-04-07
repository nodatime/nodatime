﻿using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DocfxAnnotationGenerator
{
    /// <summary>
    /// A member loaded via reflection.
    /// </summary>
    public class ReflectionMember
    {
        /// <summary>
        /// The UID Docfx would use for this member. See
        /// https://dotnet.github.io/docfx/spec/metadata_dotnet_spec.html
        /// </summary>
        public string DocfxUid { get; }

        // Only for methods and indexers...
        public IEnumerable<string> NotNullParameters { get; }

        // Methods, indexers, properties.
        public bool NotNullReturn { get; }

        public static IEnumerable<ReflectionMember> Load(Stream stream) =>
            ModuleDefinition.ReadModule(stream).Types.Where(t => t.IsPublic).SelectMany(GetMembers);

        private static IEnumerable<ReflectionMember> GetMembers(TypeDefinition type)
        {
            yield return new ReflectionMember(type);

            if (type.BaseType?.FullName == "System.MulticastDelegate")
            {
                yield break;
            }

            foreach (var property in type.Properties.Where(p => (p.GetMethod?.IsPublic ?? false) || (p.SetMethod?.IsPublic ?? false)))
            {
                yield return new ReflectionMember(property);
            }

            // TODO: Do we want protected methods?
            foreach (var method in type.Methods.Where(m => !m.IsGetter && !m.IsSetter && (m.IsPublic || m.IsFamily || IsExplicitInterfaceImplementation(m))))
            {
                yield return new ReflectionMember(method);
            }

            foreach (var field in type.Fields.Where(m => m.IsPublic && !m.IsSpecialName))
            {
                yield return new ReflectionMember(field);
            }

            foreach (var nestedMember in type.NestedTypes.Where(t => t.IsNestedPublic).SelectMany(GetMembers))
            {
                yield return nestedMember;
            }
        }

        // TODO: More specifically, is it an explicit interface implementation for a public interface?
        // (Doesn't seem to affect us right now...)
        private static bool IsExplicitInterfaceImplementation(MethodDefinition method) => method.Overrides.Count != 0;

        private ReflectionMember(TypeDefinition type)
        {
            DocfxUid = GetUid(type);
        }

        private ReflectionMember(PropertyDefinition property)
        {
            DocfxUid = $"{GetUid(property.DeclaringType)}.{property.Name}{GetParameterNames(property.Parameters)}";
        }

        private ReflectionMember(MethodDefinition method)
        {
            DocfxUid = GetUid(method);
        }

        private ReflectionMember(FieldDefinition field)
        {
            DocfxUid = $"{GetUid(field.DeclaringType)}.{field.Name}";
        }

        private string GetUid(MethodDefinition method)
        {
            if (method.Overrides.Count == 1)
            {
                var interfaceMethod = method.Overrides[0];
                var interfaceMethodNameUid = $"{GetUid(interfaceMethod.DeclaringType)}.{interfaceMethod.Name}".Replace('.', '#');
                return $"{GetUid(method.DeclaringType)}.{interfaceMethodNameUid}{GetParameterNames(method.Parameters)}";
            }
            string name = method.Name.Replace('.', '#');
            return $"{GetUid(method.DeclaringType)}.{name}{GetParameterNames(method.Parameters)}";
        }

        private string GetUid(TypeDefinition type)
        {
            return GetUid(type, false);
        }

        private string GetUid(TypeReference type)
        {
            return GetUid(type, true);
        }

        private string GetUid(TypeReference type, bool useTypeArgumentNames)
        {
            string name = type.FullName.Replace('/', '.');
            // TODO: Check whether this handles nested types involving generics
            switch (type)
            {
                case ByReferenceType brt: return $"{GetUid(brt.ElementType, useTypeArgumentNames)}@";
                case GenericParameter gp: return useTypeArgumentNames ? gp.Name : $"`{gp.Position}";
                case GenericInstanceType git: return $"{RemoveArity(name)}{GetGenericArgumentNames(git.GenericArguments, useTypeArgumentNames)}";
                default: return name;
            }
        }

        private string GetGenericArgumentNames(Collection<TypeReference> arguments, bool useTypeArgumentNames) =>
            arguments.Count == 0 ? "" : $"{{{string.Join(",", arguments.Select(arg => GetUid(arg, useTypeArgumentNames)))}}}";

        private string GetParameterNames(Collection<ParameterDefinition> parameters) =>
            parameters.Count == 0 ? "" : $"({string.Join(",", parameters.Select(p => GetUid(p.ParameterType, false)))})";

        private string RemoveArity(string name)
        {
            // TODO: Nested types, e.g. Foo`1.Bar or Foo`1.Bar`1
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        private ReflectionMember(string uid)
        {
            DocfxUid = uid;
        }
    }
}
