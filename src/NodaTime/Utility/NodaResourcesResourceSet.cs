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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NodaTime.Utility
{
    /// <summary>
    /// Implementation of IResourceSet for the custom NodaResources binary format.
    /// </summary>
    internal sealed class NodaResourcesResourceSet : IResourceSet
    {
        private readonly Dictionary<string, object> resources;

        private NodaResourcesResourceSet(Dictionary<string, object> resources)
        {
            this.resources = resources;
        }

        public string GetString(string name)
        {
            object value;
            if (resources.TryGetValue(name, out value))
            {
                string ret = value as string;
                if (ret == null)
                {
                    throw new InvalidOperationException("Wrong entry type");
                }
                return ret;
            }
            return null;
        }

        public object GetObject(string name)
        {
            object value;
            resources.TryGetValue(name, out value);
            return value;
        }

        internal static NodaResourcesResourceSet FromStream(Stream input)
        {
            Dictionary<string, object> resources = new Dictionary<string, object>();
            BinaryReader reader = new BinaryReader(input);
            int stringCount = reader.ReadInt32();
            for (int i = 0; i < stringCount; i++)
            {
                string name = reader.ReadString();
                string value = reader.ReadString();
                resources[name] = value;
                Debug.WriteLine("Got string resource {0}", name);
            }

            int binaryCount = reader.ReadInt32();
            for (int i = 0; i < binaryCount; i++)
            {
                string name = reader.ReadString();
                int length = reader.ReadInt32();
                byte[] value = reader.ReadBytes(length);
                resources[name] = value;
                Debug.WriteLine("Got binary resource {0}", name);
            }
            return new NodaResourcesResourceSet(resources);
        }
    }
}
