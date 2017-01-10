// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.NzdCompatibility
{
    /// <summary>
    /// Class to dump time zone transitions according to the TzValidate format.
    /// </summary>
    public sealed class ZoneDumper
    {
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss'Z'");
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");
        private const string LineFormatWithAbbreviations = "{0} {1} {2} {3}\n";
        private const string LineFormatWithoutAbbreviations = "{0} {1} {2}\n";

        private readonly TzdbDateTimeZoneSource source;
        private readonly Options options;

        public ZoneDumper(TzdbDateTimeZoneSource source, Options options)
        {
            this.source = source;
            this.options = options;
        }

        public void Dump(TextWriter writer)
        {
            var zones = source.GetIds()
                .Select(source.ForId)
                .OrderBy(zone => zone.Id, StringComparer.Ordinal)
                .ToList();

            if (options.ZoneId != null)
            {
                if (options.HashOnly)
                {
                    // TODO: Maybe allow this after all, and include the zone ID in the headers?
                    throw new UserErrorException("Cannot use hash option with a single zone ID");
                }
                var zone = zones.FirstOrDefault(z => z.Id == options.ZoneId);
                if (zone == null)
                {
                    throw new UserErrorException($"Unknown zone ID: {options.ZoneId}");
                }
                DumpZone(zone, writer);
            }
            else
            {
                var bodyWriter = new StringWriter();
                foreach (var zone in zones)
                {
                    DumpZone(zone, bodyWriter);
                }
                var text = bodyWriter.ToString();
                if (options.HashOnly)
                {
                    writer.Write(ComputeHash(text) + "\n");
                }
                else
                {
                    WriteHeaders(text, writer);
                    writer.Write(text);
                }
            }

        }

        private void DumpZone(DateTimeZone zone, TextWriter writer)
        {
            string lineFormat = options.DisableAbbreviations ? LineFormatWithoutAbbreviations : LineFormatWithAbbreviations;
            writer.Write($"{zone.Id}\n");
            var initial = zone.GetZoneInterval(options.Start);
            writer.Write(lineFormat,
                "Initially:          ",
                OffsetPattern.Format(initial.WallOffset),
                initial.Savings != Offset.Zero ? "daylight" : "standard",
                initial.Name);
            foreach (var zoneInterval in zone.GetZoneIntervals(options.Start, options.End)
                                             .Where(zi => zi.Start >= options.Start))
            {
                writer.Write(lineFormat,
                    InstantPattern.Format(zoneInterval.Start),
                    OffsetPattern.Format(zoneInterval.WallOffset),
                    zoneInterval.Savings != Offset.Zero ? "daylight" : "standard",
                    zoneInterval.Name);
            }
            writer.Write("\n");
        }

        private void WriteHeaders(string text, TextWriter writer)
        {
            writer.Write($"Version: {source.TzdbVersion}\n");
            writer.Write($"Body-SHA-256: {ComputeHash(text)}\n");
            writer.Write("Format: tzvalidate-0.1\n");
            // TODO: Write up these options...
            var optionsList = new List<string>();
            if (optionsList.Count != 0)
            {
                writer.Write($"Options: {string.Join(", ", optionsList)}\n");
            }
            writer.Write($"Range: {options.FromYear ?? 1}-{options.ToYear}\n");
            writer.Write($"Generator: {typeof(Program).GetTypeInfo().Assembly.GetName().Name}\n");
            writer.Write($"GeneratorUrl: https://github.com/nodatime/nodatime\n");
            writer.Write("\n");
        }

        /// <summary>
        /// Computes the SHA-256 hash of the given text after encoding it as UTF-8,
        /// and returns the hash in lower-case hex.
        /// </summary>
        private static string ComputeHash(string text)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = hashAlgorithm.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
