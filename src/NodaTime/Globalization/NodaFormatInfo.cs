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
#region usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NodaTime.Properties;
using NodaTime.Text;
using NodaTime.Text.Patterns;

#endregion

namespace NodaTime.Globalization
{
    /// <summary>
    ///   Defines how NodaTime values are formatted and displayed, depending on the culture.
    /// </summary>
    [DebuggerStepThrough]
    public class NodaFormatInfo : IFormatProvider, ICloneable
    {
        #region Patterns and pattern parsers
        private static readonly IPatternParser<Offset> OffsetPatternParser = new OffsetPatternParser();

        private readonly PerFormatInfoPatternCache<Offset> offsetPatternCache;
        #endregion

        /// <summary>
        /// A NodaFormatInfo wrapping the invariant culture.
        /// </summary>
        public static NodaFormatInfo InvariantInfo  = new NodaFormatInfo(CultureInfo.InvariantCulture);

        private static readonly IDictionary<String, NodaFormatInfo> Infos = new Dictionary<string, NodaFormatInfo>();
        internal static bool DisableCaching; // Used in testing and debugging

        private readonly string description;
        private DateTimeFormatInfo dateTimeFormat;
        private bool isReadOnly;
        private NumberFormatInfo numberFormat;
        private string offsetPatternFull;
        private string offsetPatternLong;
        private string offsetPatternMedium;
        private string offsetPatternShort;

        /// <summary>
        ///   Initializes a new instance of the <see cref="NodaFormatInfo" /> class.
        /// </summary>
        /// <param name="cultureInfo">The culture info to base this on.</param>
        internal NodaFormatInfo(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                cultureInfo = Thread.CurrentThread.CurrentCulture;
            }
            CultureInfo = cultureInfo;
            NumberFormat = cultureInfo.NumberFormat;
            DateTimeFormat = cultureInfo.DateTimeFormat;
            Name = cultureInfo.Name;
            description = "NodaFormatInfo[" + cultureInfo.Name + "]";
            var manager = Resources.ResourceManager;
            offsetPatternFull = manager.GetString("OffsetPatternFull", cultureInfo);
            OffsetPatternLong = manager.GetString("OffsetPatternLong", cultureInfo);
            offsetPatternMedium = manager.GetString("OffsetPatternMedium", cultureInfo);
            offsetPatternShort = manager.GetString("OffsetPatternShort", cultureInfo);
            offsetPatternCache = new PerFormatInfoPatternCache<Offset>(OffsetPatternParser, Text.OffsetPatternParser.AllFormats, Offset.Zero, this);
        }

        /// <summary>
        ///   Gets the culture info.
        /// </summary>
        public CultureInfo CultureInfo {  get;  private set; }

        internal AbstractNodaParser<Offset> OffsetParser { get { return offsetPatternCache; } }

        /// <summary>
        ///   Gets or sets the number format.
        /// </summary>
        /// <value>
        ///   The <see cref="NumberFormatInfo" />. May not be <c>null</c>.
        /// </value>
        public NumberFormatInfo NumberFormat
        {            
            get { return numberFormat; }
            
            set { SetValue(value, ref numberFormat); }
        }

        /// <summary>
        ///   Gets or sets the date time format.
        /// </summary>
        /// <value>
        ///   The <see cref="DateTimeFormatInfo" />. May not be <c>null</c>.
        /// </value>
        public DateTimeFormatInfo DateTimeFormat
        {
            
            get { return dateTimeFormat; }
            
            set { SetValue(value, ref dateTimeFormat); }
        }

        /// <summary>
        ///   Gets the decimal separator.
        /// </summary>
        public string DecimalSeparator
        {            
            get { return NumberFormat.NumberDecimalSeparator; }
        }

        /// <summary>
        /// Name of the culture providing this formatting information.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the positive sign.
        /// </summary>
        public string PositiveSign
        {
            
            get { return NumberFormat.PositiveSign; }
        }

        /// <summary>
        ///   Gets the negative sign.
        /// </summary>
        public string NegativeSign
        {
            
            get { return NumberFormat.NegativeSign; }
        }

        /// <summary>
        ///   Gets the time separator.
        /// </summary>
        public string TimeSeparator
        {
            
            get { return DateTimeFormat.TimeSeparator; }
        }

        /// <summary>
        ///   Gets the date separator.
        /// </summary>
        public string DateSeparator
        {
            
            get { return DateTimeFormat.DateSeparator; }
        }

