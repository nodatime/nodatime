// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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

        [Option("t", "type", HelpText = "The type of the output file { ResX, Resource, NodaZoneData }. Defaults to NodaZoneData.")]
        public OutputType OutputType { get; set; }

        public CompilerOptions()
        {
            OutputFileName = "";
            SourceDirectoryName = "";
            WindowsMappingFile = "";
            OutputType = OutputType.NodaZoneData;
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
            help.AddPreOptionsLine("Usage: ZoneInfoCompiler -s <tzdb directory> -w <windowsZone.xml file> -o <output file> [-t ResX/Resource/NodaZoneData]");
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
