using System.Collections.Generic;

namespace NodaTime.Web.Models
{
    public interface ITzdbRepository
    {
        /// <summary>
        /// Returns the list of releases, most recent first.
        /// </summary>
        IList<TzdbDownload> GetReleases();

        /// <summary>
        /// Gets the given release, if it exists (or null otherwise).
        /// </summary>
        TzdbDownload GetRelease(string name);
    }
}
