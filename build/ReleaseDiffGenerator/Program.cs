using System;
using System.IO;
using DocfxYamlLoader;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

            // TODO: Newly-obsoleted members?

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
            using (var writer = File.CreateText(Path.Combine(args[1], "api", "changes.md")))
            {
                writer.WriteLine($"# API changes from {oldRelease.Version} to {newRelease.Version}");

                WriteChanges(writer, addedMembers, true);
                WriteChanges(writer, removedMembers, false);
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

        static void WriteChanges(TextWriter writer, IEnumerable<DocfxMember> members, bool added)
        {
            string newOrRemoved = added ? "New" : "Removed";

            // Types and namespaces, individually
            WriteTypes(writer, $"{newOrRemoved} namespaces", members, DocfxMember.TypeKind.Namespace, added);
            WriteTypes(writer, $"{newOrRemoved} classes", members, DocfxMember.TypeKind.Class, added);
            WriteTypes(writer, $"{newOrRemoved} structs", members, DocfxMember.TypeKind.Struct, added);
            WriteTypes(writer, $"{newOrRemoved} interfaces", members, DocfxMember.TypeKind.Interface, added);
            WriteTypes(writer, $"{newOrRemoved} delegates", members, DocfxMember.TypeKind.Delegate, added);
            WriteTypes(writer, $"{newOrRemoved} enums", members, DocfxMember.TypeKind.Enum, added);

            // Now members of types (where the whole type isn't new/removed)
            var membersByType = members
                .Where(m => m.IsTypeMember)
                .GroupBy(m => m.ParentMember)
                .OrderBy(g => g.Key.Uid);

            if (membersByType.Any())
            {
                writer.WriteLine();
                writer.WriteLine($"## {newOrRemoved} type members, by type");
                foreach (var group in membersByType)
                {
                    writer.WriteLine();
                    var type = group.Key;
                    string typeMd = added ? $"[`{type.DisplayName}`](xref:{WebUtility.UrlEncode(type.Uid)})"
                        : $"`{type.DisplayName}`";
                    writer.WriteLine($"### {newOrRemoved} members in {typeMd}");
                    writer.WriteLine();
                    foreach (var member in group)
                    {
                        WriteBullet(writer, member, added, true);
                    }
                }
            }
        }

        static void WriteTypes(TextWriter writer, string title, IEnumerable<DocfxMember> members,
            DocfxMember.TypeKind kind, bool link)
        {
            var kindMembers = members.Where(m => m.Type == kind);
            if (!kindMembers.Any())
            {
                return;
            }
            writer.WriteLine();
            writer.WriteLine($"## {title}");
            writer.WriteLine();
            foreach (var member in kindMembers)
            {
                // We generate the "(obsolete)" part even for new members... unlikely, but possible.
                // (We haven't made IsoDayOfWeekExtensions.ToIsoDayOfWeek obsolete in 2.0.x, but
                // it's possible...)
                WriteBullet(writer, member, link, true);
            }
        }

        static void WriteBullet(TextWriter writer, DocfxMember member, bool link, bool checkObsolete)
        {
            string obsolete = checkObsolete && member.Obsolete ? " (obsolete)" : "";
            if (link)
            {
                writer.WriteLine($"- [`{member.DisplayName}`](xref:{WebUtility.UrlEncode(member.Uid)}){obsolete}");
            }
            else
            {
                writer.WriteLine($"- `{member.DisplayName}`{obsolete}");
            }
        }
    }
}