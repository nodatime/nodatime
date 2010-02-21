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
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Abstraction for handling the writing of objects to resources.
    /// </summary>
    public class ResourceOutput
        : IDisposable
    {
        private readonly IResourceWriter resourceWriter;
        private readonly MemoryStream memory;
        private readonly DateTimeZoneWriter timeZoneWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOutput"/> class.
        /// </summary>
        /// <param name="name">The output file name.</param>
        /// <param name="type">The resource type.</param>
        public ResourceOutput(string name, ResourceOutputType type)
        {
            string fileName = ResourceOutput.ChangeExtension(name, type);
            this.resourceWriter = GetResourceWriter(fileName, type);
            this.memory = new MemoryStream();
            this.timeZoneWriter = new DateTimeZoneWriter(this.memory);
        }

        /// <summary>
        /// Returns the givne file name with the extension set based on the given resource output type.
        /// </summary>
        /// <param name="fileName">The file name to change.</param>
        /// <param name="type">The <see cref="ResourceOutputType"/>.</param>
        /// <returns>The file extension to use.</returns>
        public static string ChangeExtension(string fileName, ResourceOutputType type)
        {
            if (type == ResourceOutputType.Resource)
            {
                return Path.ChangeExtension(fileName, ".resources");
            }
            else
            {
                return Path.ChangeExtension(fileName, ".resx");
            }
        }

        /// <summary>
        /// Writes a time zone to a resource with the given name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="timeZone">The <see cref="IDateTimeZone"/> to write.</param>
        public void WriteTimeZone(string name, IDateTimeZone timeZone)
        {
            this.timeZoneWriter.WriteTimeZone(timeZone);
            WriteResource(name);
        }

        /// <summary>
        /// Writes dictionary of string to string to  a resource with the given name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="dictionary">The <see cref="IDictionary"/> to write.</param>
        public void WriteDictionary(string name, IDictionary<string, string> dictionary)
        {
            this.timeZoneWriter.WriteDictionary(dictionary);
            WriteResource(name);
        }

        /// <summary>
        /// Returns the appropriate implementation of <see cref="IResourceWriter"/> to use to
        /// generate the output file as directed by the command line arguments.
        /// </summary>
        /// <param name="name">The name of the output file.</param>
        /// <param name="type">The output file type.</param>
        /// <returns>The <see cref="IResourceWriter"/> to write to.</returns>
        private IResourceWriter GetResourceWriter(string name, ResourceOutputType type)
        {
            IResourceWriter result;
            if (type == ResourceOutputType.Resource)
            {
                result = new ResourceWriter(name);
            }
            else
            {
                result = new ResXResourceWriter(name);
            }
            return result;
        }

        /// <summary>
        /// Writes contents of this object's memory stream to the resource writer.
        /// </summary>
        /// <param name="name">The name of the resource to write.</param>
        private void WriteResource(string name)
        {
            this.memory.Flush();
            byte[] bytes = this.memory.ToArray();
            string normalizedName = ResourceHelper.NormalizeAsResourceName(name);
            this.resourceWriter.AddResource(normalizedName, bytes);
            this.memory.SetLength(0);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.resourceWriter.Close();
        }

        #endregion
    }

    /// <summary>
    /// Defines the types of resource files we can write to.
    /// </summary>
    public enum ResourceOutputType
    {
        /// <summary>
        /// Generates the output file in ResX format.
        /// </summary>
        ResX,
        /// <summary>
        /// generates the output file in Resource format.
        /// </summary>
        Resource
    }
}
