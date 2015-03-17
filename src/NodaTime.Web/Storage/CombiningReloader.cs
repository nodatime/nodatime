using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Web;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// Combines multiple FileWatchingReloaders in order to produce an aggregated result.
    /// </summary>
    public class CombiningReloader<T>
    {
        private readonly ImmutableList<FileWatchingReloader<T>> reloaders;
        private readonly Func<IEnumerable<T>, T> combiner;

        private readonly object padlock = new object();
        private IEnumerable<T> lastComponents;
        private T lastResult;

        public CombiningReloader(ImmutableList<FileWatchingReloader<T>> reloaders, Func<IEnumerable<T>, T> combiner)
        {
            this.reloaders = reloaders;
            this.combiner = combiner;
        }

        public T Get()
        {
            lock (padlock)
            {
                var newComponents = reloaders.Select(r => r.Get()).ToList();
                if (lastComponents == null || !newComponents.SequenceEqual(lastComponents))
                {
                    lastComponents = newComponents.ToList();
                    lastResult = combiner(newComponents);
                }
                return lastResult;
            }
        }
    }
}