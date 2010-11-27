#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.Xml;
using System.Xml.XPath;
using CommandLine;
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.winmap
{
    public class WindowsMapperCompiler
    {
        private readonly ILog log;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "WindowsMapperCompiler" /> class.
        /// </summary>
        /// <param name = "log">The log to write message to.</param>
        public WindowsMapperCompiler(ILog log)
        {
            this.log = log;
        }

        /// <summary>
        ///   Executes the specified arguments.
        /// </summary>
        /// <param name = "arguments">The arguments.</param>
        /// <returns></returns>
        internal int Execute(string[] arguments)
        {
            var options = new WindowsMapperCompilerOptions();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(log.InfoWriter));
            return parser.ParseArguments(arguments, options) ? Execute(options) : 1;
        }

        public int Execute(WindowsMapperCompilerOptions options)
        {
            log.Info("Starting compilation of {0}", options.SourceFileName);
            DateTimeZones.SetUtcOnly();
            var inputFile = new FileInfo(options.SourceFileName);
            if (!inputFile.Exists)
            {
                log.Error("Source file {0} does not exist", inputFile.FullName);
                return 2;
            }
            var mappings = ReadInput(inputFile);
            using (var output = new ResourceOutput(WindowsToPosixResource.WindowToPosixMapBase, options.OutputType))
            {
                log.Info("Compiling to {0}", output.OutputFileName);
                output.WriteDictionary(WindowsToPosixResource.WindowToPosixMapKey, mappings);
            }
            log.Info("Finished compiling.", options.SourceFileName);
            return 0;
        }

        /// <summary>
        ///   Reads the input XML file for the windows mappings.
        /// </summary>
        /// <param name = "inputFile">The input file.</param>
        /// <returns>An <see cref = "IDictionary{TKey,TValue}" /> of Windows time zone names to POSIX names.</returns>
        private static IDictionary<string, string> ReadInput(FileInfo inputFile)
        {
            var mappings = new Dictionary<string, string>();
            using (var reader = inputFile.OpenText())
            {
                // These settings allow the XML parser to ignore the DOCTYPE element
                var readerSettings = new XmlReaderSettings();
                readerSettings.XmlResolver = null;
                readerSettings.ProhibitDtd = false;
                using (var xmlReader = XmlReader.Create(reader, readerSettings))
                {
                    var document = new XPathDocument(xmlReader);
                    var navigator = document.CreateNavigator();
                    var nodes = navigator.Select("/supplementalData/timezoneData/mapTimezones[@type = 'windows']/mapZone");
                    while (nodes.MoveNext())
                    {
                        var node = nodes.Current;
                        var windowsName = node.GetAttribute("other", "");
                        var posixName = node.GetAttribute("type", "");
                        mappings.Add(windowsName, posixName);
                    }
                }
            }
            return mappings;
        }
    }
}
