// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace NodaTime.Tools.ProjectBuilder
{
    /// <summary>
    /// Tool to create project and solution files to support building variant configurations:
    /// .NET 4, PCL and signed releases. This is only applied to the core solution; tools
    /// do not need to be built in different configurations.
    /// </summary>
    class Program
    {
        private static readonly XNamespace MsBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        private static readonly string[] ProductionProjects = { "NodaTime", "NodaTime.Testing", "NodaTime.Serialization.JsonNet" };
        private static readonly string[] TestProjects = { "NodaTime.Benchmarks", "NodaTime.Test", "NodaTime.Serialization.Test" };

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: NodaTime.Tools.ProjectBuilder <src directory>");
                return 1;
            }
            string src = args[0];
            string coreSln = Path.Combine(src, "NodaTime-Core.sln");
            if (File.Exists(coreSln))
            {
                Console.WriteLine("Cannot find solution file: {0}", coreSln);
                return 1;
            }

            new Solution("signed", "Signed {0}")
                .WithProjects(TestProjects.Concat(ProductionProjects), SignBuild())
                .Build(src);

            new Solution("pcl", "{0} Portable")
                .WithProjects(ProductionProjects, BuildPortable(), DefineConstant("PCL"), FixJsonNetReference())
                .WithProjects(TestProjects, DefineConstant("PCL"))
                .Build(src);

            new Solution("signed-pcl", "Signed {0} Portable")
                .WithProjects(ProductionProjects, SignBuild(), BuildPortable(), DefineConstant("PCL"), FixJsonNetReference())
                .WithProjects(TestProjects, SignBuild(), DefineConstant("PCL"))
                .Build(src);

            new Solution("signed-net4", "Signed {0} Net4")
                .WithProjects(ProductionProjects, SignBuild(), SetFramework("v4.0", "Client"))
                .WithProjects(TestProjects, SignBuild())
                .Build(src);
            return 0;
        }

        static Action<XDocument> FixJsonNetReference()
        {            
            return project =>
            {
                string original = @"..\..\lib\jsonnet\Net35\Newtonsoft.Json.dll";
                string replacement = @"..\..\lib\jsonnet\Portable\Newtonsoft.Json.dll";
                foreach (var path in project.Descendants(MsBuild + "HintPath").Where(p => p.Value == original).ToList())
                {
                    path.Value = replacement;
                }
            };
        }

        static Action<XDocument> SignBuild()
        {
            Action<XDocument> signing = project => GetPropertyGroup(project).Add(
                new XElement(MsBuild + "SignAssembly", "True"),
                new XElement(MsBuild + "AssemblyOriginatorKeyFile", @"..\..\NodaTime Release.snk"));
            // Signing an assembly always means defining SIGNED, and vice versa.
            // This is not like PCL, which is defined in non-PCL builds of test code which *targets* a PCL build.
            return signing + DefineConstant("SIGNED");
        }

        static Action<XDocument> BuildPortable()
        {
            Action<XDocument> fixImport = project => project.Descendants(MsBuild + "Import")
                .Single(x => (string) x.Attribute("Project") == @"$(MSBuildToolsPath)\Microsoft.CSharp.targets")
                .SetAttributeValue("Project", @"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets");
            return fixImport + SetFramework("v4.0", "Profile328");
        }

        static XElement GetPropertyGroup(XDocument project)
        {
            return project.Root.Elements(MsBuild + "PropertyGroup").Single(p => p.Attribute("Configuration") == null);
        }

        static Action<XDocument> DefineConstant(string constant)
        {
            return project =>
            {
                foreach (var element in project.Descendants(MsBuild + "DefineConstants"))
                {
                    element.Value += ";" + constant;
                }
            };
        }

        static Action<XDocument> SetFramework(string version, string profile)
        {
            return project =>
            {
                var propertyGroup = GetPropertyGroup(project);
                propertyGroup.Element(MsBuild + "TargetFrameworkVersion").Value = version;
                propertyGroup.Element(MsBuild + "TargetFrameworkProfile").Value = profile;
            };
        }
    }
}
