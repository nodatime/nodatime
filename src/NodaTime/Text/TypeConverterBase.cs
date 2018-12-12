// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides conversion and parsing for <typeparamref name="T"/>.
    /// </summary>
    internal abstract class TypeConverterBase<T> : TypeConverter
    {
        /// <summary>
        /// The pattern used to parse and serialize values of <typeparamref name="T"/>.
        /// </summary>
        [NotNull] private readonly IPattern<T> _pattern;

        /// <summary>
        /// Constructs a <see cref="TypeConverter"/> for <typeparamref name="T"/> based on the provided <see cref="IPattern{T}"/>.
        /// </summary>
        /// <param name="pattern">The pattern used to parse and serialize <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/></exception>
        protected TypeConverterBase([NotNull] IPattern<T> pattern)
            => _pattern = Preconditions.CheckNotNull(pattern, nameof(pattern));

        /// <inheritdoc />
        [Pure]
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        [Pure]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string instantString)
            {
                var parseResult = _pattern.Parse(instantString);

                if (parseResult.Success)
                {
                    return parseResult.Value;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc />
        [Pure]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is T t)
            {
                return _pattern.Format(t);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}