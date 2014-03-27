using System;
using System.IO;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// Holds a BenchmarkRepository and reloads it periodically if necessary.
    /// </summary>
    public class BenchmarkRepositoryHolder
    {
        private readonly string directory;
        private readonly string index;
        private readonly TimeSpan timeBetweenChecks;
        private DateTime loadedIndexLastModified;
        private DateTime indexNextCheck;
        private BenchmarkRepository currentRepository;
        private readonly object padlock = new object();

        public BenchmarkRepositoryHolder(string directory, TimeSpan timeBetweenChecks)
        {
            this.directory = directory;
            this.index = Path.Combine(directory, "index.txt");
            this.timeBetweenChecks = timeBetweenChecks;
            loadedIndexLastModified = DateTime.MinValue;
            indexNextCheck = DateTime.MinValue;
            // First call to GetRepository will load.
        }

        /// <summary>
        /// Fetches the repository, reloading it if necessary. This involves locking,
        /// so that if multiple threads request the repository at the same time (and it
        /// is out of date), they will all block until the first one has reloaded it,
        /// at which point they will all return the same reference.
        /// </summary>
        /// <remarks>
        /// In the future, we could reload in the background. (Or even use a FileSystemWatcher...)
        /// </remarks>
        public BenchmarkRepository GetRepository()
        {
            lock (padlock)
            {
                DateTime now = DateTime.UtcNow;
                if (now < indexNextCheck)
                {
                    return currentRepository;
                }
                indexNextCheck = now + timeBetweenChecks;
                DateTime indexLastModified = File.GetLastWriteTime(index);
                if (indexLastModified == loadedIndexLastModified)
                {
                    return currentRepository;
                }
                currentRepository = BenchmarkRepository.Load(directory);
                loadedIndexLastModified = indexLastModified;
                return currentRepository;
            }
        }
    }
}