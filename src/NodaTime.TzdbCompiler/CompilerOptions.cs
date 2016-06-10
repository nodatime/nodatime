// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzdbCompiler
{
    /// <summary>
    /// Defines the command line options that are valid.
    /// </summary>
    public class CompilerOptions
    {
        [Option("o", "output", Required = false, HelpText = "The name of the output file.", MutuallyExclusiveSet = "Output")]
        public string OutputFileName { get; set; }

        [Option("s", "source", Required = true, HelpText = "Source directory or archive containing the TZDB input files.")]
        public string SourceDirectoryName { get; set; } = "";

        [Option("w", "windows", Required = true, HelpText = "Windows to TZDB time zone mapping file (e.g. windowsZones.xml) or directory")]
        public string WindowsMapping { get; set; } = "";

        [Option(null, "windows-override", Required = false, HelpText = "Additional 'override' file providing extra Windows time zone mappings")]
        public string WindowsOverride { get; set; }

        [Option("z", "zone",
            Required = false,
            HelpText = "Single zone ID to compile data for, for test purposes. (Incompatible with -o.)",
            MutuallyExclusiveSet = "Output")]
        public string ZoneId { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo(typeof(CompilerOptions).Namespace))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("The Noda Time Authors", 2009)
            };
            help.AddPreOptionsLine("Usage: NodaTime.TzdbCompiler -s <tzdb directory> -w <windowsZone.xml file/dir> -o <output file> [-t ResX/Resource/NodaZoneData]");
            help.AddOptions(this);
            return help;
        }
    }
}
