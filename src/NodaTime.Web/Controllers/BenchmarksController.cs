using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using NodaTime.Web.Storage;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        private BenchmarkRepositoryHolder repositoryHolder = new BenchmarkRepositoryHolder(
            Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "benchmarkdata"),
            TimeSpan.FromMinutes(1));

        // GET: /Benchmarks/
        // or
        // GET: /Benchmarks/Machines
        public ActionResult Machines()
        {
            var machines = repositoryHolder.GetRepository().RunsByMachine.Select(g => g.Key).ToList();
            return View(machines);
        }

        public ActionResult Machine(string machine)
        {
            var runs = repositoryHolder.GetRepository().RunsByMachine[machine];
            ViewBag.Machine = machine;
            return View(runs);
        }
	}
}