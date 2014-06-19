using NodaTime.Web.Storage;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    // A single complete log (a set of entries).
    public sealed class MercurialLog
    {
        public ImmutableList<MercurialLogEntry> Entries { get; private set; }

        private MercurialLog()
        {
        }

        public static MercurialLog Load(string uri)
        {
            var entries = XDocument.Load(uri)
                                   .Descendants("logentry")
                                   .Select(MercurialLogEntry.FromXElement)
                                   .OrderByDescending(x => x.Date)
                                   .ToImmutableList();
            return new MercurialLog { Entries = entries };
        }

        public IEnumerable<MercurialLogEntry> EntriesBetween(string earlierHashExclusive, string laterHashInclusive)
        {
            return Entries.SkipWhile(entry => !entry.Hash.StartsWith(laterHashInclusive))
                          .TakeWhile(entry => !entry.Hash.StartsWith(earlierHashExclusive));
        }
    }
}