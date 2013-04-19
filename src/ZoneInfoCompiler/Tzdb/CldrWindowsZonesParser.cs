// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
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
        internal static WindowsZones Parse(string file)
        {
            var document = LoadFile(file);
            var mapZones = MapZones(document);
            var windowsZonesVersion = FindVersion(document);
            var tzdbVersion = document.Root.Element("windowsZones").Element("mapTimezones").Attribute("typeVersion").Value;
            var windowsVersion = document.Root.Element("windowsZones").Element("mapTimezones").Attribute("otherVersion").Value;
            return new WindowsZones(windowsZonesVersion, tzdbVersion, windowsVersion, mapZones);
        }

        private static XDocument LoadFile(string file)
        {
            // These settings allow the XML parser to ignore the DOCTYPE element
            var readerSettings = new XmlReaderSettings() { XmlResolver = null, ProhibitDtd = false };
            using (var reader = File.OpenRead(file))            
            using (var xmlReader = XmlReader.Create(reader, readerSettings))
            {
                return XDocument.Load(xmlReader);
            }
        }

        private static string FindVersion(XDocument document)
        {
            string revision = (string)document.Root.Element("version").Attribute("number");
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
        private static IList<MapZone> MapZones(XDocument document)
        {
            return document.Root.Element("windowsZones")
                .Element("mapTimezones")
                .Elements("mapZone")
                .Select(x => new MapZone(x.Attribute("other").Value,
                                         x.Attribute("territory").Value,
                                         x.Attribute("type").Value.Split(' ').ToList()))
                .ToList();
        }
    }
}
