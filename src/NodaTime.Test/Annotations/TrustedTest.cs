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
    public class TrustedTest
    {
        [Test]
        public void MembersWithTrustedParametersAreNotPublic()
        {
            var types = typeof(Instant).GetTypeInfo().Assembly.DefinedTypes;
            var invalidMembers = types.SelectMany(t => t.DeclaredMembers)
                                      .Where(m => GetParameters(m).Any(p => p.IsDefined(typeof(TrustedAttribute), false)))
                                      .Where(InvalidForTrustedParameters);

            TestHelper.AssertNoFailures(invalidMembers, FormatMemberDebugName);
        }

        private static string FormatMemberDebugName(MemberInfo m) =>
            string.Format("{0}.{1}({2})",
                m.DeclaringType.Name,
                m.Name,
                string.Join(", ", GetParameters(m).Select(p => p.ParameterType)));

        private static bool InvalidForTrustedParameters(dynamic member) =>
            // We'll need to be more specific at some point, but this will do to start with...
            member.IsPublic && (member.DeclaringType.IsPublic || member.DeclaringType.IsNestedPublic);

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
