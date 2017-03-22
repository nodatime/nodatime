// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Newtonsoft.Json;
using System;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converter which does nothing but delegate to another one for all operations.
    /// </summary>
    /// <remarks>
    /// Nothing in this class is specific to Noda Time. Its purpose is to make it easy
    /// to reuse other converter instances with <see cref="JsonConverterAttribute"/>,
    /// which can only identify a converter by type.
    /// </remarks>
    /// <example>
    /// <para>
    /// If you had some <see cref="LocalDate"/> properties which needed one converter,
    /// but others that needed another, you might want to have different types implementing
    /// those converters. Each type would just derive from this, passing the right converter
    /// into the base constructor.
    /// </para>
    /// <code>
    /// public sealed class ShortDateConverter : DelegatingConverterBase
    /// {
    ///     public ShortDateConverter() : base(NodaConverters.LocalDateConverter) {}
    /// }
    /// </code>
    /// </example>
    public abstract class DelegatingConverterBase : JsonConverter
    {
        private readonly JsonConverter original;

        /// <summary>
        /// Constructs a converter delegating to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The converter to delegate to. Must not be null.</param>
        protected DelegatingConverterBase(JsonConverter original)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }
            this.original = original;
        }

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer) =>
            original.WriteJson(writer, value, serializer);

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            original.ReadJson(reader, objectType, existingValue, serializer);

        /// <inheritdoc />
        public override bool CanRead => original.CanRead;

        /// <inheritdoc />
        public override bool CanWrite => original.CanWrite;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => original.CanConvert(objectType);
    }
}
