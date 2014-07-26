// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodaTime.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Annotations
{
    [TestFixture]
    public class TrustedTest
    {
        [Test]
        public void MembersWithTrustedParametersAreNotPublic()
        {
            var types = typeof(Instant).Assembly.GetTypes();
            var invalidMembers = types.SelectMany(t => t.GetMembers())
                                      .Where(m => GetParameters(m).Any(p => p.IsDefined(typeof(TrustedAttribute), false)))
                                      .Where(InvalidForTrustedParameters)
                                      .ToList();

            Assert.IsEmpty(invalidMembers, "Invalid members: " + string.Join(", ", invalidMembers.Select(MemberDebugName)));
        }

        private static string MemberDebugName(MemberInfo m)
        {
            return string.Format("{0}.{1}({2})", m.ReflectedType.Name, m.Name,
                string.Join(", ", GetParameters(m).Select(p => p.ParameterType)));
        }

        private static bool InvalidForTrustedParameters(dynamic member)
        {
            // We'll need to be more specific at some point, but this will do to start with...
            return member.IsPublic && member.DeclaringType.IsPublic;
        }

        private static IEnumerable<ParameterInfo> GetParameters(MemberInfo member)
        {
            if (member is MethodInfo || member is ConstructorInfo)
            {
                return ((dynamic) member).GetParameters();
            }
            if (member is PropertyInfo)
            {
                return ((PropertyInfo) member).GetIndexParameters();
            }
            return Enumerable.Empty<ParameterInfo>();
        }
    }
}
