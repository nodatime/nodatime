// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.TimeZones;
using NodaTime.TzValidate.NodaDump;
using System.IO;
using System.Net;

namespace NodaTime.Web.Controllers
{
    public class TzValidateController : Controller
    {
        // This will do something soon :)
        [Route("/tzvalidate/generate")]
        public IActionResult Generate(int startYear = 1, int endYear = 2035)
        {
            var source = TzdbDateTimeZoneSource.Default;
            var writer = new StringWriter();
            var options = new Options { FromYear = startYear, ToYear = endYear };
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
