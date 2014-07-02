using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NodaTime.Web.Controllers;
using NodaTime.Web.Storage;

namespace NodaTime.Web
{
    public class TrivialDependencyResolver : IDependencyResolver
    {
        private static readonly TimeSpan ReloadTime = TimeSpan.FromMinutes(1);

        private readonly FileWatchingReloader<BenchmarkRepository> repositoryWatcher;
        private readonly FileWatchingReloader<MercurialLog> logWatcher;

        public TrivialDependencyResolver()
        {
            string root = HostingEnvironment.ApplicationPhysicalPath;
            string benchmarkData = Path.Combine(root, "benchmarkdata");
            /*
            repositoryWatcher = new FileWatchingReloader<BenchmarkRepository>(
                Path.Combine(benchmarkData, "index.txt"),
                () => BenchmarkRepository.Load(benchmarkData),
                ReloadTime);*/
            repositoryWatcher = new FileWatchingReloader<BenchmarkRepository>(
                Path.Combine(benchmarkData, "benchmarks.xml"),
                () => BenchmarkRepository.LoadSingleFile(Path.Combine(benchmarkData, "benchmarks.xml")),
                ReloadTime);
            string logFile = Path.Combine(root, "hg-log.xml");
            logWatcher = new FileWatchingReloader<MercurialLog>(logFile, () => MercurialLog.Load(logFile), ReloadTime);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(BenchmarksController))
            {
                return new BenchmarksController(repositoryWatcher.Get(), logWatcher.Get());
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            Log.Info("Requesting multiple resolution of {0}", serviceType);
            return new List<object>();
        }
    }
}