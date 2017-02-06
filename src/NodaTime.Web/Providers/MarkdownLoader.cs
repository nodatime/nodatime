// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommonMark;
using CommonMark.Formatters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using NodaTime.Web.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using CommonMark.Syntax;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Providers
{
    public class MarkdownLoader
    {
        private const string NodaTypePrefix = "noda-type://";
        private const string NodaPropertyPrefix = "noda-property://";
        private static readonly Regex IssuePlaceholderPattern = new Regex(@"^issue \d+$");

        private readonly IFileProvider fileProvider;
        private readonly CommonMarkSettings commonMarkSettings;
        private readonly Dictionary<string, MarkdownBundle> bundles;

        public MarkdownLoader(IHostingEnvironment environment)
        {
            fileProvider = environment.ContentRootFileProvider;
            commonMarkSettings = CommonMarkSettings.Default.Clone();
            bundles = new Dictionary<string, MarkdownBundle>();

            // Lets us resolve numbers to issue links.
            commonMarkSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            commonMarkSettings.OutputDelegate = FormatDocument;
            // May become obsolete, if we can resolve [NodaTime.LocalDateTime] to the right type link etc.
            commonMarkSettings.UriResolver = ResolveUrl;

            // TODO: Make the root location configurable
            LoadRecursive("Markdown");
        }

        private void LoadRecursive(string directory)
        {
            IDirectoryContents rootContents = fileProvider.GetDirectoryContents(directory);
            foreach (var fileInfo in rootContents)
            {
                LoadRecursive($"{directory}/{fileInfo.Name}");
            }
            var index = fileProvider.GetFileInfo($"{directory}/index.json");
            if (index.Exists)
            {
                LoadBundle(directory, index);
            }
        }

        private void LoadBundle(string directory, IFileInfo index)
        {
            var bundle = JsonConvert.DeserializeObject<MarkdownBundle>(index.ReadAllText());
            // TODO: Try to persuade Json.NET to convert a string into a MarkdownPage to then
            // load later.
            foreach (var category in bundle.Categories)
            {
                category.Pages = category.PageIds.Select(id => LoadPage(directory, id, bundle)).ToList();
            }
            bundle.BuildIndex();
            bundles.Add(bundle.Name, bundle);
        }

        private MarkdownPage LoadPage(string directory, string id, MarkdownBundle bundle)
        {
            var file = fileProvider.GetFileInfo($"{directory}/{id}.md");
            using (var reader = file.CreateReader())
            {
                return MarkdownPage.Load(id, bundle, reader, commonMarkSettings);
            }
        }

        private static string ResolveUrl(string url)
        {
            if (url.StartsWith(NodaTypePrefix))
            {
                url = url.Substring(NodaTypePrefix.Length);
                int hashIndex = url.IndexOf('#');
                string type = hashIndex < 0 ? url : url.Substring(0, hashIndex);
                string anchor = hashIndex < 0 ? "" : url.Substring(hashIndex);
                return $"../api/{type}.html{anchor}";
            }
            else if (url.StartsWith(NodaPropertyPrefix))
            {
                url = url.Substring(NodaPropertyPrefix.Length);
                int nameIndex = url.LastIndexOf('.');
                string type = url.Substring(0, nameIndex);
                return $"../api/{type}.html#{url.Replace(".", "_")}";
            }
            return url;
        }

        public MarkdownBundle TryGetBundle(string bundle)
        {
            MarkdownBundle ret;
            bundles.TryGetValue(bundle, out ret);
            return ret;
        }

        private string ResolvePlaceholder(string placeholder)
        {
            if (IssuePlaceholderPattern.IsMatch(placeholder))
            {
                string url = placeholder.Replace("issue ", "https://github.com/nodatime/nodatime/issues/");
                return $"<a href=\"{url}\">{placeholder}</a>";
            }
            throw new Exception($"Unhandled placeholder: '{placeholder}'");
        }

        private void FormatDocument(Block block, TextWriter writer, CommonMarkSettings settings)
        {
            var formatter = new HtmlFormatter(writer, settings) { PlaceholderResolver = ResolvePlaceholder };
            formatter.WriteDocument(block);
        }
    }

    internal static class FileInfoExtensions
    {
        internal static TextReader CreateReader(this IFileInfo file) => new StreamReader(file.CreateReadStream());
        internal static string ReadAllText(this IFileInfo file)
        {
            using (var reader = file.CreateReader())
            {
                return reader.ReadToEnd();
            }
        }
    }
}
