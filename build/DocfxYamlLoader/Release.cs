using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DocfxYamlLoader
{
    public class Release
    {
        public string Version { get; }
        public List<DocfxMember> Members { get; }
        public Dictionary<string, DocfxMember> MembersByUid { get; }

        private Release(string version, List<DocfxMember> members)
        {
            Version = version;
            Members = members;
            MembersByUid = Members.ToDictionary(m => m.Uid);
            foreach (var member in members)
            {
                if (member.Parent != null)
                {
                    member.ParentMember = MembersByUid[member.Parent];
                }
            }
        }

        public static Release Load(string directory, string version)
        {
            var serializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();
            var members = Directory.EnumerateFiles(directory, "*.yml", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f) != "toc.yml")
                .Select(f => File.ReadAllText(f))
                .Select(yaml => serializer.Deserialize<DocfxYamlFile>(yaml))
                .SelectMany(f => f.Items)
                .ToList();
            return new Release(version, members);
        }

        public class DocfxYamlFile
        {
            public List<DocfxMember> Items { get; set; }
        }
    }
}
