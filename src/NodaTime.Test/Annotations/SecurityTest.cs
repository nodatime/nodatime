// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;

namespace NodaTime.Test.Annotations
{
    public class SecurityTest
    {
        [Test]
        public void SecurityAttributesOnInterfaceImplementations()
        {
            var violations = new List<string>();

            foreach (var type in typeof(Instant).GetTypeInfo().Assembly.DefinedTypes.Where(type => !type.IsInterface))
            {
                foreach (var iface in type.ImplementedInterfaces)
                {
                    var map = type.GetRuntimeInterfaceMap(iface);
                    var methodCount = map.InterfaceMethods.Length;
                    for (int i = 0; i < methodCount; i++)
                    {
                        if (map.TargetMethods[i].DeclaringType.GetTypeInfo() == type &&
                            map.InterfaceMethods[i].IsDefined(typeof(SecurityCriticalAttribute), false) &&
                            !map.TargetMethods[i].IsDefined(typeof(SecurityCriticalAttribute), false))
                        {
                            violations.Add(type.FullName + "/" + map.TargetMethods[i].Name);
                        }
                    }
                }
            }
            TestHelper.AssertNoFailures(violations, v => v);
        }
    }
}
