using NUnit.Common;
using NUnitLite;
using System;
using System.Reflection;

namespace NodaTime.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var writer = new ExtendedTextWrapper(Console.Out);
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, writer, Console.In);
        }
    }
}
