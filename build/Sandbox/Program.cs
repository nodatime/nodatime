using DocfxAnnotationGenerator;
using DocfxYamlLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = File.OpenRead(@"c:\users\jon\test\Projects\nodatime\build\tmp\docfx\build\src\NodaTime.Serialization.JsonNet\bin\Debug\net45\NodaTime.Serialization.JsonNet.dll"))
            {
                var cecilUids = ReflectionMember.Load(stream).Select(m => m.DocfxUid).ToList();
                var release = Release.Load(@"c:\users\jon\test\projects\nodatime\build\tmp\docfx\obj\2.0.x", "2.0.x");
                var realUids = release.Members
                    .Where(m => m.Type != DocfxMember.TypeKind.Namespace)
                    .Where(m => m.Uid.StartsWith("NodaTime.Serialization"))
                    .Where(m => !m.Uid.StartsWith("NodaTime.Testing"))
                    .Select(m => m.Uid).ToList();

                Console.WriteLine($"UIDs from Cecil: {cecilUids.Count}");
                Console.WriteLine($"UIDs from Docfx: {realUids.Count}");
                Console.WriteLine();
                PrintUids("UIDs only in Cecil", cecilUids.Except(realUids));
                Console.WriteLine();
                PrintUids("UIDs only in Docfx", realUids.Except(cecilUids));
            }
        }

        static void PrintUids(string title, IEnumerable<string> uids)
        {
            Console.WriteLine($"{title} ({uids.Count()})");
            foreach (var uid in uids)
            {
                Console.WriteLine(uid);
            }
        }
    }
}