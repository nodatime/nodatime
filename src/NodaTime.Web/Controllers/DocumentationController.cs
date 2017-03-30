// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.Web.Providers;

namespace NodaTime.Web.Controllers
{
    public class DocumentationController : Controller
    {
        private readonly MarkdownLoader loader;

        public DocumentationController(MarkdownLoader loader)
        {
            this.loader = loader;
        }

        public IActionResult ViewDocumentation(string bundle, string url)
        {           
            if (url == null || url.EndsWith("/"))
            {
                url += "index";
            }

            var page = loader.TryGetBundle(bundle)?.TryGetPage(url);
            if (page != null)
            {
                return View("Docs", page);
            }
            var resource = loader.TryGetBundle(bundle)?.TryGetResource(url);
            if (resource != null)
            {
                return File(resource.GetContent(), resource.ContentType);
            }
            return NotFound();
        }
    }
}
