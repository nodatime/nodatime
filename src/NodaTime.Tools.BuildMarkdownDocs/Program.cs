// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using MarkdownSharp;

namespace NodaTime.Tools.BuildMarkdownDocs
{
    /// <summary>
    /// Simple program to build Markdown-based documentation.
    /// </summary>
    internal sealed class Program
    {
        private const string TemplateTitle = "%TITLE%";
        private const string TemplateHtmlTitle = "%HTML_TITLE%";
        private const string TemplateBody = "%BODY%";
        private const string TemplateProjectTitle = "%PROJECT_TITLE%";
        private const string TemplatePrevAnchor = "%PREV_ANCHOR%";
        private const string TemplateNextAnchor = "%NEXT_ANCHOR%";
        private const string TemplateFooter = "%FOOTER%";
        private const string TemplateTOC = "%TOC%";
        private const string ApiUrlPrefix = "../api/html/";
        private static readonly Regex NamespacePattern = new Regex(@"noda-ns://([A-Za-z0-9_.]*)", RegexOptions.Multiline);
        private static readonly Regex TypePattern = new Regex(@"noda-type://([A-Za-z0-9_.]*)", RegexOptions.Multiline);
        private static readonly Regex IssueUrlPattern = new Regex(@"(\[[^\]]*\])\[issue (\d+)\]", RegexOptions.Multiline);
        private static readonly Regex IssueLinkPattern = new Regex(@"\[issue (\d+)\]\[\]", RegexOptions.Multiline);

        private static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: NodaTime.Tools.BuildMarkdownDocs <project.xml> <output directory>");
                Console.WriteLine("Note: Output directory will be completely removed before processing. Use with care!");
                return 1;
            }

            try
            {
                BuildDocumentation(args[0], args[1]);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }

        private static void BuildDocumentation(string projectXmlFilename, string outputDirectory)
        {
            var project = ReadProject(projectXmlFilename);
            Console.WriteLine("Building {0}", project.Title);

            CheckFileExists(project.MarkdownTemplateFile);
            project.MarkdownTemplate = File.ReadAllText(project.MarkdownTemplateFile);

            // Check other pre-requisites before blowing away the output directory.
            foreach (string includedDirectory in project.IncludedDirectories)
            {
                CheckDirectoryExists(includedDirectory);
            }
            // Read the section titles and compute the output filename and href.
            foreach (Section section in project.Sections)
            {
                CheckFileExists(section.InputFilename);
                section.Title = ReadTitle(section.InputFilename);
                string outputFilename = Path.ChangeExtension(Path.GetFileName(section.InputFilename), "html");
                section.OutputFilename = Path.Combine(outputDirectory, outputFilename);
                section.Href = outputFilename;  // or could convert 'index.html' -> '.'
            }

            if (Directory.Exists(outputDirectory))
            {
                Console.WriteLine("Deleting previous output directory");
                Directory.Delete(outputDirectory, true);
            }
            Directory.CreateDirectory(outputDirectory);

            foreach (string includedDirectory in project.IncludedDirectories)
            {
                Console.WriteLine("Including contents of directory {0}", includedDirectory);
                foreach (string rawFile in Directory.GetFiles(includedDirectory))
                {
                    File.Copy(rawFile, Path.Combine(outputDirectory, Path.GetFileName(rawFile)));
                }
            }

            foreach (Section section in project.Sections)
            {
                Console.WriteLine("{0}{1} ({2})", "".PadLeft(section.TOCDepth), section.Title, Path.GetFileName(section.OutputFilename));
                RenderSection(project, section);
            }
            Console.WriteLine("Done.");
        }

        /// <summary>
        /// A mutable description of the whole project, including derived information.
        /// </summary>
        private sealed class Project
        {
            /// <summary>
            /// The project title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The page footer.
            /// </summary>
            public string Footer { get; set; }

            /// <summary>
            /// The Markdown template's filename.
            /// </summary>
            public string MarkdownTemplateFile { get; set; }

            /// <summary>
            /// The Markdown template.
            /// </summary>
            public string MarkdownTemplate { get; set; }

            /// <summary>
            /// The directories to include directly.
            /// </summary>
            public IList<string> IncludedDirectories { get; set; }

            /// <summary>
            /// The sections of the document to generate, in document order.
            /// </summary>
            public IList<Section> Sections { get; set; }
        }

        /// <summary>
        /// A section in the document.
        /// </summary>
        private sealed class Section
        {
            /// <summary>
            /// The Markdown filename containing the section's contents.
            /// </summary>
            public string InputFilename { get; set; }

            /// <summary>
            /// The HTML output filename.
            /// </summary>
            public string OutputFilename { get; set; }

            /// <summary>
            /// The depth of this section in the table of contents.
            /// </summary>
            public int TOCDepth { get; set; }

            /// <summary>
            /// The relative URL to this section.
            /// </summary>
            public string Href { get; set; }

            /// <summary>
            /// The section's title.
            /// </summary>
            public string Title { get; set; }

            public override string ToString()
            {
                return "{"
                    + "InputFilename: " + InputFilename
                    + " OutputFilename: " + OutputFilename
                    + " TOCDepth: " + TOCDepth
                    + " Href: " + Href
                    + " Title: " + Title
                    + "}";
            }
        }

