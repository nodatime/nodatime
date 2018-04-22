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
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();
            var members =
                from file in Directory.EnumerateFiles(directory, "*.yml", SearchOption.AllDirectories)
                where Path.GetFileName(file) != "toc.yml"
                from item in Load(deserializer, file).Items
                select item;
            return new Release(version, members.ToList());
        }

        private static DocfxYamlFile Load(Deserializer deserializer, string file)
        {
            string yaml = File.ReadAllText(file);
            var doc = deserializer.Deserialize<DocfxYamlFile>(yaml);
            foreach (var member in doc.Items)
            {
                member.YamlFile = file;
            }
            return doc;
        }

        public class DocfxYamlFile
        {
            public List<DocfxMember> Items { get; set; }
        }
    }
}
