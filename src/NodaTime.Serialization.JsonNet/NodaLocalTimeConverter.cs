using System;
using System.Globalization;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="LocalTime"/> to and from the ISO 8601 time format without a timezone specified (e.g. 12:53).
    /// </summary>
    public class NodaLocalTimeConverter : JsonConverter
    {
        private const string DefaultDateTimeFormat = "HH':'mm':'ss.FFFFFFF";

        public NodaLocalTimeConverter()
        {
            // default values
            TimeFormat = DefaultDateTimeFormat;
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets or sets the time format used when converting to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting to and from JSON.</value>
        public string TimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the culture used when converting to and from JSON.
        /// </summary>
        /// <value>The culture used when converting to and from JSON.</value>
        public CultureInfo Culture { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LocalTime) || objectType == typeof(LocalTime?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is LocalTime))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.LocalTime, got {0}.", value.GetType().FullName));

            var localTime = (LocalTime)value;

            var text = localTime.ToString(TimeFormat, Culture);
            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(LocalTime?))
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));

                return null;
            }

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing instant. Expected String, got {0}.", reader.TokenType));

            var localTimeText = reader.Value.ToString();

            if (string.IsNullOrEmpty(localTimeText) && objectType == typeof(LocalTime?))
                return null;

            return string.IsNullOrEmpty(TimeFormat)
                       ? LocalTime.Parse(localTimeText, Culture)
                       : LocalTime.ParseExact(localTimeText, TimeFormat, Culture);
        }
    }
}
