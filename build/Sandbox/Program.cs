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
            using (var stream = File.OpenRead(@"c:\users\jon\test\projects\nodatime\build\tmp\docfx\unstable\src\NodaTime\bin\Debug\net45\NodaTime.dll"))
            {
                var members = ReflectionMember.Load(stream).ToList();
                var cecilUids = members.Select(m => m.DocfxUid).ToList();
                var release = Release.Load(@"c:\users\jon\test\projects\nodatime\build\tmp\docfx\obj\unstable", "unstable");
                var realUids = release.Members
                    .Where(m => m.Type != DocfxMember.TypeKind.Namespace)
                    .Where(m => !m.FullName.StartsWith("NodaTime.Testing"))
                    .Where(m => !m.FullName.StartsWith("NodaTime.Serialization"))
                    .Select(m => m.Uid)
                    .ToList();

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