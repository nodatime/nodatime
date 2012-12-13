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

#if !PCL
using System.Resources;

namespace NodaTime.Utility
{
    /// <summary>
    /// Implementation of IResourceSet which simply delegates through to a real ResourceSet
    /// </summary>
    internal sealed class BclResourceSet : IResourceSet
    {
        private readonly ResourceSet original;

        internal BclResourceSet(ResourceSet original)
        {
            this.original = Preconditions.CheckNotNull(original, "original");
        }

        public string GetString(string name)
        {
            return original.GetString(name);
        }

        public object GetObject(string name)
        {
            return original.GetObject(name);
        }
    }
}
#endif
