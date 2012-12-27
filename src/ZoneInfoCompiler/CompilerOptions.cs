#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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

using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Defines the command line options that are valid.
    /// </summary>
    public class CompilerOptions
    {
        [Option("o", "output", Required = true, HelpText = "The name of the output file.")]
        public string OutputFileName { get; set; }

        [Option("s", "source", Required = true, HelpText = "Source directory containing the TZDB input files.")]
        public string SourceDirectoryName { get; set; }

        [Option("w", "windows", Required = true, HelpText = "Windows to TZDB time zone mapping file (e.g. windowsZones.xml)")]
        public string WindowsMappingFile { get; set; }

        [Option("t", "type", HelpText = "The type of the output file { ResX, Resource, NodaResource }. Defaults to Resx.")]
        public OutputType OutputType { get; set; }

        public CompilerOptions()
        {
            OutputFileName = "";
            SourceDirectoryName = "";
            WindowsMappingFile = "";
            OutputType = OutputType.ResX;
        }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            string productName = FetchProductName();
            var help = new HelpText(new HeadingInfo(productName))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("Jon Skeet", 2009, 2013)
            };
            help.AddPreOptionsLine("Usage: ZoneInfoCompiler -s <tzdb directory> -w <windowsZone.xml file> -o <output file> [-t ResX/Resource/NodaResource]");
            help.AddOptions(this);
            return help;
        }

        private static string FetchProductName()
        {
            var program = Assembly.GetEntryAssembly();
            if (program != null)
            {
                var attributes = program.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }
            return "Product";
        }
    }
}
