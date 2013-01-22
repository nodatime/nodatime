// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Holds the mappings between Windows time zone IDs and TZDB IDs.
    /// </summary>
    internal class WindowsMapping
    {
        internal string Version { get; private set; }
        internal IDictionary<string, string> WindowsToTzdbIds { get; private set; } 

        private WindowsMapping(string version, IDictionary<string, string> mapping)
        {
            this.Version = version;
            this.WindowsToTzdbIds = mapping;
        }

        internal static WindowsMapping Parse(string file)
        {
            var document = LoadFile(file);
            var map = MapZones(document);
            Console.WriteLine("Mapped {0} Windows zones", map.Count);
            var version = FindVersion(document);
            return new WindowsMapping(version, map);
        }

        /// <summary>
        /// Reads the input XML file for the windows mappings.
        /// </summary>
        /// <returns>A list of Windows time zone mappings</returns>
        private static IDictionary<string, string> MapZones(XDocument document)
        {
            return document.Root.Element("windowsZones")
                .Element("mapTimezones")
                .Elements("mapZone")
                .Where(x => x.Attribute("territory").Value == "001") // "Default" territory
                .ToDictionary(x => x.Attribute("other").Value, x => x.Attribute("type").Value);
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
    }
}