        /// <summary>
        ///   Gets the <see cref="NodaFormatInfo" /> object for the current thread.
        /// </summary>
        public static NodaFormatInfo CurrentInfo
        {
            get { return GetInstance(Thread.CurrentThread.CurrentCulture); }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            
            get { return isReadOnly; }
            
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
        ///   Gets or sets the <see cref="Offset" /> "F" pattern.
        /// </summary>
        /// <value>
        ///   The offset full pattern.
        /// </value>
        public string OffsetPatternFull
        {
            
            get { return offsetPatternFull; }
            
            set { SetValue(value, ref offsetPatternFull); }
        }

        /// <summary>
        ///   Gets or sets the <see cref="Offset" /> "L" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern long.
        /// </value>
        public string OffsetPatternLong
        {            
            get { return offsetPatternLong; }
            
            set { SetValue(value, ref offsetPatternLong); }
        }

        /// <summary>
        ///   Gets or sets the <see cref="Offset" /> "M" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern medium.
        /// </value>
        public string OffsetPatternMedium
        {
            
            get { return offsetPatternMedium; }
            
            set { SetValue(value, ref offsetPatternMedium); }
        }

        /// <summary>
        ///   Gets or sets the <see cref="Offset" /> "S" pattern.
        /// </summary>
        /// <value>
        ///   The offset pattern short.
        /// </value>
        public string OffsetPatternShort
        {
            
            get { return offsetPatternShort; }
            
            set { SetValue(value, ref offsetPatternShort); }
        }

        #region ICloneable Members
        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        
        public object Clone()
        {
            var info = (NodaFormatInfo)MemberwiseClone();
            info.isReadOnly = false;
            return info;
        }
        #endregion

        #region IFormatProvider Members
        /// <summary>
        ///   Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        ///   An instance of the object specified by <paramref name = "formatType" />, if the <see cref="T:System.IFormatProvider" />
        ///   implementation can supply that type of object; otherwise, null.
        /// </returns>
        
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(NodaFormatInfo))
            {
                return this;
            }
            if (formatType == typeof(NumberFormatInfo))
            {
                return NumberFormat;
            }
            if (formatType == typeof(DateTimeFormatInfo))
            {
                return DateTimeFormat;
            }
            return null;
        }
        #endregion

        /// <summary>
        ///   Clears the cache.
        /// </summary>
        internal static void ClearCache()
        {
            lock (Infos) Infos.Clear();
        }

        private void SetValue<T>(T value, ref T property)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(Resources.Noda_CannotChangeReadOnly);
            }
            if (value == null)
            {
                throw new ArgumentNullException("value", Resources.Noda_ArgumentNull);
            }
            property = value;
        }

        /// <summary>
        ///   Gets the <see cref="NodaFormatInfo" /> for the given <see cref="CultureInfo" />.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The <see cref="NodaFormatInfo" />. Will next be <c>null</c>.</returns>
        internal static NodaFormatInfo GetFormatInfo(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            string name = cultureInfo.Name;
            NodaFormatInfo result;
            if (cultureInfo == CultureInfo.InvariantCulture)
            {
                return InvariantInfo;
            }
            lock (Infos)
            {
                // TODO: Consider fetching by the cultureInfo instead, as otherwise two culture instances
                // with the same name will give the wrong result.
                if (DisableCaching || !Infos.TryGetValue(name, out result))
                {
                    result = new NodaFormatInfo(cultureInfo) { IsReadOnly = true };
                    if (!DisableCaching)
                    {
                        Infos.Add(name, result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///   Gets the <see cref="NodaFormatInfo" /> for the given <see cref="IFormatProvider" />. If the
        ///   format provider is <c>null</c> or if it does not provide a <see cref="NodaFormatInfo" />
        ///   object then the format object for the current thread is returned.
        /// </summary>
        /// <param name="provider">The <see cref="IFormatProvider" />.</param>
        /// <returns>The <see cref="NodaFormatInfo" />. Will next be <c>null.</c></returns>
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
            return GetInstance(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///   Sets the <see cref="NodaFormatInfo" /> to use for the given culture.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="formatInfo">The format info.</param>
        internal static void SetFormatInfo(CultureInfo cultureInfo, NodaFormatInfo formatInfo)
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
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        
        public override string ToString()
        {
            return description;
        }
    }
}