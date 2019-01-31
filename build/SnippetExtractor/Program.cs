// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SnippetExtractor
{
    partial class Program
    {
        private static int Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();

            // Force System.Collections.Immutable to be deployed
            ImmutableStack.Create<int>();
            
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
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine($"Error: {e}");
                foreach (var ex in e.LoaderExceptions)
                {
                    Console.WriteLine($"Loader error: {ex}");
                }
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
                    Console.WriteLine($"Generating snippet for {snippet.Uid}");
                    var rewritten = await rewriter.RewriteSnippetAsync(snippet);
                    rewritten.Write(writer);
                    // For some reason, we run out of memory without this. Eek.
                    GC.Collect();
                }
            }
        }

        private static async Task<Project> LoadProjectAsync(string solutionFile, string projectName)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFile);
            var project = solution.Projects.Single(p => p.Name == projectName);
            // It's horrible to have this hardcoded here, but it's the simplest thing that works.
            var netcoreapp20 = @"c:\Program Files\dotnet\shared\Microsoft.NETCore.App\2.0.9";
            var netcoreapp20Assemblies = Directory.GetFiles(netcoreapp20, "System*.dll")
                .Concat(Directory.GetFiles(netcoreapp20, "netstandard*.dll"))
                .Concat(Directory.GetFiles(netcoreapp20, "mscorlib*.dll"));
            var publishDirectory = Path.Combine(Path.GetDirectoryName(project.OutputFilePath), "publish");
            var localAssemblies = Directory.GetFiles(publishDirectory, "*.dll").Where(f => Path.GetFileName(f) != "NodaTime.Demo.dll");
            var allReferences = localAssemblies.Concat(netcoreapp20Assemblies).Select(f => MetadataReference.CreateFromFile(f));
            Console.WriteLine("Compiling the project");
            project = project
                .WithProjectReferences(new ProjectReference[0])
                .WithMetadataReferences(allReferences);
            var compilation = await project.GetCompilationAsync();
            compilation.CheckSuccessful();
            Console.WriteLine("Compiled the project successfully");
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