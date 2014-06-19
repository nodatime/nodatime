using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// Abstraction 
    /// </summary>
    /// <typeparam name="T">Type of resource provided</typeparam>
    public class FileWatchingReloader<T>
    {
        private readonly string triggerFile;
        private readonly Func<T> reloadFunction;
        private readonly TimeSpan timeBetweenChecks;
        private readonly object padlock = new object();
        private DateTime lastWrite;
        private DateTime nextCheckTime;
        private T currentValue;

        public FileWatchingReloader(string triggerFile, Func<T> reloadFunction, TimeSpan timeBetweenChecks)
        {
            this.triggerFile = triggerFile;
            this.reloadFunction = reloadFunction;
            this.timeBetweenChecks = timeBetweenChecks;
            lastWrite = DateTime.MinValue;
            nextCheckTime = DateTime.MinValue;
            // First call to GetValue will reload
        }

        /// <summary>
        /// Fetches the value, reloading it if necessary. This involves locking,
        /// so that if multiple threads request the value at the same time (and it
        /// is out of date), they will all block until the first one has reloaded it,
        /// at which point they will all return the same reference.
        /// </summary>
        /// <remarks>
        /// In the future, we could reload in the background. (Or even use a FileSystemWatcher...)
        /// </remarks>
        public T Get()
        {
            lock (padlock)
            {
                DateTime now = DateTime.UtcNow;
                if (now < nextCheckTime)
                {
                    return currentValue;
                }
                nextCheckTime = now + timeBetweenChecks;
                DateTime latestLastWrite = File.GetLastWriteTime(triggerFile);
                if (latestLastWrite == lastWrite)
                {
                    return currentValue;
                }
                currentValue = reloadFunction();
                lastWrite = latestLastWrite;
                return currentValue;
            }
        }

    }
}