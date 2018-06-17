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
using System.Reflection;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

namespace SnippetExtractor
{
    partial class SnippetExtractor
    {
        public static void ExtractSnippets(string solutionFile, string projectName, string outputDirectory)
        {
            ProcessSnippets(solutionFile, projectName, outputDirectory).GetAwaiter().GetResult();
        }

        private static async Task ProcessSnippets(string solutionFile, string projectName, string outputDirectory)
        {
            // Make sure snippets run in an invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // It's horrible to have this hardcoded here, but it's the simplest thing that works.
            
            var netcoreappPackage = NuGetPackageResolver
                .GetGlobalInstalledPackage("microsoft.netcore.app", version: "2.0.0");
            var netcoreappDirectory = Path.GetDirectoryName(netcoreappPackage.NotNull().FileName);
            var netcoreapp20 = (PathConstruction.AbsolutePath) netcoreappDirectory / "ref" / "netcoreapp2.0";
          
            var netcoreapp20Assemblies = Directory.GetFiles(netcoreapp20, "System*.dll")
                .Concat(Directory.GetFiles(netcoreapp20, "netstandard*.dll"))
                .Concat(Directory.GetFiles(netcoreapp20, "mscorlib*.dll")).ToArray();



            var project = await LoadProjectAsync(solutionFile, projectName, netcoreapp20Assemblies);
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

        private static async Task<Project> LoadProjectAsync(string solutionFile, string projectName,IEnumerable<string> netcoreapp20Assemblies)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFile);
            var project = solution.Projects.Single(p => p.Name == projectName);
          
            var publishDirectory = Path.Combine(Path.GetDirectoryName(project.OutputFilePath), "publish");
            var localAssemblies = Directory.GetFiles(publishDirectory, "*.dll").Where(f => Path.GetFileName(f) != "NodaTime.Demo.dll");
            var allReferences = localAssemblies.Concat(netcoreapp20Assemblies).Select(f => MetadataReference.CreateFromFile(f));
            project = project
                .WithProjectReferences(new ProjectReference[0])
                .WithMetadataReferences(allReferences);
            var compilation = await project.GetCompilationAsync();
            compilation.CheckSuccessful();
            return project;
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