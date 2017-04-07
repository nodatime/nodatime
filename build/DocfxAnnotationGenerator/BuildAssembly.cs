using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace DocfxAnnotationGenerator
{
    public sealed class BuildAssembly
    {
        public string TargetFramework { get; }
        public string AssemblyName { get; }
        public IImmutableList<ReflectionMember> Members { get; }

        private BuildAssembly(string targetFramework, string assemblyName, IEnumerable<ReflectionMember> members)
        {
            TargetFramework = targetFramework;
            AssemblyName = assemblyName;
            Members = members.ToImmutableList();
        }

        public static BuildAssembly Load(string targetFramework, string assemblyName, Stream stream) =>
            new BuildAssembly(targetFramework, assemblyName, ReflectionMember.Load(stream));

        public static BuildAssembly Load(string targetFramework, string assemblyName, string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return Load(targetFramework, assemblyName, stream);
            }
        }
    }
}
