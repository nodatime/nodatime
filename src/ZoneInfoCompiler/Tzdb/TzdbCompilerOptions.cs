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
using CommandLine;
using CommandLine.Text;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Defines the command line options that are valid.
    /// </summary>
    internal class TzdbCompilerOptions
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo(AssemblyInfo.Product, AssemblyInfo.Version);

        #region Standard Option Attribute
        [Option("s", "source",
            Required = true,
            HelpText = "Source directory containing the input files.")] public string SourceDirectoryName = string.Empty;

        [Option("o", "output",
            Required = true,
            HelpText = "The name of the output file.")] public string OutputFileName = string.Empty;

        [Option("t", "type",
            HelpText = "The type of the output file { ResX, Resource }.")] public ResourceOutputType OutputType = ResourceOutputType.ResX;
        #endregion

        #region Specialized Option Attribute
        [ValueList(typeof(List<string>))] public IList<string> InputFiles;

        [HelpOption(
            HelpText = "Display this help.")]
        public string GetUsage()
        {
            var help = new HelpText(HeadingInfo);
            help.AdditionalNewLineAfterOption = true;
            help.Copyright = new CopyrightInfo("Jon Skeet", 2009);
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("Licensed under the Apache License, Version 2.0 (the \"License\");");
            help.AddPreOptionsLine("you may not use this file except in compliance with the License.");
            help.AddPreOptionsLine("You may obtain a copy of the License at");
            help.AddPreOptionsLine("      http://www.apache.org/licenses/LICENSE-2.0");
            help.AddPreOptionsLine("Unless required by applicable law or agreed to in writing, software");
            help.AddPreOptionsLine("distributed under the License is distributed on an \"AS IS\" BASIS,");
            help.AddPreOptionsLine("WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.");
            help.AddPreOptionsLine("See the License for the specific language governing permissions and");
            help.AddPreOptionsLine("limitations under the License.");
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("Usage: " + AssemblyInfo.Product + " tzdb -s source -o foo.resx");
            help.AddPreOptionsLine("       " + AssemblyInfo.Product + " tzdb -s source -o foo.resource --type Resource");

            help.AddOptions(this);

            return help;
        }
        #endregion
    }
}