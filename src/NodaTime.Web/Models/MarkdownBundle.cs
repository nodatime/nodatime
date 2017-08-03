// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using CommonMark;
using Microsoft.Extensions.FileProviders;
using NodaTime.Web.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodaTime.Web.Models
{
    public class MarkdownBundle
    {
        private Dictionary<string, MarkdownPage> pagesById;
        private Dictionary<string, MarkdownResource> resourcesById;

        public string Name { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<string> Resources { get; set; } = new List<string>();

        /// <summary>
        /// The parent bundle name, used to inherit resources and pages. 
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// The bundle associated with <see cref="Parent"/>.
        /// </summary>
        public MarkdownBundle ParentBundle { get; set; }

        public string ContentDirectory { get; set; }

        public class Category
        {
            public string Title { get; set; }
            public List<string> PageIds { get; set; }
            public List<MarkdownPage> Pages { get; set; }
        }        

        public MarkdownPage TryGetPage(string id)
        {
            pagesById.TryGetValue(id, out var ret);
            return ret;
        }

        public MarkdownResource TryGetResource(string id)
        {
            resourcesById.TryGetValue(id, out var ret);
            return ret;
        }

        internal void LoadContent(IFileProvider fileProvider, CommonMarkSettings commonMarkSettings)
        {        
            foreach (var category in Categories)
            {
                category.Pages = category.PageIds.Select(id => LoadPage(fileProvider, commonMarkSettings, id)).ToList();
            }
            resourcesById = Resources.ToDictionary(id => id, id => LoadResource(fileProvider, id));
            pagesById = Categories.SelectMany(c => c.Pages).ToDictionary(p => p.Id);
        }

        private MarkdownPage LoadPage(IFileProvider fileProvider, CommonMarkSettings commonMarkSettings, string id)
        {
            var filename = $"{ContentDirectory}/{id}.md";
            try
            {
                var file = fileProvider.GetFileInfo(filename);
                if (!file.Exists)
                {
                    if (ParentBundle == null)
                    {
                        throw new Exception($"Unable to find {file.Name} for bundle {Name} and no parent bundle exists");
                    }
                    var page = ParentBundle.TryGetPage(id);
                    if (page == null)
                    {
                        throw new Exception($"Unable to find {file.Name} for bundle {Name} and parent bundle doesn't have the page");
                    }
                    return page.WithBundle(this);
                }
                using (var reader = file.CreateReader())
                {
                    return MarkdownPage.Load(id, this, reader, commonMarkSettings);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to parse markdown content from {filename}", e);
            }
        }

        private MarkdownResource LoadResource(IFileProvider fileProvider, string resourceName)
        {
            var file = fileProvider.GetFileInfo($"{ContentDirectory}/{resourceName}");
            if (!file.Exists)
            {
                if (ParentBundle == null)
                {
                    throw new Exception($"Unable to find {file.Name} for bundle {Name} and no parent bundle exists");
                }
                var resource = ParentBundle.TryGetResource(resourceName);
                if (resource == null)
                {
                    throw new Exception($"Unable to find {file.Name} for bundle {Name} and parent bundle doesn't have the resource");
                }
                return resource;
            }
            using (var stream = file.CreateReadStream())
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                if (!resourceName.EndsWith(".png"))
                {
                    throw new InvalidOperationException("We only know how to deal with .png at the moment!");
                }
                return new MarkdownResource(resourceName, memoryStream.ToArray(), "image/png");
            }
        }
    }
}
