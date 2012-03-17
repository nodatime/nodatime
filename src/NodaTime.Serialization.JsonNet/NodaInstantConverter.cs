using System;
using System.Globalization;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="Instant"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
    /// </summary>
    public class NodaInstantConverter : JsonConverter
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFZ";

        public NodaInstantConverter()
        {
            // default values
            DateTimeFormat = DefaultDateTimeFormat;
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets or sets the date time format used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting a date to and from JSON.</value>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the culture used when converting a date to and from JSON.
        /// </summary>
        /// <value>The culture used when converting a date to and from JSON.</value>
        public CultureInfo Culture { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Instant) || objectType == typeof (Instant?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Instant))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.Instant, got {0}.", value.GetType().FullName));
            
            var instant = (Instant) value;
            var text = instant.ToString(DateTimeFormat, Culture);
            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(Instant?))
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));

                return null;
            }

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing instant. Expected String, got {0}.", reader.TokenType));

            var instantText = reader.Value.ToString();

            if (string.IsNullOrEmpty(instantText) && objectType == typeof(Instant?))
                return null;

            return string.IsNullOrEmpty(DateTimeFormat)
                       ? Instant.Parse(instantText, Culture)
                       : Instant.ParseExact(instantText, DateTimeFormat, Culture);
        }
    }
}
