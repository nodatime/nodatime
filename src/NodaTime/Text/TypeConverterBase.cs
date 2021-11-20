// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Utility;
using System;
using System.ComponentModel;
using System.Globalization;

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
        private readonly IPattern<T> pattern;

        /// <summary>
        /// Constructs a <see cref="TypeConverter"/> for <typeparamref name="T"/> based on the provided <see cref="IPattern{T}"/>.
        /// </summary>
        /// <param name="pattern">The pattern used to parse and serialize <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/></exception>
        protected TypeConverterBase(IPattern<T> pattern) => this.pattern = Preconditions.CheckNotNull(pattern, nameof(pattern));

        /// <inheritdoc />
        [Pure]
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc />
        [Pure]
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            // The ParseResult<>.Value property will throw appropriately if the operation was unsuccessful
            value is string text ? pattern.Parse(text).Value! : base.ConvertFrom(context, culture, value);

        /// <inheritdoc />
        [Pure]
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
            destinationType == typeof(string) && value is T nodaValue
            ? pattern.Format(nodaValue)
            : base.ConvertTo(context, culture, value, destinationType);
    }
}
