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
            using (var stream = File.OpenRead(@"c:\users\jon\test\NodaTime.dll"))
            {
                var members = ReflectionMember.Load(stream).ToList();
                Console.WriteLine("[NotNull] return members:");
                foreach (var member in members.Where(m => m.NotNullReturn))
                {
                    Console.WriteLine($"  {member.DocfxUid}");
                }
                Console.WriteLine();
                Console.WriteLine("[NotNull] parameters:");
                foreach (var member in members.Where(m => m.NotNullParameters.Any()))
                {
                    Console.WriteLine($"  {member.DocfxUid}: {string.Join(", ", member.NotNullParameters)}");
                }
                /*
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
                PrintUids("UIDs only in Docfx", realUids.Except(cecilUids));*/
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