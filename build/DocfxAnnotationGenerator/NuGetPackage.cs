using SharpCompress.Archives.Zip;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml.Linq;

namespace DocfxAnnotationGenerator
{
    public sealed class NuGetPackage
    {
        public string File { get; }
        public XDocument Manifest { get; }
        public IImmutableList<BuildAssembly> Assemblies { get; }

        private NuGetPackage(string file, XDocument manifest, IEnumerable<BuildAssembly> assemblies)
        {
            File = file;
            Manifest = manifest;
            Assemblies = assemblies.ToImmutableList();
        }

        public static NuGetPackage Load(string file)
        {
            XDocument manifest = null;
            var assemblies = new List<BuildAssembly>();

            using (var zip = ZipArchive.Open(file))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.Key.EndsWith(".nuspec"))
                    {
                        using (var stream = entry.OpenEntryStream())
                        {
                            manifest = XDocument.Load(stream);
                        }
                    }
                    else if (entry.Key.StartsWith("lib/") && entry.Key.EndsWith(".dll"))
                    {
                        var path = entry.Key.Substring(4);
                        string targetFramework = Path.GetDirectoryName(path);
                        // Simplify the world somewhat...
                        if (targetFramework.StartsWith("portable-"))
                        {
                            targetFramework = "PCL";
                        }
                        var assemblyFile = Path.GetFileName(path);
                        using (var stream = entry.OpenEntryStream())
                        {
                            // Mono.Cecil requires the stream to be seekable. It's simplest
                            // just to copy the whole DLL to a MemoryStream and pass that to Cecil.
                            var ms = new MemoryStream();
                            stream.CopyTo(ms);
                            ms.Position = 0;
                            assemblies.Add(BuildAssembly.Load(targetFramework, assemblyFile, ms));
                        }
                    }
                }
            }
            return new NuGetPackage(file, manifest, assemblies);
        }
    }
}
