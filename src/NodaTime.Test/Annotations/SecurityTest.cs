using System.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Test.Annotations
{
    [TestFixture]
    public class SecurityTest
    {
        [Test]
        public void SecurityAttributesOnInterfaceImplementations()
        {
            var violations = new List<string>();
            foreach (var type in typeof(Instant).Assembly.GetTypes().Where(type => !type.IsInterface))
            {
                foreach (var iface in type.GetInterfaces())
                {
                    var map = type.GetInterfaceMap(iface);
                    var methodCount = map.InterfaceMethods.Length;
                    for (int i = 0; i < methodCount; i++)
                    {
                        if (map.InterfaceMethods[i].IsDefined(typeof(SecurityCriticalAttribute), false) &&
                            !map.TargetMethods[i].IsDefined(typeof(SecurityCriticalAttribute), false))
                        {
                            violations.Add(type.FullName + "/" + map.TargetMethods[i].Name);
                        }
                    }
                }
            }
            Assert.IsEmpty(violations);
        }
    }
}
