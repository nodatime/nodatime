using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility;

public class ReferenceEqualityComparerTest
{
    [Test]
    public void Equals()
    {
        var instance = new ReferenceEqualityComparer<string>();
        var s1 = "test";
        var s2 = "test";
        var s3 = new string(s1.ToArray());
        Assert.True(instance.Equals(s1, s2));
        Assert.False(instance.Equals(s1, s3));
        Assert.True(instance.Equals(null, null));
    }

    [Test]
    public void TestGetHashCode()
    {
        var instance = new ReferenceEqualityComparer<string>();
        var s1 = "test";
        var s2 = "test";
        var s3 = new string(s1.ToArray());
        Assert.AreEqual(instance.GetHashCode(s1), instance.GetHashCode(s2));
        Assert.AreNotEqual(instance.GetHashCode(s1), instance.GetHashCode(s3));
        Assert.AreEqual(0, instance.GetHashCode(null!));
    }
}
