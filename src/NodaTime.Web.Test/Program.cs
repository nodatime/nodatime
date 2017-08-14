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
}