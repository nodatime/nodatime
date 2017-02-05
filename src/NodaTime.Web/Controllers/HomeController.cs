using Microsoft.AspNetCore.Mvc;

namespace NodaTime.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