        /// <summary>
        /// Reads the project.xml file and converts it to a data object.
        /// </summary>
        private static Project ReadProject(string file)
        {
            var basePath = Path.GetDirectoryName(file);
            var xml = ReadProjectXml(file);

            var project = new Project();
            project.Title = (string)xml.Root.Element("title");
            project.Footer = (string)xml.Root.Element("footer");
            project.MarkdownTemplateFile = Path.Combine(basePath, (string)xml.Root.Element("template").Attribute("src"));
            project.IncludedDirectories = xml.Root.Elements("include").Select(x => Path.Combine(basePath, (string)x.Attribute("directory"))).ToList();
            project.Sections = xml.Root.Element("contents").DescendantsAndSelf().Select(x => new Section() {
                InputFilename = Path.Combine(basePath, (string)x.Attribute("src")),
                TOCDepth = TOCDepth(x)
            }).ToList();

            return project;
        }

        /// <summary>
        /// Reads the project.xml file.
        /// </summary>
        private static XDocument ReadProjectXml(string file)
        {
            CheckFileExists(file);
            using (var reader = XmlReader.Create(file))
            {
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Computes the depth of the given section in the table of contents.
        /// </summary>
        private static int TOCDepth(XElement e)
        {
            return e.AncestorsAndSelf().TakeWhile(x => x.Name != "contents").Count();
        }

        /// <summary>
        /// Reads just the title from the given Markdown-formatted input file.
        /// </summary>
        private static string ReadTitle(string file)
        {
            using (TextReader reader = File.OpenText(file))
            {
                return reader.ReadLine();
            }
        }

        /// <summary>
        /// Processes a single section.
        /// </summary>
        /// <param name="project">The project details.</param>
        /// <param name="section">The section to generate.</param>
        private static void RenderSection(Project project, Section section)
        {
            string body;

            using (TextReader reader = File.OpenText(section.InputFilename))
            {
                reader.ReadLine();  // Title, which we've already read.
                string markdown = reader.ReadToEnd();
                markdown = TranslateNodaUrls(markdown);
                markdown = markdown.Replace(TemplateTOC, MakeTOC(project.Sections));
                body = new Markdown().Transform(markdown).Replace("<pre>", "<pre class=\"prettyprint\">");
            }

            Section previousSection = project.Sections.TakeWhile(x => x != section).LastOrDefault();
            Section nextSection = project.Sections.SkipWhile(x => x != section).Skip(1).FirstOrDefault();
            string previousSectionAnchor = previousSection != null
                ? "<a href=\"" + previousSection.Href + "\">" + HttpUtility.HtmlEncode(previousSection.Title) + "</a>"
                : "";
            string nextSectionAnchor = nextSection != null
                ? "<a href=\"" + nextSection.Href + "\">" + HttpUtility.HtmlEncode(nextSection.Title) + "</a>"
                : "";

            string htmlTitle = section.TOCDepth == 0
                ? project.Title
                : string.Format("{0} - {1}", section.Title, project.Title);
            string html = project.MarkdownTemplate
                .Replace(TemplateTitle, HttpUtility.HtmlEncode(section.Title))
                .Replace(TemplateHtmlTitle, HttpUtility.HtmlEncode(htmlTitle))
                .Replace(TemplateBody, body)
                .Replace(TemplateProjectTitle, HttpUtility.HtmlEncode(project.Title))
                .Replace(TemplatePrevAnchor, previousSectionAnchor)
                .Replace(TemplateNextAnchor, nextSectionAnchor)
                .Replace(TemplateFooter, HttpUtility.HtmlEncode(project.Footer));

            File.WriteAllText(section.OutputFilename, html);
        }

        /// <summary>
        /// Translates URLs of the form noda-ns://NodaTime.Text or noda-type://NodaTime.Text.ParseResult
        /// into "real" URLs relative to the generated documentation.
        /// </summary>
        private static string TranslateNodaUrls(string markdown)
        {
            markdown = NamespacePattern.Replace(markdown, match => TranslateUrl(match, "N"));
            markdown = TypePattern.Replace(markdown, match => TranslateUrl(match, "T"));
            markdown = IssueUrlPattern.Replace(markdown, TranslateIssueUrl);
            markdown = IssueLinkPattern.Replace(markdown, TranslateIssueLink);
            return markdown;
        }
        
        private static string TranslateIssueLink(Match match)
        {
            string issue = match.Groups[1].Value;
            return "[issue " + issue + "](http://code.google.com/p/noda-time/issues/detail?id=" + issue + ")";
        }
        
        // A link where only the URL part is specified as [issue xyz]
        private static string TranslateIssueUrl(Match match)
        {
            string text = match.Groups[1].Value;
            string issue = match.Groups[2].Value;
            return text + "(http://code.google.com/p/noda-time/issues/detail?id=" + issue + ")";
        }

        private static string TranslateUrl(Match match, string memberTypePrefix)
        {
            string name = match.Groups[1].Value;
            return ApiUrlPrefix + memberTypePrefix + "_" + name.Replace(".", "_") + ".htm";
        }

        private static string MakeTOC(IList<Section> sections)
        {
            StringBuilder sb = new StringBuilder();

            int currentLevel = 0;
            foreach (Section section in sections)
            {
                if (section.TOCDepth == 0)
                {
                    continue;  // Skip the root
                }
                while (currentLevel < section.TOCDepth)
                {
                    sb.Append("<ul>\r\n");
                    currentLevel++;
                }
                while (currentLevel > section.TOCDepth)
                {
                    sb.Append("</ul>\r\n");
                    currentLevel--;
                }
                sb.Append("<li><a href=\"" + section.Href + "\">" + HttpUtility.HtmlEncode(section.Title) + "</a></li>\r\n");
            }
            while (currentLevel > 0)
            {
                sb.Append("</ul>\n");
                currentLevel--;
            }

            return sb.ToString();
        }

        private static void CheckDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Required directory " + path + " does not exist");
            }
        }

        private static void CheckFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("Required file " + path + " does not exist");
            }
        }
    }
}
