using Microsoft.AspNetCore.Mvc;
using NodaTime.Web.Models;
using System.Linq;

namespace NodaTime.Web.Controllers
{
    public class TzdbController : Controller
    {
        private const string ContentType = "application/octet-stream";
        private readonly ITzdbRepository tzdbRepository;

        public TzdbController(ITzdbRepository tzdbRepository)
        {
            this.tzdbRepository = tzdbRepository;
        }

        [Route(@"/tzdb/{id:regex(^.*\.nzd$)}")]
        public IActionResult Get(string id)
        {
            var release = tzdbRepository.GetRelease(id);
            if (release == null)
            {
                return NotFound();
            }
            return File(release.GetContent(), ContentType, release.Name);
        }

        [Route("/tzdb/latest.txt")]
        public IActionResult Latest() =>
            new ContentResult
            {
                ContentType = "text/plain",
                Content = tzdbRepository.GetReleases().Last().NodaTimeOrgUrl,
                StatusCode = 200
            };

        [Route("/tzdb/index.txt")]
        public IActionResult Index()
        {
            var releases = tzdbRepository.GetReleases();
            var releaseUrls = releases.Select(r => r.NodaTimeOrgUrl);
            return new ContentResult
            {
                ContentType = "text/plain",
                Content = string.Join("\r\n", releaseUrls),
                StatusCode = 200
            };
        }
    }
}
