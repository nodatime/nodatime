using DocfxYamlLoader;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace DocfxAnnotationGenerator
{
    class Program
    {
        private static readonly string[] packages = { "NodaTime", "NodaTime.Testing", "NodaTime.Serialization.JsonNet" };
        private static readonly string[] unstableFrameworks = { "net45", "netstandard1.3" };

        private readonly IEnumerable<Release> releases;
        private readonly Dictionary<string, List<BuildAssembly>> reflectionDataByVersion;
        private readonly string docfxRoot;
        private readonly string packagesDir;
        private readonly string srcRoot;

        private static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments: <docfx root> <packages dir> <src root> <version1> <version2> ...");
                Console.WriteLine("The docfx root dir should contain the obj directory");
                Console.WriteLine("The packages dir should contain the nuget packages (as NodaTime-1.0.x.nupkg etc)");
                Console.WriteLine("The src root dir should contain the 'unstable' code");
                return 1;
            }


            var instance = new Program(args.Skip(3), args[0], args[1], args[2]);
            instance.CreateDirectories();
            instance.WriteSinceAnnotations();
            instance.WriteAvailabilityAnnotations();
            instance.ModifyYamlFiles();
            return 0;
        }

        private Program(IEnumerable<string> versions, string docfxRoot, string packagesDir, string srcRoot)
        {
            this.docfxRoot = docfxRoot;
            this.packagesDir = packagesDir;
            this.srcRoot = srcRoot;
            Console.WriteLine("Loading docfx metadata");
            releases = versions.Select(v => Release.Load(Path.Combine(docfxRoot, "obj", v), v)).ToList();
            Console.WriteLine("Loading assemblies");
            reflectionDataByVersion = versions.ToDictionary(v => v, v => LoadAssemblies(v, packagesDir, srcRoot).ToList());
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
                        if (!uidsToVersions.TryGetValue(uid, out string version))
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

        private void ModifyYamlFiles()
        {
            // We don't want to load and save the YAML files over and over again, so we perform
            // potentially-multiple mutations, then resave. We only load documents as and when we need to.
            foreach (var release in releases)
            {
                Console.WriteLine($"Modifying YAML files for {release.Version}");
                var files = new Dictionary<string, YamlStream>();

                AnnotateNotNullParameters(release, files);
                AnnotateNotNullReturns(release, files);

                foreach (var pair in files)
                {
                    using (var writer = File.CreateText(pair.Key))
                    {
                        writer.WriteLine("### YamlMime:ManagedReference");
                        pair.Value.Save(writer, false);
                    }
                }
            }
        }

        private void AnnotateNotNullParameters(Release release, Dictionary<string, YamlStream> files)
        {
            var members = reflectionDataByVersion[release.Version]
                .SelectMany(asm => asm.Members)
                .Where(m => m.NotNullParameters.Any());

            foreach (var member in members)
            {
                var document = FindDocument(release, files, member.DocfxUid);
                var node = FindChildByUid(document, "items", member.DocfxUid);

                if (!node.Children.TryGetValue("exceptions", out YamlNode exceptions))
                {
                    exceptions = new YamlSequenceNode();
                    node.Add("exceptions", exceptions);
                }
                YamlSequenceNode exceptionsSequence = (YamlSequenceNode) exceptions;
                var currentArgumentNullException = exceptionsSequence.Children
                    .Cast<YamlMappingNode>()
                    .SingleOrDefault(e => ((YamlScalarNode)e.Children["type"]).Value == "System.ArgumentNullException");
                if (currentArgumentNullException != null)
                {
                    exceptionsSequence.Children.Remove(currentArgumentNullException);
                }

                var names = member.NotNullParameters.ToList();
                string message;
                
                if (names.Count == 1)
                {
                    message = $"{ParamRef(names[0])} is null.";
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < names.Count - 2; i++)
                    {
                        builder.Append($"{ParamRef(names[i])}, ");
                    }
                    builder.Append($"{ParamRef(names[names.Count - 2])} or {ParamRef(names.Last())} is null");
                    message = builder.ToString();
                }
                exceptionsSequence.Children.Add(new YamlMappingNode
                {
                    { "type", "System.ArgumentNullException" },
                    { "commentId", "T:System.ArgumentNullException" },
                    { "description", message }
                });

                // Make sure the reference to ArgumentNullException is present
                var reference = FindChildByUid(document, "references", "System.ArgumentNullException");
                if (reference == null)
                {
                    ((YamlSequenceNode)document.Children["references"]).Add(new YamlMappingNode
                    {
                        { "uid", "System.ArgumentNullException" },
                        { "commentId", "T:System.ArgumentNullException" },
                        { "parent", "System" },
                        { "isExternal", "true" },
                        { "name", "ArgumentNullException" },
                        { "nameWithType", "ArgumentNullException" },
                        { "fullName", "System.ArgumentNullException" }
                    });
                }
            }

            string ParamRef(string name) => $"<span class=\"paramref\">{name}</span>"; ;
        }

        private void AnnotateNotNullReturns(Release release, Dictionary<string, YamlStream> files)
        {
            var errors = new List<string>();
            var members = reflectionDataByVersion[release.Version]
                .SelectMany(asm => asm.Members)
                .Where(m => m.NotNullReturn)
                .DistinctBy(m => m.DocfxUid);

            foreach (var member in members)
            {
                var document = FindDocument(release, files, member.DocfxUid);
                var node = FindChildByUid(document, "items", member.DocfxUid);

                var returnElement = (YamlMappingNode) node["syntax"]["return"];
                if (!returnElement.Children.ContainsKey("description"))
                {
                    errors.Add(member.DocfxUid);
                    continue;
                }
                var description = (YamlScalarNode) returnElement["description"];
                var suffix = " (The value returned is never null.)";
                if (!description.Value.EndsWith(suffix))
                {
                    description.Value += suffix;
                }
            }
            if (errors.Count != 0)
            {
                throw new Exception($"UIDs with no return description:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
            }
        }

        private YamlMappingNode FindDocument(Release release, Dictionary<string, YamlStream> files, string uid)
        {
            var docfxMember = release.MembersByUid[uid];
            string file = docfxMember.YamlFile;
            if (!files.TryGetValue(file, out YamlStream document))
            {
                document = LoadYamlFile(file);
                files[file] = document;
            }
            return (YamlMappingNode)document.Documents[0].RootNode;
        }

        private YamlMappingNode FindChildByUid(YamlMappingNode parent, string sequenceName, string uid)
        {
            var items = (YamlSequenceNode) parent[sequenceName];
            return items.Cast<YamlMappingNode>().SingleOrDefault(node => ((YamlScalarNode)node["uid"]).Value == uid);
        }
        
        private static YamlStream LoadYamlFile(string file)
        {
            var stream = new YamlStream();
            using (var reader = File.OpenText(file))
            {
                stream.Load(reader);
            }
            return stream;
        }

        private string GetOverwriteDirectory(Release release)
            => Path.Combine(docfxRoot, "obj", release.Version, "overwrite");

        private static IEnumerable<BuildAssembly> LoadAssemblies(string version, string packagesDir, string srcRoot)
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
                    .Select(p => Path.Combine(packagesDir, $"{p}-{version}.nupkg"))
                    .Where(file => File.Exists(file))
                    .Select(file => NuGetPackage.Load(file))
                    .SelectMany(pkg => pkg.Assemblies);
            }
        }
    }
}
