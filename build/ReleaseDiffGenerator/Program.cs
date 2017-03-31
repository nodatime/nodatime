using System;
using System.IO;
using DocfxYamlLoader;
using System.Collections.Generic;
using System.Linq;

namespace ReleaseDiffGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ReleaseDiffGenerator <old release directory> <new release directory>");
                return 1;
            }

            var oldRelease = Release.Load(args[0], Path.GetFileName(args[0]));
            var newRelease = Release.Load(args[1], Path.GetFileName(args[1]));


            var oldMemberUids = new HashSet<string>(oldRelease.MembersByUid.Keys);
            var newMemberUids = new HashSet<string>(newRelease.MembersByUid.Keys);

            var addedMembers = newMemberUids
                .Except(oldMemberUids)
                .OrderBy(uid => uid)
                .Select(uid => newRelease.MembersByUid[uid])
                // Don't include members where the parent is also new
                .Where(m => m.Parent == null || oldMemberUids.Contains(m.Parent))
                .ToList();
            var removedMembers = oldMemberUids
                .Except(newMemberUids)
                .OrderBy(uid => uid)
                .Select(uid => oldRelease.MembersByUid[uid])
                // Don't include members where the parent was also removed
                .Where(m => m.Parent == null || newMemberUids.Contains(m.Parent))
                .ToList();

            // TODO:
            // - Linking of removed items (can't be a normal link, as it has to be to previous version)
            // - Display text of link
            // - Grouping by type (only when there's more than one item for the type? Unsure)
            // - Investigate warnings given by docfx of invalid UIDs.
            using (var writer = File.CreateText(Path.Combine(args[1], "api", "changes.md")))
            {
                writer.WriteLine($"# API changes from {oldRelease.Version} to {newRelease.Version}");
                writer.WriteLine();

                writer.WriteLine($"## Added in {newRelease.Version}");
                writer.WriteLine();
                foreach (var member in addedMembers)
                {
                    writer.WriteLine($"- [{member.Uid}](xref:{member.Uid}) ({member.Type})");
                }
                writer.WriteLine();

                writer.WriteLine($"## Removed in {newRelease.Version} (present in {oldRelease.Version})");
                writer.WriteLine();
                foreach (var member in removedMembers)
                {
                    writer.WriteLine($"- {member.Uid} ({member.Type})");
                }
            }

            var tocFile = Path.Combine(args[1], "api", "toc.yml");
            var toc = File.ReadAllLines(tocFile).ToList();
            if (!toc[1].StartsWith("- name: Changes"))
            {
                toc.Insert(1, $"- name: Changes from {oldRelease.Version}");
                toc.Insert(2, "  href: changes.md");
            }
            File.WriteAllLines(tocFile, toc);

            return 0;
        }
    }
}