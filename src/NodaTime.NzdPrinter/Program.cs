// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.TimeZones;
using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NodaTime.Tools.Common;
using System;
using System.IO;
using System.Text;
using static NodaTime.TimeZones.IO.TzdbStreamFieldId;
using static System.FormattableString;

namespace NodaTime.NzdPrinter
{
    /// <summary>
    /// Diagnostic tool to print out the contents of an NZD file.
    /// There's a lot of code here which is basically duplicating what's in the main
    /// code base, but it's hard to avoid that.
    /// </summary>
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: NodaTime.NzdPrinter <path/url to nzd file>");
                return 1;
            }
            var stream = new MemoryStream(FileUtility.LoadFileOrUrl(args[0]));
            int version = new BinaryReader(stream).ReadInt32();
            Console.WriteLine($"File format version: {version}");
            string[] stringPool = null; // Will be populated before it's used...
            foreach (var field in TzdbStreamField.ReadFields(stream))
            {
                Console.WriteLine($"Field: {field.Id}");
                var reader = new DateTimeZoneReader(field.CreateStream(), stringPool);
                switch (field.Id)
                {
                    case StringPool:
                        stringPool = ReadStringPool(reader);
                        break;
                    case TzdbStreamFieldId.TimeZone:
                        ReadTimeZone(reader);
                        break;
                    case TzdbVersion:
                        Console.WriteLine($"TZDB version: {reader.ReadString()}");
                        break;
                    case TzdbIdMap:
                        ReadMap(reader);
                        break;
                    case CldrSupplementalWindowsZones:
                        ReadWindowsZones(reader);
                        break;
                    case WindowsAdditionalStandardNameToIdMapping:
                        ReadMap(reader);
                        break;
                    case ZoneLocations:
                        ReadZoneLocations(reader);
                        break;
                    case Zone1970Locations:
                        ReadZone1970Locations(reader);
                        break;
                }
                Console.WriteLine();
            }

            return 0;
        }

        private static string[] ReadStringPool(DateTimeZoneReader reader)
        {
            int count = reader.ReadCount();
            Console.WriteLine($"  String pool contains {count} entries");
            var stringPool = new string[count];
            for (int i = 0; i < count; i++)
            {
                stringPool[i] = reader.ReadString();
            }
            return stringPool;
        }

        private static void ReadTimeZone(DateTimeZoneReader reader)
        {
            Console.WriteLine($"  ID: {reader.ReadString()}");
            var type = (DateTimeZoneWriter.DateTimeZoneType) reader.ReadByte();
            Console.WriteLine($"  Type: {type}");
            switch (type)
            {
                case DateTimeZoneWriter.DateTimeZoneType.Fixed:
                    ReadFixedTimeZone(reader);
                    break;
                case DateTimeZoneWriter.DateTimeZoneType.Precalculated:
                    ReadPrecalculatedTimeZone(reader);
                    break;
                default:
                    Console.WriteLine("  (Unknown time zone type)");
                    break;
            }
        }

        private static void ReadFixedTimeZone(DateTimeZoneReader reader)
        {
            Console.WriteLine($"  Offset: {reader.ReadOffset()}");
            if (reader.HasMoreData)
            {
                Console.WriteLine($"  Name: {reader.ReadString()}");
            }
        }

        private static void ReadPrecalculatedTimeZone(DateTimeZoneReader reader)
        {
            int size = reader.ReadCount();
            Console.WriteLine($"  Precalculated intervals: {size}");
            var start = reader.ReadZoneIntervalTransition(null);
            for (int i = 0; i < size; i++)
            {
                var name = reader.ReadString();
                var offset = reader.ReadOffset();
                var savings = reader.ReadOffset();
                var nextStart = reader.ReadZoneIntervalTransition(start);
                Console.WriteLine(Invariant($"  {start:yyyy-MM-dd'T'HH:mm:ss} - {nextStart:yyyy-MM-dd'T'HH:mm:ss}; wall offset: {offset}; savings: {savings}; name: {name}"));
                start = nextStart;
            }
            if (reader.ReadByte() == 1)
            {
                Offset standardOffset = reader.ReadOffset();
                string standardName = reader.ReadString();
                ZoneYearOffset standardYearOffset = ZoneYearOffset.Read(reader);
                string daylightName = reader.ReadString();
                ZoneYearOffset daylightYearOffset = ZoneYearOffset.Read(reader);
                Offset savings = reader.ReadOffset();
                Console.WriteLine("  Tail zone:");
                Console.WriteLine($"    Standard time: {standardYearOffset}; offset: {standardOffset}; name: {standardName}");
                Console.WriteLine($"    Daylight time: {daylightYearOffset}; offset: {standardOffset + savings}; name: {daylightName}");
            }
            else
            {
                Console.WriteLine("  No tail zone");
            }
        }

        private static void ReadWindowsZones(DateTimeZoneReader reader)
        {
            var zones = WindowsZones.Read(reader);
            Console.WriteLine($"  Version: {zones.Version}");
            Console.WriteLine($"  TZDB version: {zones.TzdbVersion}");
            Console.WriteLine($"  Windows version: {zones.WindowsVersion}");
            Console.WriteLine($"  Mappings: {zones.MapZones.Count}");
            foreach (var mapZone in zones.MapZones)
            {
                Console.WriteLine($"  {mapZone}");
            }
        }

        private static void ReadZoneLocations(DateTimeZoneReader reader)
        {
            var count = reader.ReadCount();
            Console.WriteLine($"  Entries: {count}");
            for (int i = 0; i < count; i++)
            {
                int latitudeSeconds = reader.ReadSignedCount();
                int longitudeSeconds = reader.ReadSignedCount();
                string countryName = reader.ReadString();
                string countryCode = reader.ReadString();
                string zoneId = reader.ReadString();
                string comment = reader.ReadString();
                Console.WriteLine($"  Lat seconds: {latitudeSeconds}; long seconds: {longitudeSeconds}: country: {countryName} ({countryCode}); id: {zoneId}; comment: {comment}");
            }
        }

        private static void ReadZone1970Locations(DateTimeZoneReader reader)
        {
            var count = reader.ReadCount();
            Console.WriteLine($"  Entries: {count}");
            for (int i = 0; i < count; i++)
            {
                int latitudeSeconds = reader.ReadSignedCount();
                int longitudeSeconds = reader.ReadSignedCount();
                int countryCount = reader.ReadCount();
                StringBuilder countries = new StringBuilder();
                for (int j = 0; j < countryCount; j++)
                {
                    countries.Append($"{reader.ReadString()} ({reader.ReadString()}), ");
                }
                if (countries.Length > 0)
                {
                    // Remove the trailing comma+space.
                    countries.Length -= 2;
                }
                string zoneId = reader.ReadString();
                string comment = reader.ReadString();
                Console.WriteLine($"  Lat seconds: {latitudeSeconds}; long seconds: {longitudeSeconds}: countries: [{countries}]; id: {zoneId}; comment: {comment}");
            }
        }

        private static void ReadMap(DateTimeZoneReader reader)
        {
            int count = reader.ReadCount();
            Console.WriteLine($"  Entries: {count}");
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                Console.WriteLine($"    {key} -> {value}");
            }
        }
    }
}
