#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.winmap
{
    public class WindowsMapperCompiler
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMapperCompiler" /> class.
        /// </summary>
        /// <param name="log">The log to write message to.</param>
        public WindowsMapperCompiler(ILog log)
        {
            this.log = log;
        }

        public int Execute(string inputFileName, ResourceOutput output)
        {
            log.Info("Starting compilation of {0}", inputFileName);
            var inputFile = new FileInfo(inputFileName);
            if (!inputFile.Exists)
            {
                log.Error("Source file {0} does not exist", inputFile.FullName);
                return 2;
            }
            var document = LoadFile(inputFile);
            var mappings = MapZones(document);
            log.Info("Mapped {0} zones in total.", mappings.Count);
            output.WriteDictionary(TzdbTimeZoneProvider.WindowsToPosixMapKey, mappings);
            var version = FindVersion(document);
            output.WriteString(TzdbTimeZoneProvider.WindowsToPosixMapVersionKey, version);
            log.Info("Finished compiling.", inputFileName);
            return 0;
        }

        /// <summary>
        /// Reads the input XML file for the windows mappings.
        /// </summary>
        /// <returns>An <see cref="IDictionary{TKey,TValue}" /> of Windows time zone names to POSIX names.</returns>
        private IDictionary<string, string> MapZones(XDocument document)
        {
            return document.Root.Element("windowsZones")
                .Element("mapTimezones")
                .Elements("mapZone")
                .ToDictionary(x => x.Attribute("other").Value, x => x.Attribute("type").Value);
        }

        private string FindVersion(XDocument document)
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

        private XDocument LoadFile(FileInfo inputFile)
        {
            using (var reader = inputFile.OpenText())
            {
                // These settings allow the XML parser to ignore the DOCTYPE element
                var readerSettings = new XmlReaderSettings();
                readerSettings.XmlResolver = null;
                readerSettings.ProhibitDtd = false;
                using (var xmlReader = XmlReader.Create(reader, readerSettings))
                {
                    return XDocument.Load(xmlReader);
                }
            }
        }
    }
}
