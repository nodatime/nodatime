// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzValidate.NzdCompatibility
{
    /// <summary>
    /// Options for TzValidate dumpers, typically specified on the command line.
    /// Note that this class unfortunately mixes "pure" dumper options (from/to year, whether
    /// or not to include abbreviations etc) with "tool" options (source to load from, output file).
    /// This may be corrected in the future.
    /// </summary>
    public sealed class Options
    {
        [Option("f", "from-year", Required = false, HelpText = "Lower bound (inclusive) to print transitions from.")]
        public int? FromYear { get; set; }

        [Option("t", "to-year", Required = false,
            HelpText = "Upper bound (exclusive) to print transitions until (defaults to 2035)", DefaultValue = 2035)]
        public int ToYear { get; set; }

        [Option("s", "source", Required = true, HelpText = "Data source - a single file or URL; must be an NZD file")]
        public string Source { get; set; }

        [Option("z", "zone", Required = false, HelpText = "Zone ID, to dump a single time zone")]
        public string ZoneId { get; set; }

        [Option("o", "output", Required = false, HelpText = "Output file (defaults to writing to the console")]
        public string OutputFile { get; set; }

        [Option(null, "hash", Required = false, HelpText = "Only output the SHA-256 hash")]
        public bool HashOnly { get; set; }

        [Option(null, "noabbr", Required = false, HelpText = "Disable output of abbreviations")]
        public bool DisableAbbreviations { get; set; }

        internal Instant Start => FromYear == null ? Instant.FromUtc(1, 1, 1, 0, 0) : Instant.FromUtc(FromYear.Value, 1, 1, 0, 0);
        internal Instant End => Instant.FromUtc(ToYear, 1, 1, 0, 0);

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("NzdCompatibility"))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("The Noda Time Authors", 2017)
            };
            help.AddPreOptionsLine("Usage: NodaTime.TzValidate.NzdCompatibility [-s data-source] [-f from-year] [-t to-year] [-z zone id] [-o output] [-hash] [-noabbr]");
            help.AddPreOptionsLine("The data source must be a Noda Time .nzd file (local or web)");
            help.AddOptions(this);
            return help;
        }

    }
}
