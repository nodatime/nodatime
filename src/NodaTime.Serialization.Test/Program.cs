using NUnit.Common;
using NUnitLite;
using System;
using System.Reflection;

namespace NodaTime.Serialization.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if PCL
            var writer = new ExtendedTextWrapper(Console.Out);
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, writer, Console.In);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
