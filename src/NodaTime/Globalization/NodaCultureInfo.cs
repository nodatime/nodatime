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
using System.Globalization;
using System.Threading;

namespace NodaTime.Globalization
{
    /// <summary>
    ///   Provides a <see cref = "CultureInfo" /> that supports NodaTime formatting information. This allows
    ///   for the <see cref = "NodaFormatInfo" /> data to be set into the <see cref = "Thread.CurrentUICulture" />
    ///   so it does not have to be passed around. This object defers all unrecognized requests to the
    ///   underlying culture info so it acts like a wrapper around a system culture info.
    /// </summary>
    public class NodaCultureInfo : CultureInfo
    {
        private static NodaCultureInfo invariantCulture;
        private NodaFormatInfo formatInfo;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NodaCultureInfo" /> class.
        /// </summary>
        /// <param name = "name">The name of the base culture.</param>
        public NodaCultureInfo(string name) : base(name)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NodaCultureInfo" /> class.
        /// </summary>
        /// <param name = "name">The name of the base culture.</param>
        /// <param name = "useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false).</param>
        public NodaCultureInfo(string name, bool useUserOverride) : base(name, useUserOverride)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NodaCultureInfo" /> class.
        /// </summary>
        /// <param name = "culture">A predefined <see cref = "T:System.Globalization.CultureInfo" /> identifier, <see cref = "P:System.Globalization.CultureInfo.LCID" /> property of an existing <see cref = "T:System.Globalization.CultureInfo" /> object, or Windows-only culture identifier.</param>
        public NodaCultureInfo(int culture) : base(culture)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NodaCultureInfo" /> class.
        /// </summary>
        /// <param name = "culture">A predefined <see cref = "T:System.Globalization.CultureInfo" /> identifier, <see cref = "P:System.Globalization.CultureInfo.LCID" /> property of an existing <see cref = "T:System.Globalization.CultureInfo" /> object, or Windows-only culture identifier.</param>
        /// <param name = "useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (true) or the default culture settings (false).</param>
        /// <exception cref = "T:System.ArgumentOutOfRangeException">
        ///   <paramref name = "culture" /> is less than zero.
        /// </exception>
        /// <exception cref = "T:System.ArgumentException">
        ///   <paramref name = "culture" /> is not a valid culture identifier.
        ///   -or-
        ///   In .NET Compact Framework applications, <paramref name = "culture" /> is not supported by the operating system of the device.
        /// </exception>
        public NodaCultureInfo(int culture, bool useUserOverride) : base(culture, useUserOverride)
        {
        }

        /// <summary>
        ///   Gets the invariant culture.
        /// </summary>
        /// <value>The invariant culture.</value>
        public new static NodaCultureInfo InvariantCulture
        {
            get
            {
                if (invariantCulture == null)
                {
                    invariantCulture = new NodaCultureInfo(0x007f, false);
                }
                return invariantCulture;
            }
        }

        /// <summary>
        ///   Gets or sets the noda format info of this object.
        /// </summary>
        /// <value>
        ///   The <see cref = "NodaFormatInfo" /> value. This will never be <c>null</c>.
        /// </value>
        public NodaFormatInfo NodaFormatInfo
        {
            get
            {
                if (formatInfo == null)
                {
                    formatInfo = NodaFormatInfo.GetFormatInfo(this);
                }
                return formatInfo;
            }
            set { formatInfo = value; }
        }

        /// <summary>
        ///   Gets the <see cref = "T:System.Globalization.CultureInfo" /> that represents the parent culture of the current
        ///   <see cref = "T:System.Globalization.CultureInfo" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///   The <see cref = "T:System.Globalization.CultureInfo" /> that represents the parent culture of the current
        ///   <see cref = "T:System.Globalization.CultureInfo" />.
        /// </returns>
        public new NodaCultureInfo Parent
        {
            get
            {
                CultureInfo parent = base.Parent;
                if (CultureInfo.InvariantCulture.LCID == parent.LCID)
                {
                    return InvariantCulture;
                }
                return new NodaCultureInfo(parent.Name, UseUserOverride);
            }
        }

        /// <summary>
        ///   Gets an object that defines how to format the specified type.
        /// </summary>
        /// <param name = "formatType">The <see cref = "T:System.Type" /> for which to get a formatting object. This method only supports the <see cref = "T:System.Globalization.NumberFormatInfo" /> and <see cref = "T:System.Globalization.DateTimeFormatInfo" /> types.</param>
        /// <returns>
        ///   The value of the <see cref = "P:System.Globalization.CultureInfo.NumberFormat" /> property, which is a <see cref = "T:System.Globalization.NumberFormatInfo" /> containing the default number format information for the current <see cref = "T:System.Globalization.CultureInfo" />, if <paramref name = "formatType" /> is the <see cref = "T:System.Type" /> object for the <see cref = "T:System.Globalization.NumberFormatInfo" /> class.
        ///   -or-
        ///   The value of the <see cref = "P:System.Globalization.CultureInfo.DateTimeFormat" /> property, which is a <see cref = "T:System.Globalization.DateTimeFormatInfo" /> containing the default date and time format information for the current <see cref = "T:System.Globalization.CultureInfo" />, if <paramref name = "formatType" /> is the <see cref = "T:System.Type" /> object for the <see cref = "T:System.Globalization.DateTimeFormatInfo" /> class.
        ///   -or-
        ///   null, if <paramref name = "formatType" /> is any other object.
        /// </returns>
        public override object GetFormat(Type formatType)
        {
            if (formatType == typeof(NodaFormatInfo))
            {
                return NodaFormatInfo;
            }
            return base.GetFormat(formatType);
        }
    }
}