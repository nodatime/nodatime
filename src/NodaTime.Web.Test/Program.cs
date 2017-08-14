using NUnit.Framework;
using NUnitLite;
using System.Reflection;

namespace NodaTime.Web.Test
{
    class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args);
        }
    }

    // Fake test just so that there's something to test in the first commit.
    public class DummyTest
    {
        [Test]
        public void Dummy()
        {
        }
    }
}