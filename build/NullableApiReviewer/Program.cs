using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NullableApiReviewer
{
    /// <summary>
    /// Tool to report every public member in an assembly that contains a NullableAttribute annotation,
    /// in order to aid code review.
    /// </summary>
    class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please specify an assembly to analyze");
                return;
            }
            var asm = Assembly.LoadFrom(args[0]);
            foreach (var type in asm.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => t.BaseType != typeof(MulticastDelegate))
                .OrderBy(t => t.Namespace)
                .ThenBy(t => t.FullName))
            {
                ReportType(type.GetTypeInfo());
            }
        }

        private static void ReportType(TypeInfo type)
        {
            List<string> entries = new string?[0]
                .Concat(type.DeclaredConstructors.Select(GetConstructorEntry))
                .Concat(type.DeclaredProperties.Select(GetPropertyEntry))
                .Concat(type.GetMethods().Select(GetMethodEntry))
                .Where(entry => entry != null)
                .Select(e => e!)
                .ToList();
            if (entries.Count != 0)
            {
                Console.WriteLine($"{type.FullName}:");
                foreach (var entry in entries)
                {
                    Console.WriteLine($"  {entry}");
                }
                Console.WriteLine();
            }
        }

        private static string? GetMethodEntry(MethodInfo method)
        {
            if (!method.IsPublic || method.IsSpecialName)
            {
                return null;
            }
            var nullability = false;
            var builder = new StringBuilder();
            nullability |= AppendType(builder, method.ReturnType, method.ReturnParameter.GetCustomAttributesData());
            builder.Append(" ").Append(method.Name).Append("(");
            nullability |= AppendParameters(builder, method.GetParameters());
            builder.Append(")");
            return nullability ? builder.ToString() : null;
        }

        private static string? GetPropertyEntry(PropertyInfo property)
        {
            if (property.GetMethod?.IsPublic != true)
            {
                return null;
            }
            var nullability = false;
            var builder = new StringBuilder();
            nullability |= AppendType(builder, property.PropertyType, property.GetCustomAttributesData());
            builder.Append(" ").Append(property.Name);
            var parameters = property.GetIndexParameters();
            if (parameters.Length != 0)
            {
                builder.Append("[");
                nullability |= AppendParameters(builder, parameters);
                builder.Append("]");
            }
            return nullability ? builder.ToString() : null;
        }

        private static string? GetConstructorEntry(ConstructorInfo ctor)
        {
            if (!ctor.IsPublic)
            {
                return null;
            }
            var nullability = false;
            var builder = new StringBuilder(ctor.DeclaringType!.Name);
            builder.Append("(");
            nullability |= AppendParameters(builder, ctor.GetParameters());
            builder.Append(")");
            return nullability ? builder.ToString() : null;
        }

        private static bool AppendType(StringBuilder builder, Type type, IEnumerable<CustomAttributeData> data)
        {
            var nullable = data
                .Where(d => d.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute")
                .SingleOrDefault();
            if (nullable == null)
            {
                builder.Append(GetNameOrAlias(type));
                return false;
            }
            var value = nullable.ConstructorArguments[0].Value;
            IEnumerable<byte> values = value switch
            {
                // A single value means "apply to everything in the type", e.g. 1 for Dictionary<string, string>, 2 for Dictionary<string?, string?>?
                // The simplest way of representing this is just a very long string of bytes all with the same value. 
                byte b => Enumerable.Repeat(b, 1_000_000),
                // Not terribly robust, but...
                IEnumerable<CustomAttributeTypedArgument> arguments => arguments.Select(arg => (byte)arg.Value!).ToArray(),
                { } => throw new InvalidOperationException($"Unexpected argument type: {value.GetType()}"),
                _ => throw new InvalidOperationException("Unexpected null argument")
            };
            var iterator = values.GetEnumerator();
            AppendTypeWithNullability(builder, iterator, type);
            return true;
        }

        private static void AppendTypeWithNullability(StringBuilder builder, IEnumerator<byte> nullableBytes, Type type)
        {
            bool appendQuestionMark = !type.IsValueType && IsNullable();
            if (type.IsGenericType)
            {
                builder.Append(type.Name.Split('`')[0]);
                builder.Append("<");
                bool first = true;
                foreach (var typeArg in type.GetGenericArguments())
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }
                    first = false;
                    AppendTypeWithNullability(builder, nullableBytes, typeArg);
                }
                builder.Append(">");
            }
            else
            {
                builder.Append(GetNameOrAlias(type));
            }
            if (appendQuestionMark)
            {
                builder.Append("?");
            }

            bool IsNullable()
            {
                if (!nullableBytes.MoveNext())
                {
                    throw new InvalidOperationException("Not enough nullability information");
                }
                return nullableBytes.Current == 2;
            }
        }

        private static bool AppendParameters(StringBuilder builder, IEnumerable<ParameterInfo> parameters)
        {
            bool first = true;
            bool nullability = false;
            foreach (var parameter in parameters)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;
                var parameterType = parameter.ParameterType;
                if (parameter.IsOut)
                {
                    builder.Append("out ");
                    parameterType = parameterType.GetElementType()!;
                }
                else if (parameterType.IsByRef)
                {
                    builder.Append("ref ");
                    parameterType = parameterType.GetElementType()!;
                }
                nullability |= AppendType(builder, parameterType, parameter.GetCustomAttributesData());
                builder.Append(" ").Append(parameter.Name);
            }
            return nullability;
        }

        private static readonly IDictionary<Type, string> aliases = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(void), "void" },
            { typeof(object), "object" },
            { typeof(string), "string" }
        };

        private static string GetNameOrAlias(Type type) =>
            aliases.TryGetValue(type, out var name) ? name : type.Name;
    }
}
