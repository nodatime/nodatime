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
            if (args.Length != 3)
            {
                Console.WriteLine("Arguments: <project file> <output directory>");
                return 1;
            }
            try
            {
                ProcessSnippets(args[0], args[1], args[2]).GetAwaiter().GetResult();
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

        private static async Task ProcessSnippets(string solutionFile, string projectName, string outputDirectory)
        {
            // Make sure snippets run in an invariant culture.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            var project = await LoadProjectAsync(solutionFile, projectName);
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

        private static async Task<Project> LoadProjectAsync(string solutionFile, string projectName)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFile);
            var project = solution.Projects.Single(p => p.Name == projectName);
            // TODO: Work out why we need to do this, and fix it. This is horrible.
            if (project.MetadataReferences.Count == 0)
            {
                var nodaTimeProject = project.Solution.GetProject(project.ProjectReferences.Single().ProjectId);
                var mscorlib = nodaTimeProject.MetadataReferences.Cast<PortableExecutableReference>().Single();
                var frameworkDirectory = Path.GetDirectoryName(mscorlib.FilePath);
                string[] assemblies = { "mscorlib.dll", "System.Numerics.dll", "System.Linq.dll", "System.Collections.dll", "System.Core.dll" };
                var frameworkAssemblies = assemblies.Select(lib => MetadataReference.CreateFromFile(Path.Combine(frameworkDirectory, lib)));
                project = project.AddMetadataReferences(frameworkAssemblies);
            }
            project = project.RemoveProjectReference(project.ProjectReferences.Single());
            var outputDirectory = Path.GetDirectoryName(project.OutputFilePath);
            var libraries = Directory.GetFiles(outputDirectory, "*.dll").Select(lib => MetadataReference.CreateFromFile(lib));
            project = project.AddMetadataReferences(libraries);
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