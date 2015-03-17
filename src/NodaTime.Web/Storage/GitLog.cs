using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NodaTime.Text;

namespace NodaTime.Web.Storage
{
    public static class GitLog
    {
        private const string CommitPrefix = "commit ";
        private const string AuthorPrefix = "Author: ";
        private const string DatePrefix = "Date: ";
        private static readonly OffsetDateTimePattern DatePattern = OffsetDateTimePattern.CreateWithInvariantCulture("ddd MMM d HH:mm:ss yyyy o<+HHmm>");
        private static readonly Regex AuthorWithEmailPattern = new Regex("Author: (?<name>[^<]*) <(?<email>[^>]*)>");

        public static ImmutableList<SourceLogEntry> Load(string file)
        {
            var lines = File.ReadAllLines(file);
            return LoadEntries(lines).OrderByDescending(x => x.Date).ToImmutableList();
        }

        private static IEnumerable<SourceLogEntry> LoadEntries(IEnumerable<string> lines)
        {
            var builder = new SourceLogEntry.Builder();
            var messageBuilder = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.StartsWith(CommitPrefix))
                {
                    if (builder.Hash != null)
                    {
                        builder.Message = messageBuilder.ToString();
                        messageBuilder.Length = 0;
                        yield return builder.Build();
                        builder.Reset();
                    }
                    builder.Hash = line.Substring(CommitPrefix.Length);
                }
                else if (line.StartsWith(AuthorPrefix))
                {
                    var match = AuthorWithEmailPattern.Match(line);
                    if (match.Success)
                    {
                        builder.AuthorName = match.Groups["name"].Value;
                        builder.AuthorEmail = match.Groups["email"].Value;
                    }
                    else
                    {
                        builder.AuthorName = line.Substring(AuthorPrefix.Length);
                    }
                }
                else if (line.StartsWith(DatePrefix))
                {
                    builder.Date = DatePattern.Parse(line.Substring(DatePrefix.Length).Trim()).Value.ToDateTimeOffset();
                }
                else
                {
                    string messageLine = line.Trim();
                    if (messageLine.Trim() == "")
                    {
                        continue;
                    }
                    if (messageBuilder.Length > 0)
                    {
                        messageBuilder.Append(" ");
                    }
                    messageBuilder.Append(messageLine);
                }
            }
            if (builder.Hash != null)
            {
                yield return builder.Build();
            }
        }
    }
}