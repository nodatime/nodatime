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
using System.Globalization;
using System.Resources;
using System.Threading;
using NodaTime.Properties;
using System.Diagnostics;

namespace NodaTime.Globalization
{
    /// <summary>
    ///   Defines how NodaTime values are formatted and displayed, depending on the culture.
    /// </summary>
    [DebuggerStepThrough]
    public class NodaFormatInfo : IFormatProvider
    {
        private static readonly IDictionary<String, NodaFormatInfo> Infos = new Dictionary<string, NodaFormatInfo>();
        private readonly CultureInfo cultureInfo;
        private bool isReadOnly;
        private string offsetPatternFull;
        private string offsetPatternLong;
        private string offsetPatternMedium;
        private string offsetPatternShort;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NodaFormatInfo" /> class.
        /// </summary>
        /// <param name = "cultureInfo">The culture info to base this on.</param>
        public NodaFormatInfo(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            ResourceManager manager = Resources.ResourceManager;
            offsetPatternFull = manager.GetString("OffsetPatternFull", cultureInfo);
            OffsetPatternLong = manager.GetString("OffsetPatternLong", cultureInfo);
            offsetPatternMedium = manager.GetString("OffsetPatternMedium", cultureInfo);
            offsetPatternShort = manager.GetString("OffsetPatternShort", cultureInfo);
        }

        /// <summary>
        ///   Gets the <see cref = "NodaFormatInfo" /> object for the current thread.
        /// </summary>
        public static NodaFormatInfo CurrentInfo
        {
            [DebuggerStepThrough]
            get { return GetInstance(Thread.CurrentThread.CurrentUICulture); }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { return isReadOnly; }
            [DebuggerStepThrough]
            set
            {
                if (isReadOnly && value != true)
                {
                    throw new InvalidOperationException("Cannot make a read only object writable.");
                }
                isReadOnly = value;
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "Offset" /> "F" pattern.
        /// </summary>
        /// <value>
        ///   The offset full pattern.
        /// </value>
        public string OffsetPatternFull
        {
            [DebuggerStepThrough]
            get { return offsetPatternFull; }
            [DebuggerStepThrough]
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change a read only object.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.ValueNotNull);
                }
                offsetPatternFull = value;
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "Offset" /> "L" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern long.
        /// </value>
        public string OffsetPatternLong
        {
            [DebuggerStepThrough]
            get { return offsetPatternLong; }
            [DebuggerStepThrough]
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change a read only object.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.ValueNotNull);
                }
                offsetPatternLong = value;
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "Offset" /> "M" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern medium.
        /// </value>
        public string OffsetPatternMedium
        {
            [DebuggerStepThrough]
            get { return offsetPatternMedium; }
            [DebuggerStepThrough]
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change a read only object.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.ValueNotNull);
                }
                offsetPatternMedium = value;
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "Offset" /> "S" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern short.
        /// </value>
        public string OffsetPatternShort
        {
            [DebuggerStepThrough]
            get { return offsetPatternShort; }
            [DebuggerStepThrough]
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change a read only object.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.ValueNotNull);
                }
                offsetPatternShort = value;
            }
        }

        #region IFormatProvider Members
        /// <summary>
        ///   Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name = "formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        ///   An instance of the object specified by <paramref name = "formatType" />, if the <see cref = "T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        [DebuggerStepThrough]
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(NodaFormatInfo) ? this : cultureInfo.GetFormat(formatType);
        }
        #endregion

        /// <summary>
        ///   Gets the <see cref = "NodaFormatInfo" /> for the given <see cref = "CultureInfo" />.
        /// </summary>
        /// <param name = "cultureInfo">The culture info.</param>
        /// <returns>The <see cref = "NodaFormatInfo" />. Will next be <c>null</c>.</returns>
        [DebuggerStepThrough]
        public static NodaFormatInfo GetFormatInfo(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            string name = cultureInfo.Name;
            NodaFormatInfo result;
            lock (Infos)
            {
                if (!Infos.TryGetValue(name, out result))
                {
                    result = new NodaFormatInfo(cultureInfo);
                    result.IsReadOnly = true;
                    Infos.Add(name, result);
                }
            }
            return result;
        }

        /// <summary>
        ///   Gets the <see cref = "NodaFormatInfo" /> for the given <see cref = "IFormatProvider" />. If the
        ///   format provider is <c>null</c> or if it does not provide a <see cref = "NodaFormatInfo" />
        ///   object then the format object for the current thread is returned.
        /// </summary>
        /// <param name = "provider">The <see cref = "IFormatProvider" />.</param>
        /// <returns>The <see cref = "NodaFormatInfo" />. Will next be <c>null.</c></returns>
        [DebuggerStepThrough]
        public static NodaFormatInfo GetInstance(IFormatProvider provider)
        {
            if (provider != null)
            {
                var format = provider as NodaFormatInfo;
                if (format != null)
                {
                    return format;
                }
                format = provider.GetFormat(typeof(NodaFormatInfo)) as NodaFormatInfo;
                if (format != null)
                {
                    return format;
                }
                var cultureInfo = provider as CultureInfo;
                if (cultureInfo != null)
                {
                    return GetFormatInfo(cultureInfo);
                }
            }
            return GetInstance(Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        ///   Sets the <see cref = "NodaFormatInfo" /> to use for the given culture.
        /// </summary>
        /// <param name = "cultureInfo">The culture info.</param>
        /// <param name = "formatInfo">The format info.</param>
        public static void SetFormatInfo(CultureInfo cultureInfo, NodaFormatInfo formatInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            string name = cultureInfo.Name;
            if (formatInfo == null)
            {
                lock (Infos) Infos.Remove(name);
            }
            else
            {
                formatInfo.IsReadOnly = true;
                lock (Infos) Infos[name] = formatInfo;
            }
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        [DebuggerStepThrough]
        public override string ToString()
        {
            return "NodaFormatInfo[" + cultureInfo.Name + "]";
        }
    }
}