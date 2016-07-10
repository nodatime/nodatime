// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace NodaTime.Web.Controllers
{
    public class TzValidateController : Controller
    {
        // This will do something soon :)
        public IActionResult Index(int startYear = 1, int endYear = 2035)
        {
            return new ContentResult
            {
                Content = $"Foo; start={startYear} end = {endYear}",
                ContentType = "text/plain",
                StatusCode = (int) HttpStatusCode.OK
            };
        }
    }
}
