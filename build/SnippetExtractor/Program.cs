// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetExtractor
{
    partial class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Arguments: <project file> <output directory>");
                return 1;
            }
            try
            {
                ProcessSnippets(args[0], args[1]).Wait();
                return 0;
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"Error: {e.InnerException}");
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return 1;
            }
        }

        private static async Task ProcessSnippets(string projectFile, string outputDirectory)
        {
            // Make sure snippets run in an invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // TODO: Work out why it doesn't work when targeting netcoreapp1.0
            var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["TargetFramework"] = "net451" });
            var project = await workspace.OpenProjectAsync(projectFile);

            var sourceSnippets = await LoadSnippetsAsync(project);
            var rewriter = new SnippetRewriter(project);
            using (var writer = File.CreateText(Path.Combine(outputDirectory, "snippets.md")))
            {
                foreach (var snippet in sourceSnippets)
                {
                    var rewritten = await rewriter.RewriteSnippetAsync(snippet);
                    rewritten.Write(writer);
                }
            }
        }

        /// <summary>
        /// Load all the snippets from the project.
        /// </summary>
        private static async Task<IEnumerable<SourceSnippet>> LoadSnippetsAsync(Project project)
        {
            // TODO: Sort out better ways of doing async + LINQ. This must be a solved problem.
            // System.Interactive.Async perhaps?
            var snippetFileTasks = project.Documents
                .Select(doc => SnippetFileSyntaxTree.CreateAsync(doc))
                .ToList();
            var snippetFiles = await Task.WhenAll(snippetFileTasks);
            return snippetFiles.SelectMany(file => file.GetSnippets());
        }
    }
}