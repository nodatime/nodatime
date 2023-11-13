// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Utility;
using System;
using System.ComponentModel;
using System.Globalization;
using NodaTime.Extensions;

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
        private readonly IPattern<T> defaultPattern;

        /// <summary>
        /// Constructs a <see cref="TypeConverter"/> for <typeparamref name="T"/> based on the provided <see cref="IPattern{T}"/>.
        /// </summary>
        /// <param name="defaultPattern">The pattern used to parse and serialize <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="defaultPattern"/></exception>
        protected TypeConverterBase(IPattern<T> defaultPattern) => this.defaultPattern = Preconditions.CheckNotNull(defaultPattern, nameof(defaultPattern));

        /// <summary>
        /// Auto-property to get the a deserialization pattern
        /// </summary>
        protected IPattern<T> Pattern
        {
            get
            {
                var patterns = NodaTimeCustomPatternExtensions.GetCustomPatterns<T>();
                if (patterns is null)
                {
                    // no custom patterns - use default one
                    return defaultPattern;
                }
                else
                {
                    // combine all custom patterns into a composite pattern
                    var builder = new CompositePatternBuilder<T>();
                    foreach (var p in patterns)
                    {
                        builder.Add(p, _ => true);
                    }

                    // add default pattern
                    builder.Add(defaultPattern, _ => true);

                    return builder.Build();
                }
            }
        }

        /// <inheritdoc />
        [Pure]
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc />
        [Pure]
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            // The ParseResult<>.Value property will throw appropriately if the operation was unsuccessful
            value is string text ? Pattern.Parse(text).Value! : base.ConvertFrom(context, culture, value);

        /// <inheritdoc />
        [Pure]
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
            destinationType == typeof(string) && value is T nodaValue
            ? Pattern.Format(nodaValue)
            : base.ConvertTo(context, culture, value, destinationType);
    }
}
