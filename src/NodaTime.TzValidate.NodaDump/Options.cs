// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzValidate.NodaDump
{
    /// <summary>
    /// Options for ZicDump, typically specified on the command line.
    /// </summary>
    internal sealed class Options
    {
        [Option("f", "from-year", Required = false, HelpText = "Lower bound (inclusive) to print transitions from.")]
        public int? FromYear { get; set; }

        [Option("t", "to-year", Required = false,
            HelpText = "Upper bound (exclusive) to print transitions until (defaults to 2035)", DefaultValue = 2035)]
        public int ToYear { get; set; }

        [Option("s", "source", Required = false, HelpText = "Data source - a single file or URL, or a directory")]
        public string Source { get; set; }

        [Option("z", "zone", Required = false, HelpText = "Zone ID, to dump a single time zone")]
        public string ZoneId { get; set; }

        [Option(null, "hash", Required = false, HelpText = "Only output the SHA-256 hash")]
        public bool HashOnly { get; set; }

        internal Instant Start => FromYear == null ? Instant.MinValue : Instant.FromUtc(FromYear.Value, 1, 1, 0, 0);
        internal Instant End => Instant.FromUtc(ToYear, 1, 1, 0, 0);

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("NodaDump"))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("The Noda Time Authors", 2015)
            };
            help.AddPreOptionsLine("Usage: NodaTime.TzValidate.NodaDump [-s data-source] [-f from-year] [-t to-year] [-z zone id]");
            help.AddPreOptionsLine("The data source can be:");
            help.AddPreOptionsLine("- a Noda Time .nzd file (local or web)");
            help.AddPreOptionsLine("- an IANA data release .tar.gz file (local or web)");
            help.AddPreOptionsLine("- a local directory of IANA data files");
            help.AddPreOptionsLine("If no source is provided, DateTimeZoneProviders.Tzdb will be used.");
            help.AddOptions(this);
            return help;
        }

    }
}
