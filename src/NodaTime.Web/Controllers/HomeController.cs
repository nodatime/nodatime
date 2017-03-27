using Microsoft.AspNetCore.Mvc;
using NodaTime.Web.Models;
using NodaTime.Web.Providers;
using System;

namespace NodaTime.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MarkdownBundle markdownBundle;

        public HomeController(MarkdownLoader markdownLoader)
        {
            markdownBundle = markdownLoader.TryGetBundle("root");
            if (markdownBundle == null)
            {
                throw new ArgumentException("Couldn't get root bundle", nameof(markdownLoader));
            }
        }

        public IActionResult Versions() => View("Docs", markdownBundle.TryGetPage("versions"));

        public IActionResult Index() => View();
    }
}
