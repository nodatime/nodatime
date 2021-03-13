// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Parses the mappings between Windows time zone IDs and TZDB IDs.
    /// </summary>
    internal class CldrWindowsZonesParser
    {
        internal static WindowsZones Parse(XDocument document)
        {
            var root = document.Root;
            if (root is null)
            {
                throw new ArgumentException("XML document has no root element");
            }
            var mapZones = MapZones(root);
            var windowsZonesVersion = FindVersion(root!);
            var tzdbVersion = root.Element("windowsZones")?.Element("mapTimezones")?.Attribute("typeVersion")?.Value ?? "";
            var windowsVersion = root.Element("windowsZones")?.Element("mapTimezones")?.Attribute("otherVersion")?.Value ?? "";
            return new WindowsZones(windowsZonesVersion, tzdbVersion, windowsVersion, mapZones);
        }

        internal static WindowsZones Parse(string file) => Parse(LoadFile(file));

        private static XDocument LoadFile(string file)
        {
            // These settings allow the XML parser to ignore the DOCTYPE element
            var readerSettings = new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore };
            using (var reader = File.OpenRead(file))            
            using (var xmlReader = XmlReader.Create(reader, readerSettings))
            {
                return XDocument.Load(xmlReader);
            }
        }

        private static string FindVersion(XElement root)
        {
            string? revision = (string?) root.Element("version")?.Attribute("number");
            if (revision is null)
            {
                return "";
            }
            string prefix = "$Revision: ";
            if (revision.StartsWith(prefix))
            {
                revision = revision.Substring(prefix.Length);
            }
            string suffix = " $";
            if (revision.EndsWith(suffix))
            {
                revision = revision.Substring(0, revision.Length - suffix.Length);
            }
            return revision;
        }

        /// <summary>
        /// Reads the input XML file for the windows mappings.
        /// </summary>
        /// <returns>A lookup of Windows time zone mappings</returns>
        private static IList<MapZone> MapZones(XElement root)
        {
            var elements = root.Element("windowsZones")?.Element("mapTimezones")?.Elements("mapZone");
            if (elements is null)
            {
                throw new ArgumentException("Missing Windows time zone XML elements");
            }
            return elements.Select(CreateMapZone).ToList();

            static MapZone CreateMapZone(XElement element)
            {
                var other = element.Attribute("other")?.Value ?? throw new ArgumentException("Missing 'other' attribute");
                var territory = element.Attribute("territory")?.Value ?? throw new ArgumentException("Missing 'territory' attribute");
                var type = element.Attribute("type")?.Value ?? throw new ArgumentException("Missing 'type' attribute");

                return new MapZone(other, territory, type.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}
