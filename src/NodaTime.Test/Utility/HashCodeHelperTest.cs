using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility;

// Most aspects are effectively tested indirectly. This test is just to add coverage.
public class HashCodeHelperTest
{
    [Test]
    public void Hash_Nulls()
    {
        // The constants in HashCodeHelper aren't internal as we don't want to expose them in the assembly.
        // But really, we're never going to change them either.
        Assert.AreEqual(17 * 37, HashCodeHelper.Initialize().Hash<string?>(null).Value);
        Assert.AreEqual(17 * 37 * 37, HashCodeHelper.Hash<string?, string?>(null, null));
        Assert.AreEqual(17 * 37 * 37 * 37, HashCodeHelper.Hash<string?, string?, string?>(null, null, null));
    }
}
