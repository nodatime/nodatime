using System;
using System.IO;
using System.Linq;
using NodaTime.Text;

namespace NodaTime.CheckTimeZones
{
    /// <summary>
    /// Diagnostic program to dump time zone transitions between 1900 and 2050, or compare one set
    /// with another (e.g. from Joda Time).
    /// </summary>
    /// <remarks>
    /// Currently, there's just code to dump both sets of transitions (for all zones) - you can then
    /// use a diffing tool to see the changes (or lack thereof).
    /// </remarks>
    // TODO(Post-V1): Include the ability to dump a single zone
    // TODO(Post-V1): Include the ability to parse the output of JodaDump and just validate it
    // TODO(Post-V1): Include the ability to specify a resx file or possibly even a tzdb data directory
    public class Program
    {
        private static readonly Instant Start = Instant.FromUtc(1900, 1, 1, 0,  0);
        private static readonly Instant End = Instant.FromUtc(2050, 1, 1, 0, 0);

        private static readonly InstantPattern DateTimePattern
            = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss");

        private static readonly OffsetPattern OffsetPattern
            = OffsetPattern.CreateWithInvariantCulture("+HH:mm:ss");

        private static void Main()
        {
            // Ensure it really is a culture-insensitive ordinal ordering.
            foreach (var id in DateTimeZoneProviders.Default.Ids.OrderBy(x => x, StringComparer.Ordinal))
            {
                DumpZone(DateTimeZoneProviders.Default[id], Console.Out);
            }
        }

        private static void DumpZone(DateTimeZone zone, TextWriter output)
        {
            output.WriteLine(zone.Id);

            Instant instant = Start;
            while (instant < End)
            {
                var interval = zone.GetZoneInterval(instant);
                output.WriteLine("{0}  {1}  {2}",
                    DateTimePattern.Format(instant),
                    OffsetPattern.Format(interval.StandardOffset),
                    OffsetPattern.Format(interval.Savings));
                instant = interval.End;
            }

            output.WriteLine();
        }
    }
}
