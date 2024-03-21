// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET6_0_OR_GREATER
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace NodaTime.Test;

public class TypeInitializationTest
{
    private static IEnumerable<Type> TypesToTest => typeof(Instant).Assembly.GetTypes()
        .Where(t => !t.IsGenericTypeDefinition)
        .Where(t => !t.IsEnum)
        .Where(t => t.BaseType != typeof(MulticastDelegate));

    [Test]
    [TestCaseSource(nameof(TypesToTest))]
    public void InitializeType(Type type)
    {
        var typeInContext = LoadTypeInContext(type);
        RuntimeHelpers.RunClassConstructor(typeInContext.TypeHandle);
    }

    [Test]
    [TestCaseSource(nameof(TypesToTest))]
    public void GetProperties(Type type)
    {
        var typeInContext = LoadTypeInContext(type);
        RuntimeHelpers.RunClassConstructor(typeInContext.TypeHandle);

        foreach (var property in typeInContext.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.CanRead))
        {
            property.GetValue(null);
        }
    }

    private static Type LoadTypeInContext(Type type)
    {
        var location = type.Assembly.Location;
        var context = new AssemblyLoadContext($"Context for {type.FullName}");
        var contextAssembly = context.LoadFromAssemblyPath(location);
        return contextAssembly.GetType(type.FullName!) ?? throw new Exception($"Couldn't load type {type}");
    }
}
#endif
