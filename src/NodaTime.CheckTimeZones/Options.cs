// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.CheckTimeZones
{
    internal class Options
    {
        [Option("f", "file", HelpText = "File to load zone database from, e.g. tzdb.nzd. If unspecifed, built-in zones are used.")]
        public string File { get; set; }

        [Option(null, "from", HelpText = "'From' year: the first year to show transitions from.", DefaultValue = 1950)]
        public int FromYear { get; set; }

        [Option(null, "to", HelpText = "'To' year: the last year to show transitions from", DefaultValue = 2049)]
        public int ToYear { get; set; }

        [Option("z", "zone", HelpText = "Zone ID of the single zone to display. If unspecified, all zones are shown.")]
        public string Zone { get; set; }

        [HelpOption("?", "help", HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("NodaTime.CheckZones"))
            {
                AdditionalNewLineAfterOption = true,
            };
            help.AddOptions(this);
            return help;
        }


        internal Options()
        {
            FromYear = 1950;
            ToYear = 2050;
        }
    }
}
