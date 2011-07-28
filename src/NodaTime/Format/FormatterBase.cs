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
using System.Threading;

namespace NodaTime.Format
{
    /// <summary>
    ///   Defines the abstract base for all formatter objects.
    /// </summary>
    /// <typeparam name = "T">The type that this formatter formats.</typeparam>
    internal abstract class FormatterBase<T>
    {
        /// <summary>
        ///   Formats the specified value using the given format provider.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <param name = "formatProvider">The format provider to use. If <c>null</c> the
        ///   <see cref = "P:System.Threading.Thread.CurrentCulture" /> value is used.</param>
        /// <returns>The formatted string.</returns>
        internal string Format(T value, IFormatProvider formatProvider)
        {
            return FormatValue(value, formatProvider ?? Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        ///   Overridden in subclasses to provides the actual formatting implementation.
        /// </summary>
        /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
        /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
        /// <returns>The formatted string.</returns>
        protected abstract string FormatValue(T value, IFormatProvider formatProvider);
    }
}