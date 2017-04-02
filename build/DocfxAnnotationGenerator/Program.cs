using DocfxYamlLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocfxAnnotationGenerator
{
    class Program
    {
        private readonly IEnumerable<Release> releases;
        private readonly string docfxRoot;

        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Arguments: <docfx root> <version1> <version2>");
                Console.WriteLine("The docfx root dir should contain the obj directory");
                return 1;
            }

            var releases = args.Skip(1)
                .Select(arg => Release.Load(Path.Combine(args[0], "obj", arg), arg))
                .ToList();

            var instance = new Program(releases, args[0]);
            instance.CreateDirectories();
            instance.WriteSinceAnnotations();
            return 0;
        }

        private Program(IEnumerable<Release> releases, string docfxRoot)
        {
            this.releases = releases;
            this.docfxRoot = docfxRoot;
        }

        private void CreateDirectories()
        {
            foreach (var release in releases)
            {
                Directory.CreateDirectory(GetOverwriteDirectory(release));
            }
        }

        private void WriteSinceAnnotations()
        {
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
    }
}
