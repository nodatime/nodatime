using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using NodaTime.Web.Models;
using NodaTime.Web.Storage;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        private static readonly TimeSpan ReloadTime = TimeSpan.FromMinutes(1);

        private FileWatchingReloader<BenchmarkRepository> repositoryWatcher;
        private FileWatchingReloader<MercurialLog> logWatcher;

        public BenchmarksController()
        {
            string directory = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "benchmarkdata");
            repositoryWatcher = new FileWatchingReloader<BenchmarkRepository>(
                Path.Combine(directory, "index.txt"),
                () => BenchmarkRepository.Load(directory),
                ReloadTime);
            string logFile = Path.Combine(directory, "hg-log.xml");
            logWatcher = new FileWatchingReloader<MercurialLog>(logFile, () => MercurialLog.Load(logFile), ReloadTime);
        }

        // GET: /Benchmarks/
        // or
        // GET: /Benchmarks/Machines
        public ActionResult Machines()
        {
            var machines = repositoryWatcher.Get().RunsByMachine.Select(g => g.Key).ToList();
            return View(machines);
        }

        public ActionResult Machine(string machine)
        {
            var runs = repositoryWatcher.Get().RunsByMachine[machine];
            ViewBag.Machine = machine;
            return View(runs);
        }

        public ActionResult Diff(string machine, string left, string right)
        {
            var repository = repositoryWatcher.Get();
            var log = logWatcher.Get();

            var leftFile = repository.GetRun(machine, left);
            var rightFile = repository.GetRun(machine, right);

            return View(new BenchmarkDiff(leftFile, rightFile, log));
        }


        public ActionResult BenchmarkRun(string machine, string label)
        {
            var repository = repositoryWatcher.Get();
            var log = logWatcher.Get();

            BenchmarkRun run = null;
            string nextLabel = null; // In descending date order, so earlier...
            foreach (var candidate in repository.RunsByMachine[machine])
            {
                if (run != null)
                {
                    nextLabel = candidate.Label;
                    break;
                }
                if (candidate.Label == label)
                {
                    run = candidate;
                }
            }
            ImmutableList<MercurialLogEntry> changes = null;
            if (nextLabel != null)
            {
                string earlierHash = BenchmarkRepository.HashForLabel(nextLabel);
                string thisHash = BenchmarkRepository.HashForLabel(label);
                changes = log.EntriesBetween(earlierHash, thisHash).ToImmutableList();
            }
            return View(new BenchmarkRunAndMercurialLogs(run, changes));
        }


        public ActionResult MethodHistory(string machine, string method)
        {
            var repository = repositoryWatcher.Get();
            var history = MethodHistoryModel.ForMachineMethod(repository, machine, method);
            return View(history);
        }

    }
}