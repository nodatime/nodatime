using System;
using System.Globalization;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="LocalDate"/> to and from the ISO 8601 date format (e.g. 2008-04-12).
    /// </summary>
    public class NodaLocalDateConverter : JsonConverter
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd";

        public NodaLocalDateConverter()
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
            return objectType == typeof(LocalDate) || objectType == typeof(LocalDate?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is LocalDate))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.LocalDate, got {0}.", value.GetType().FullName));
            
            var localDate = (LocalDate)value;

            if (localDate.Calendar.Name != "ISO")
                throw new NotSupportedException("Sorry, only the ISO calendar is currently supported for serialization.");

            var text = localDate.ToString(DateTimeFormat, Culture);
            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(LocalDate?))
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));

                return null;
            }

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing instant. Expected String, got {0}.", reader.TokenType));

            var localDateText = reader.Value.ToString();

            if (string.IsNullOrEmpty(localDateText) && objectType == typeof(LocalDate?))
                return null;

            return string.IsNullOrEmpty(DateTimeFormat)
                       ? LocalDate.Parse(localDateText, Culture)
                       : LocalDate.ParseExact(localDateText, DateTimeFormat, Culture);
        }
    }
}
