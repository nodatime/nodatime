// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.Helpers;
using NodaTime.TimeZones;
using NodaTime.TzValidate.NodaDump;
using NodaTime.Web.Models;
using System.IO;
using System.Linq;
using System.Net;

namespace NodaTime.Web.Controllers
{
    [AddHeader("X-Robots-Tag", "noindex")]
    public class TzValidateController : Controller
    {
        private readonly ITzdbRepository repository;

        public TzValidateController(ITzdbRepository repository) =>
            this.repository = repository;

        [Route("/tzvalidate/generate")]
        public IActionResult Generate(int startYear = 1, int endYear = 2035, string? zone = null, string? version = null)
        {
            if (startYear < 1 || endYear > 3000 || startYear > endYear)
            {
                return BadRequest("Invalid start/end year combination");
            }

            var source = TzdbDateTimeZoneSource.Default;
            if (version != null)
            {
                var release = repository.GetRelease($"tzdb{version}.nzd");
                if (release == null)
                {
                    return BadRequest("Unknown version");
                }
                source = TzdbDateTimeZoneSource.FromStream(release.GetContent());
            }

            if (zone != null && !source.GetIds().Contains(zone))
            {
                return BadRequest("Unknown zone");
            }

            var writer = new StringWriter();
            var options = new Options { FromYear = startYear, ToYear = endYear, ZoneId = zone };
            var dumper = new ZoneDumper(source, options);
            dumper.Dump(writer);

            return new ContentResult
            {
                Content = writer.ToString(),
                ContentType = "text/plain",
                StatusCode = (int) HttpStatusCode.OK
            };
        }
    }
}
