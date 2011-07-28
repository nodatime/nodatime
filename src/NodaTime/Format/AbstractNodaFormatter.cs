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

namespace NodaTime.Format
{
    internal abstract class AbstractNodaFormatter<T> : INodaFormatter<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractNodaFormatter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        protected AbstractNodaFormatter(IFormatProvider formatProvider)
        {
            FormatProvider = formatProvider;
        }

        #region INodaFormatter<T> Members
        /// <summary>
        /// Gets or sets the format provider use by this formatter to format values..
        /// </summary>
        /// <value>
        /// The format provider.
        /// </value>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Formats the specified value using the <see cref="IFormatProvider"/> given when the formatter
        /// was constructed. This does NOT use the current thread <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The value formatted as a string.</returns>
        public abstract string Format(T value);

        /// <summary>
        /// Returns a new copy of this formatter that uses the given <see cref="IFormatProvider"/> for
        /// formatting instead of the one that this formatter uses.
        /// </summary>
        /// <param name="formatProvider">The format provider to use.</param>
        /// <returns>A new copy of this formatter using the given <see cref="IFormatProvider"/>.</returns>
        public abstract INodaFormatter<T> WithFormatProvider(IFormatProvider formatProvider);
        #endregion
    }
}