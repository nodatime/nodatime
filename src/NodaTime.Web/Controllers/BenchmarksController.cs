using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Minibench.Framework;
using NodaTime.Web.Models;
using NodaTime.Web.Storage;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        private readonly BenchmarkRepository repository;
        private readonly ImmutableList<SourceLogEntry> log;

        public BenchmarksController(BenchmarkRepository repository, ImmutableList<SourceLogEntry> log)
        {
            this.repository = repository;
            this.log = log;
        }

        // GET: /Benchmarks/
        // or
        // GET: /Benchmarks/Machines
        public ActionResult Machines()
        {
            var machines = repository.RunsByMachine.Select(g => g.Key).OrderBy(name => name).ToList();
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
            string nextLabel = null; // In descending label order, so earlier...
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
            ImmutableList<SourceLogEntry> changes = null;
            if (nextLabel != null)
            {
                string earlierHash = BenchmarkRepository.HashForLabel(nextLabel);
                string thisHash = BenchmarkRepository.HashForLabel(label);
                changes = log.EntriesBetween(earlierHash, thisHash).ToImmutableList();
            }
            return View(new BenchmarkRunAndSourceLogs(run, changes));
        }


        public ActionResult MethodHistory(string machine, string method, bool full = false)
        {
            var history = MethodHistoryModel.ForMachineMethod(repository, machine, method, full);
            return View(history);
        }
    }
}