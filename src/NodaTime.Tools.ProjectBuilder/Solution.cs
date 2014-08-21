// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace NodaTime.Tools.ProjectBuilder
{
    internal sealed class Solution
    {
        private static readonly XNamespace MsBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        private readonly string suffix;
        private readonly string outputPathFormat;
        private readonly List<ProjectCollection> projectCollections = new List<ProjectCollection>();

        internal Solution(string suffix, string outputPathFormat)
        {
            this.suffix = suffix;
            this.outputPathFormat = outputPathFormat;
        }

        internal Solution WithProjects(IEnumerable<string> projects, params Action<XDocument>[] transformations)
        {
            projectCollections.Add(new ProjectCollection { Projects = projects.ToList(), Transformations = transformations });
            return this;
        }

        internal void Build(string directory)
        {
            string currentSln = Path.Combine(directory, "NodaTime-Core.sln");
            string newSln = Path.Combine(directory, "NodaTime-Core-" + directory + ".sln");
            string fullSuffix = "-" + suffix + ".csproj";
            File.WriteAllLines(newSln, File.ReadLines(currentSln).Select(x => x.Replace(".csproj", fullSuffix)));

            foreach (var collection in projectCollections)
            {
                foreach (var project in collection.Projects)
                {
                    XDocument document = XDocument.Load(Path.Combine(directory, project, project + ".csproj"));
                    foreach (var transformation in collection.Transformations)
                    {
                        transformation(document);
                    }
                    SetOutputDirectory(document, "Debug");
                    SetOutputDirectory(document, "Release");
                    SetDocumentationationFile(document);
                    FixProjectReferences(document);
                    document.Save(Path.Combine(directory, project, project + "-" + suffix + ".csproj"));
                }
            }
        }

        private void FixProjectReferences(XDocument document)
        {
            foreach (var reference in document.Descendants(MsBuild + "ProjectReference"))
            {
                var include = reference.Attribute("Include");
                var bits = include.Value.Split('\\');
                string lastPart = bits.Last();
                if (lastPart.StartsWith("NodaTime"))
                {
                    bits[bits.Length - 1] = lastPart.Replace(".csproj", "-" + suffix + ".csproj");
                    include.Value = string.Join("\\", bits);
                }
            }
        }

        private void SetOutputDirectory(XDocument document, string configuration)
        {
            string original = @"bin\" + configuration + @"\";
            string replacement = @"bin\" + string.Format(outputPathFormat, configuration) + @"\";
            document.Descendants(MsBuild + "OutputPath").Single(x => x.Value == @"bin\" + original + @"\").Value = @"bin\" + replacement + @"\";
        }

        private void SetDocumentationationFile(XDocument document)
        {
            string original = @"bin\Release\";
            string replacement = @"bin\" + string.Format(outputPathFormat, "Release") + @"\";
            foreach (var item in document.Descendants(MsBuild + "DocumentationFile").Where(x => x.Value.StartsWith(original)).ToList())
            {
                item.Value = replacement + item.Value.Substring(original.Length);
            }
        }

        private class ProjectCollection
        {
            internal List<string> Projects { get; set; }
            internal Action<XDocument>[] Transformations { get; set; }
        }
    }
}
