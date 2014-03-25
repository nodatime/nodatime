using System.Web.Mvc;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        // GET: /Benchmarks/
        // or
        // GET: /Benchmarks/Machines
        public ActionResult Machines()
        {
            object model = new string[] { "Machine1", "Machine2" };
            return View(model);
        }

        public ActionResult Machine(string machine)
        {
            object model = machine;
            return View(model);
        }
	}
}