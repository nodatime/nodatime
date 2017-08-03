// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using CommonMark;
using CommonMark.Syntax;
using Microsoft.AspNetCore.Html;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// A page within the Markdown directory, already rendered and with metadata.
    /// </summary>
    public class MarkdownPage
    {
        private static readonly Regex DirectiveSplitter = new Regex(@"^@([a-zA-Z]+)\s*=\s*""(.*)""$");

        public string Title { get; }
        public string Id { get; }
        public IHtmlContent Content { get; }
        public MarkdownBundle Bundle { get; }

        private MarkdownPage(string id, MarkdownBundle bundle, string title, IHtmlContent content)
        {
            Id = id;
            Title = title;
            Content = content;
            Bundle = bundle;
        }

        public static MarkdownPage InheritancePlaceholder(string id, MarkdownBundle bundle) =>
            new MarkdownPage(id, bundle, null, null);

        /// <summary>
        /// Creates a new page with the same ID, content and title, but the specified bundle.
        /// This is used to create an "inherited" page.
        /// </summary>
        public MarkdownPage WithBundle(MarkdownBundle bundle) =>
            new MarkdownPage(Id, bundle, Title, Content);

        public static MarkdownPage Load(string id, MarkdownBundle bundle, TextReader reader, CommonMarkSettings commonMarkSettings)
        {
            string line;
            string title = "";
            while ((line = reader.ReadLine()).StartsWith("@"))
            {
                var match = DirectiveSplitter.Match(line);
                if (!match.Success)
                {
                    throw new Exception($"Invalid directive: {line}");
                }
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (key == "Title")
                {
                    title = value;
                }
            }
            if (line.Length > 0)
            {
                throw new Exception("Blank line required after directives");
            }

            // TODO: Just use CommonMarkConverter.Convert? (We don't do anything between
            // stages.)
            Block block = CommonMarkConverter.ProcessStage1(reader, commonMarkSettings);
            CommonMarkConverter.ProcessStage2(block, commonMarkSettings);
            var writer = new StringWriter();
            CommonMarkConverter.ProcessStage3(block, writer, commonMarkSettings);

            var content = new HtmlString(writer.ToString());
            return new MarkdownPage(id, bundle, title, content);
        }       
    }
}
