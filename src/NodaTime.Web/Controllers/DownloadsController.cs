// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.Web.Models;

namespace NodaTime.Web.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly IReleaseRepository releaseRepository;

        public DownloadsController(IReleaseRepository releaseRepository)
        {
            this.releaseRepository = releaseRepository;
        }

        [Route("/downloads/index.html")]
        [Route("/downloads")]
        public IActionResult Index()
        {
            var releases = releaseRepository.GetReleases();
            return View(releases);
        }
    }
}
