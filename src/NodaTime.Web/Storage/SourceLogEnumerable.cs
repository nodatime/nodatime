using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.Storage
{
    public static class SourceLogEnumerable
    {
        public static IEnumerable<SourceLogEntry> EntriesBetween(this IEnumerable<SourceLogEntry> entries, string earlierHashExclusive, string laterHashInclusive)
        {
            return entries.SkipWhile(entry => !entry.Hash.StartsWith(laterHashInclusive))
                          .TakeWhile(entry => !entry.Hash.StartsWith(earlierHashExclusive));
        }
    }
}