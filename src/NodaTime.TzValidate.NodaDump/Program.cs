// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.NodaDump
{
    /// <summary>
    /// Utility to convert tz source data (or an nzd file) into the tzvalidate format, so that
    /// we can validate that Noda Time handles it the same way that zic does.
    /// This is performed using the same code that TzdbCompiler does, except without using
    /// any Windows zone mappings from CLDR.
    /// </summary>
    internal class Program
    {
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.CreateWithInvariantCulture("uuuu-MM-dd HH:mm:ss'Z'");
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");

        private static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            string version;
            List<DateTimeZone> zones = LoadSource(options, out version);
            zones = zones.OrderBy(zone => zone.Id, StringComparer.Ordinal).ToList();

            if (options.ZoneId != null)
            {
                if (options.HashOnly)
                {
                    Console.WriteLine("Cannot use --hash option with a single zone ID");
                    return 1;
                }
                var zone = zones.FirstOrDefault(z => z.Id == options.ZoneId);
                if (zone == null)
                {
                    throw new Exception($"Unknown zone ID: {options.ZoneId}");
                }
                DumpZone(zone, options, Console.Out);
            }
            else
            {
                var writer = new StringWriter();
                foreach (var zone in zones)
                {
                    DumpZone(zone, options, writer);
                }
                var text = writer.ToString();
                if (options.HashOnly)
                {
                    Console.WriteLine(ComputeHash(text));
                }
                else
                {
                    WriteHeaders(text, version, options, Console.Out);
                    Console.Write(text);
                }
            }

            return 0;
        }

        private static void DumpZone(DateTimeZone zone, Options options, TextWriter writer)
        {
            writer.Write($"{zone.Id}\n");
            var initial = zone.GetZoneInterval(Instant.MinValue);
            writer.Write("Initially:           {0} {1} {2}\n",
                OffsetPattern.Format(initial.WallOffset),
                initial.Savings != Offset.Zero ? "daylight" : "standard",
                initial.Name);
            foreach (var zoneInterval in zone.GetZoneIntervals(options.Start, options.End)
                .Where(zi => zi.HasStart && zi.Start >= options.Start))
            {
                writer.Write("{0} {1} {2} {3}\n",
                    InstantPattern.Format(zoneInterval.Start),
                    OffsetPattern.Format(zoneInterval.WallOffset),
                    zoneInterval.Savings != Offset.Zero ? "daylight" : "standard",
                    zoneInterval.Name);
            }
            writer.Write("\n");
        }

        private static List<DateTimeZone> LoadSource(Options options, out string version)
        {
            var source = options.Source;
            if (source == null)
            {
                var tzdbSource = TzdbDateTimeZoneSource.Default;
                version = tzdbSource.TzdbVersion;
                return tzdbSource.GetIds().Select(id => tzdbSource.ForId(id)).ToList();
            }
            if (source.EndsWith(".nzd"))
            {
                var data = LoadFileOrUrl(source);
                var tzdbSource = TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
                version = tzdbSource.TzdbVersion;
                return tzdbSource.GetIds().Select(id => tzdbSource.ForId(id)).ToList();
            }
            else
            {
                var compiler = new TzdbZoneInfoCompiler(log: null);
                var database = compiler.Compile(source);
                version = database.Version;
                return database.GenerateDateTimeZones()
                    .Concat(database.Aliases.Keys.Select(database.GenerateDateTimeZone))
                    .ToList();
            }
        }

        private static byte[] LoadFileOrUrl(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadData(source);
                }
            }
            return File.ReadAllBytes(source);
        }
        
        private static void WriteHeaders(string text, string version, Options options, TextWriter writer)
        {
            writer.Write($"Version: {version}\n");
            writer.Write($"Body-SHA-256: {ComputeHash(text)}\n");
            writer.Write("Format: tzvalidate-0.1\n");
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
