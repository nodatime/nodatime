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
        private readonly BenchmarkRepository repository;
        private readonly MercurialLog log;

        public BenchmarksController(BenchmarkRepository repository, MercurialLog log)
        {
            this.repository = repository;
            this.log = log;
        }

        // GET: /Benchmarks/
        // or
        // GET: /Benchmarks/Machines
        public ActionResult Machines()
        {
            var machines = repository.RunsByMachine.Select(g => g.Key).ToList();
            ViewBag.LoadingTime = repository.LoadingTime;
            ViewBag.Loaded = repository.Loaded;
            return View(machines);
        }

        public ActionResult Machine(string machine)
        {
            var runs = repository.RunsByMachine[machine];
            ViewBag.Machine = machine;
            return View(runs);
        }

        public ActionResult Diff(string machine, string left, string right)
        {
            var leftFile = repository.GetRun(machine, left);
            var rightFile = repository.GetRun(machine, right);

            return View(new BenchmarkDiff(leftFile, rightFile, log));
        }


        public ActionResult BenchmarkRun(string machine, string label)
        {
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
            var history = MethodHistoryModel.ForMachineMethod(repository, machine, method);
            return View(history);
        }
    }
}