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

using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    ///   Abstraction for handling the writing of objects to resources.
    /// </summary>
    public sealed class ResourceOutput : IDisposable
    {
        private static Dictionary<ResourceOutputType, string> Extensions = new Dictionary<ResourceOutputType, string>
        {
            { ResourceOutputType.Resource, "resources" },
            { ResourceOutputType.ResX, "resx" },
            { ResourceOutputType.NodaResource, "nodaresources" },
        };

        private readonly IResourceWriter resourceWriter;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ResourceOutput" /> class.
        /// </summary>
        /// <param name="name">The output file name.</param>
        /// <param name="type">The resource type.</param>
        public ResourceOutput(string name, ResourceOutputType type)
        {
            OutputFileName = Path.ChangeExtension(name, Extensions[type]);
            resourceWriter = GetResourceWriter(OutputFileName, type);
        }

        public string OutputFileName { get; private set; }

        #region IDisposable Members
        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting
        ///   unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            resourceWriter.Close();
        }
        #endregion

        /// <summary>
        ///   Returns the appropriate implementation of <see cref="IResourceWriter" /> to use to
        ///   generate the output file as directed by the command line arguments.
        /// </summary>
        /// <param name="name">The name of the output file.</param>
        /// <param name="type">The output file type.</param>
        /// <returns>The <see cref="IResourceWriter" /> to write to.</returns>
        private static IResourceWriter GetResourceWriter(string name, ResourceOutputType type)
        {
            switch (type)
            {
                case ResourceOutputType.NodaResource:
                    return new NodaResourceWriter(name);
                case ResourceOutputType.ResX:
                    return new ResourceWriter(name);
                case ResourceOutputType.Resource:
                    return new ResXResourceWriter(name);
                default:
                    throw new ArgumentException("Invalid resource type", "type");
            }
        }

        /// <summary>
        ///   Writes dictionary of string to string to  a resource with the given name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}" /> to write.</param>
        public void WriteDictionary(string name, IDictionary<string, string> dictionary)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new DateTimeZoneCompressionWriter(stream);
                writer.WriteCount(dictionary.Count);
                foreach (var entry in dictionary)
                {
                    writer.WriteString(entry.Key);
                    writer.WriteString(entry.Value);
                }
                resourceWriter.AddResource(name, stream.ToArray());
            }
        }

        public void WriteString(string name, string value)
        {
            resourceWriter.AddResource(name, value);
        }

        /// <summary>
        /// Writes a time zone to a resource with the time zone ID, normalized.
        /// </summary>
        /// <param name="timeZone">The <see cref="DateTimeZone" /> to write.</param>
        public void WriteTimeZone(DateTimeZone timeZone)
        {
            WriteResource(ResourceHelper.NormalizeAsResourceName(timeZone.Id), writer => writer.WriteTimeZone(timeZone));
        }

        private void WriteResource(string name, Action<DateTimeZoneWriter> writerAction)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new DateTimeZoneCompressionWriter(stream);
                writerAction(writer);
                resourceWriter.AddResource(name, stream.ToArray());
            }
        }
    }

    /// <summary>
    /// Defines the types of resource files we can write to.
    /// </summary>
    public enum ResourceOutputType
    {
        /// <summary>Generates the output file in ResX format.</summary>
        ResX,
        /// <summary>Generates the output file in Resource format.</summary>
        Resource,
        /// <summary>Generates the output file in custom NodaTime resource format.</summary>
        NodaResource
    }
}
