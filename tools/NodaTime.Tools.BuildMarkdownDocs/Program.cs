#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownSharp;

namespace NodaTime.Tools.BuildMarkdownDocs
{
    /// <summary>
    /// Simple program to build Markdown-based documentation.
    /// </summary>
    internal sealed class Program
    {
        private const string MarkdownDirectoryName = "markdown";
        private const string RawDirectoryName = "raw";
        private const string MarkdownTemplateName = "template.html";
        private const string MarkdownSuffix = "txt";
        private const string TemplateTitle = "%TITLE%";
        private const string TemplateBody = "%BODY%";
        private const string ApiUrlPrefix = "../api/html/";
        private static readonly Regex NamespacePattern = new Regex(@"noda-ns://(\S*)", RegexOptions.Multiline);
        private static readonly Regex TypePattern = new Regex(@"noda-type://(\S*)", RegexOptions.Multiline);

        private static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: NodaTime.Tools.BuildMarkdownDocs <input directory> <output directory>");
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

        private static void BuildDocumentation(string inputDirectory, string outputDirectory)
        {
            CheckDirectoryExists(inputDirectory);
            string markdownDirectory = Path.Combine(inputDirectory, MarkdownDirectoryName);
            string rawDirectory = Path.Combine(inputDirectory, RawDirectoryName);
            CheckDirectoryExists(markdownDirectory);
            CheckDirectoryExists(rawDirectory);
            string markdownTemplateFile = Path.Combine(markdownDirectory, MarkdownTemplateName);
            CheckFileExists(markdownTemplateFile);
            string markdownTemplate = File.ReadAllText(markdownTemplateFile);

            if (Directory.Exists(outputDirectory))
            {
                Console.WriteLine("Deleting previous output directory");
                Directory.Delete(outputDirectory, true);
            }
            Directory.CreateDirectory(outputDirectory);

            // TODO: If we need subdirectories, we can handle that with repeated calls to the method below,
            // passing in the existing template file etc.
            ProcessDirectory(markdownDirectory, rawDirectory, outputDirectory, markdownTemplate);
        }

        /// <summary>
        /// Processes a single directory - does not recurse
        /// </summary>
        /// <param name="markdownDirectory">Directory in which to find markdown files</param>
        /// <param name="rawDirectory">Directory in which to find raw files to copy verbatim</param>
        /// <param name="outputDirectory">Directory to write files to</param>
        /// <param name="markdownTemplate">Markdown template text (not a filename!)</param>
        private static void ProcessDirectory(string markdownDirectory, string rawDirectory, string outputDirectory, string markdownTemplate)
        {
            foreach (string rawFile in Directory.GetFiles(rawDirectory))
            {
                File.Copy(rawFile, Path.Combine(outputDirectory, Path.GetFileName(rawFile)));
            }

            foreach (string markdownFile in Directory.GetFiles(markdownDirectory, "*." + MarkdownSuffix))
            {
                string outputFile = Path.Combine(outputDirectory, Path.ChangeExtension(Path.GetFileName(markdownFile), "html"));
                string title;
                string body;

                using (TextReader reader = File.OpenText(markdownFile))
                {
                    title = HttpUtility.HtmlEncode(reader.ReadLine());
                    string markdown = reader.ReadToEnd();
                    markdown = TranslateNodaUrls(markdown);
                    body = new Markdown().Transform(markdown).Replace("<pre>", "<pre class=\"prettyprint\">");
                }

                string html = markdownTemplate.Replace(TemplateTitle, title)
                                              .Replace(TemplateBody, body);

                File.WriteAllText(outputFile, html);
            }
        }

        /// <summary>
        /// Translates URLs of the form noda-ns://NodaTime.Text or noda-type://NodaTime.Text.ParseResult
        /// into "real" URLs relative to the generated documentation.
        /// </summary>
        private static string TranslateNodaUrls(string markdown)
        {
            markdown = NamespacePattern.Replace(markdown, match => TranslateUrl(match, "N"));
            markdown = TypePattern.Replace(markdown, match => TranslateUrl(match, "T"));
            return markdown;
        }

        private static string TranslateUrl(Match match, string memberTypePrefix)
        {
            string name = match.Groups[1].Value;
            return ApiUrlPrefix + memberTypePrefix + "_" + name.Replace(".", "_") + ".htm";
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
