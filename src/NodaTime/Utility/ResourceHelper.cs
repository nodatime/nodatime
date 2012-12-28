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

#if !PCL

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using NodaTime.Properties;
using NodaTime.TimeZones;

namespace NodaTime.Utility
{
    /// <summary>
    /// Provides helper methods for using resources.
    /// </summary>
    internal static class ResourceHelper
    {
        private static readonly Regex InvalidResourceNameCharacters = new Regex("[^A-Za-z0-9_/]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Normalizes the given name into a valid resource name by replacing invalid
        /// characters with alternatives.
        /// </summary>
        /// <param name="name">The name to normalize.</param>
        /// <returns>The normalized name.</returns>
        internal static string NormalizeAsResourceName(string name)
        {
            Preconditions.CheckNotNull(name, "name");
            name = name.Replace("-", "_minus_");
            name = name.Replace("+", "_plus_");
            name = name.Replace("<", "_less_");
            name = name.Replace(">", "_greater_");
            name = name.Replace("&", "_and_");
            return InvalidResourceNameCharacters.Replace(name, "_");
        }

        /// <summary>
        /// Gets the default <see cref="ResourceSet"/> from a <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="manager">The <see cref="ResourceManager"/> to get resources from.</param>
        /// <returns>The default <see cref="ResourceSet"/>.</returns>
        /// <remarks>The default <see cref="ResourceSet"/> for a <see cref="ResourceManager"/> is
        /// the <see cref="ResourceSet"/> that is used by <see cref="ResourceManager.GetObject(string)"/>.</remarks>
        internal static ResourceSet GetDefaultResourceSet(ResourceManager manager)
        {
            return manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        }

        internal static ResourceSet GetDefaultResourceSet(string baseName, Assembly assembly)
        {
            return GetDefaultResourceSet(new ResourceManager(baseName, assembly));
        }
    }
}
#endif
