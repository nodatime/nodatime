using DocfxYamlLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocfxAnnotationGenerator
{
    class Program
    {
        private static readonly string[] packages = { "NodaTime", "NodaTime.Testing", "NodaTime.Serialization.JsonNet" };
        private static readonly string[] unstableFrameworks = { "net45", "netstandard1.3" };

        private readonly IEnumerable<Release> releases;
        private readonly Dictionary<string, List<BuildAssembly>> reflectionDataByVersion;
        private readonly string docfxRoot;
        private readonly string historyRoot;
        private readonly string srcRoot;

        private static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments: <docfx root> <history root> <src root> <version1> <version2> ...");
                Console.WriteLine("The docfx root dir should contain the obj directory");
                Console.WriteLine("The history root dir should contain the nuget packages (as NodaTime-1.0.x.nupkg etc)");
                Console.WriteLine("The src root dir should contain the 'unstable' code");
                return 1;
            }


            var instance = new Program(args.Skip(3), args[0], args[1], args[2]);
            instance.CreateDirectories();
            instance.WriteSinceAnnotations();
            instance.WriteAvailabilityAnnotations();
            return 0;
        }

        private Program(IEnumerable<string> versions, string docfxRoot, string historyRoot, string srcRoot)
        {
            this.docfxRoot = docfxRoot;
            this.historyRoot = historyRoot;
            this.srcRoot = srcRoot;
            Console.WriteLine("Loading docfx metadata");
            releases = versions.Select(v => Release.Load(Path.Combine(docfxRoot, "obj", v), v)).ToList();
            Console.WriteLine("Loading assemblies");
            reflectionDataByVersion = versions.ToDictionary(v => v, v => LoadAssemblies(v, historyRoot, srcRoot).ToList());
        }

        private void CreateDirectories()
        {
            foreach (var release in releases)
            {
                Directory.CreateDirectory(GetOverwriteDirectory(release));
            }
        }

        private void WriteAvailabilityAnnotations()
        {
            Console.WriteLine("Generating 'availability' annotations");
            foreach (var release in releases)
            {
                var assemblies = reflectionDataByVersion[release.Version];
                // I'm sure there's a cleaner way of doing this, but it should work...
                var frameworksByUid = 
                    assemblies.SelectMany(asm => asm.Members.Select(mem => new { Uid = mem.DocfxUid, Framework = asm.TargetFramework }))
                              .ToLookup(pair => pair.Uid, pair => pair.Framework);
                var file = Path.Combine(GetOverwriteDirectory(release), "availability.md");
                using (var writer = File.CreateText(file))
                {
                    foreach (var uid in release.Members.Where(m => m.Type != DocfxMember.TypeKind.Namespace).Select(m => m.Uid))
                    {
                        string availability = string.Join(", ", frameworksByUid[uid].OrderBy(f => f));
                        if (availability == "")
                        {
                            // We can refine this later...
                            throw new Exception($"No reflection metadata for {uid}");
                        }
                        writer.WriteLine("---");
                        writer.WriteLine($"uid: {uid}");
                        writer.WriteLine($"availability: '{availability}'");
                        writer.WriteLine("---");
                        writer.WriteLine();
                    }
                }
            }
        }

        private void WriteSinceAnnotations()
        {
            Console.WriteLine("Generating 'since' annotations");
            var uidsToVersions = new Dictionary<string, string>();

            foreach (Release release in releases)
            {
                var file = Path.Combine(GetOverwriteDirectory(release), "since.md");
                using (var writer = File.CreateText(file))
                {
                    foreach (var uid in release.Members.Select(m => m.Uid))
                    {
                        string version;
                        if (!uidsToVersions.TryGetValue(uid, out version))
                        {
                            version = release.Version;
                            uidsToVersions[uid] = version;
                        }
                        writer.WriteLine("---");
                        writer.WriteLine($"uid: {uid}");
                        writer.WriteLine($"since: '{version}'");
                        writer.WriteLine("---");
                        writer.WriteLine();
                    }
                }
                // Effectively clear out any versions removed by this release.
                // (e.g. if a member is in 1.3.x, not in 2.0.x, then in 2.1.x,
                // we want the 2.1.x docs to say it's since 2.1.x).
                uidsToVersions = release.Members
                    .Select(m => m.Uid)
                    .ToDictionary(uid => uid, uid => uidsToVersions[uid]);
            }
        }

        private string GetOverwriteDirectory(Release release)
            => Path.Combine(docfxRoot, "obj", release.Version, "overwrite");

        private static IEnumerable<BuildAssembly> LoadAssemblies(string version, string historyRoot, string srcRoot)
        {
            if (version == "unstable")
            {
                return from package in packages
                       from framework in unstableFrameworks
                       let file = $"{package}.dll"
                       select BuildAssembly.Load(framework, file, Path.Combine(srcRoot, package, "bin", "Debug", framework, file));
            }
            else
            {
                return packages
                    .Select(p => Path.Combine(historyRoot, $"{p}-{version}.nupkg"))
                    .Where(file => File.Exists(file))
                    .Select(file => NuGetPackage.Load(file))
                    .SelectMany(pkg => pkg.Assemblies);
            }
        }


    }
}
