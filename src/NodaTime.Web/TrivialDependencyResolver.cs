using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NodaTime.Web.Controllers;
using NodaTime.Web.Storage;
using System.Collections.Immutable;

namespace NodaTime.Web
{
    public class TrivialDependencyResolver : IDependencyResolver
    {
        private static readonly TimeSpan ReloadTime = TimeSpan.FromMinutes(1);

        private readonly FileWatchingReloader<BenchmarkRepository> repositoryWatcher;
        private readonly CombiningReloader<ImmutableList<SourceLogEntry>> logWatcher;

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
            string hgLogFile = Path.Combine(root, "hg-log.xml");
            string gitLogFile = Path.Combine(root, "git-log.txt");
            var hgWatcher = new FileWatchingReloader<ImmutableList<SourceLogEntry>>(hgLogFile, () => MercurialLog.Load(hgLogFile), ReloadTime);
            var gitWatcher = new FileWatchingReloader<ImmutableList<SourceLogEntry>>(gitLogFile, () => GitLog.Load(gitLogFile), ReloadTime);
            logWatcher = new CombiningReloader<ImmutableList<SourceLogEntry>>(new[] { hgWatcher, gitWatcher }.ToImmutableList(), logs => logs.SelectMany(x => x).OrderByDescending(x => x.Date).ToImmutableList());
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