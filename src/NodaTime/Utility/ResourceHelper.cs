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
using System.Globalization;
using System.IO;
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

        internal static CultureInfo GetCulture(IFormatProvider formatProvider)
        {
            if (formatProvider is CultureInfo)
            {
                return formatProvider as CultureInfo;
            }
            return Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Returns the message string from the package resources formatted with the given replacement parameters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there is no resource string with the given id then the id is returned as the message. 
        /// </para>
        /// </remarks>
        /// <param name="id">the message id to retrieve.</param>
        /// <param name="replacements">The optional replacement parameters.</param>
        /// <returns>The formatted string.</returns>
        internal static string GetMessage(string id, params object[] replacements)
        {
            var message = Resources.ResourceManager.GetString(id) ?? id;
            return String.Format(message, replacements);
        }

        /// <summary>
        /// Returns the message string from the package resources formatted with the given replacement parameters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there is no resource string with the given id then the id is returned as the message. 
        /// </para>
        /// </remarks>
        /// <param name="formatProvider"></param>
        /// <param name="invariant"></param>
        /// <param name="id">the message id to retrieve.</param>
        /// <param name="replacements">The optional replacement parameters.</param>
        /// <returns>The formatted string.</returns>
        internal static string GetMessage(IFormatProvider formatProvider, string invariant, string id, params object[] replacements)
        {
            var culture = GetCulture(formatProvider);
            string message = culture.Equals(CultureInfo.InvariantCulture) ? invariant : (Resources.ResourceManager.GetString(id) ?? id);
            return replacements.Length == 0 ? message : String.Format(message, replacements);
        }

        /// <summary>
        /// Normalizes the given name into a valid resource name by replacing invalid
        /// characters with alternatives.
        /// </summary>
        /// <param name="name">The name to normalize.</param>
        /// <returns>The normalized name.</returns>
        internal static string NormalizeAsResourceName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
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
        /// <remarks>The default <see cref="ResourceSet"/> for a <see cref="ResourceManager"/> is the <see cref="ResourceSet"/> that is used by <see cref="ResourceManager.GetObject(string)"/>.</remarks>
        internal static ResourceSet GetDefaultResourceSet(ResourceManager manager)
        {
            return manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        }

        /// <summary>
        /// Loads a dictionary of string to string with the given name from the given resource manager.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to load from.</param>
        /// <param name="name">The resource name.</param>
        /// <returns>The <see cref="IDictionary{TKey,TValue}"/> or <c>null</c> if there is no such resource.</returns>
        internal static IDictionary<string, string> LoadDictionary(ResourceSet source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var normalizedName = NormalizeAsResourceName(name);
            var bytes = source.GetObject(normalizedName) as byte[];
            if (bytes != null)
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var reader = new DateTimeZoneCompressionReader(stream);
                    return reader.ReadDictionary();
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a time zone with the given name from the given resource manager.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to load from.</param>
        /// <param name="name">The resource name.</param>
        /// <param name="id">The time zone id for the loaded time zone.</param>
        /// <returns>The <see cref="DateTimeZone"/> or <c>null</c> if there is no such resource.</returns>
        internal static DateTimeZone LoadTimeZone(ResourceSet source, string name, string id)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var normalizedName = NormalizeAsResourceName(name);
            var bytes = source.GetObject(normalizedName) as byte[];
            if (bytes != null)
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var reader = new DateTimeZoneCompressionReader(stream);
                    return reader.ReadTimeZone(id);
                }
            }
            return null;
        }
    }
}