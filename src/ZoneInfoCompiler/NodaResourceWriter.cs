#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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

using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace NodaTime.ZoneInfoCompiler
{
    internal sealed class NodaResourceWriter : IResourceWriter
    {
        private readonly Stream output;
        private bool generated = false;
        private Dictionary<string, string> stringResources = new Dictionary<string, string>();
        private Dictionary<string, byte[]> binaryResources = new Dictionary<string, byte[]>();

        internal NodaResourceWriter(Stream output)
        {
            this.output = Preconditions.CheckNotNull(output, "output");
        }

        internal NodaResourceWriter(string name) : this(File.Create(name))
        {
        }

        public void AddResource(string name, byte[] value)
        {
            binaryResources[name] = value;
        }

        public void AddResource(string name, object value)
        {
            throw new NotSupportedException();
        }

        public void AddResource(string name, string value)
        {
            stringResources[name] = value;
        }

        public void Close()
        {
            if (!generated)
            {
                Generate();
            }
            output.Close();
        }

        public void Generate()
        {
            if (generated)
            {
                throw new InvalidOperationException("Resources have already been generated");
            }
            generated = true;
            var writer = new BinaryWriter(output);
            writer.Write(stringResources.Count);
            foreach (var entry in stringResources)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }
            writer.Write(binaryResources.Count);
            foreach (var entry in binaryResources)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value.Length);
                writer.Write(entry.Value);
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
