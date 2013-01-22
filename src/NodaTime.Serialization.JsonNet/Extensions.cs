using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Static class containing extension methods to configure Json.NET for Noda Time types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings, IDateTimeZoneProvider provider)
        {
            // add our converters
            settings.Converters.Add(NodaConverters.InstantConverter);
            settings.Converters.Add(NodaConverters.IntervalConverter);
            settings.Converters.Add(NodaConverters.LocalDateConverter);
            settings.Converters.Add(NodaConverters.LocalDateTimeConverter);
            settings.Converters.Add(NodaConverters.LocalTimeConverter);
            settings.Converters.Add(NodaConverters.OffsetConverter);
            settings.Converters.Add(new NodaDateTimeZoneConverter(provider));
            settings.Converters.Add(NodaConverters.DurationConverter);
            settings.Converters.Add(NodaConverters.RoundtripPeriodConverter);

            // return to allow fluent chaining if desired
            return settings;
        }

        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializer ConfigureForNodaTime(this JsonSerializer serializer, IDateTimeZoneProvider provider)
        {
            // add our converters
            serializer.Converters.Add(NodaConverters.InstantConverter);
            serializer.Converters.Add(NodaConverters.IntervalConverter);
            serializer.Converters.Add(NodaConverters.LocalDateConverter);
            serializer.Converters.Add(NodaConverters.LocalDateTimeConverter);
            serializer.Converters.Add(NodaConverters.LocalTimeConverter);
            serializer.Converters.Add(NodaConverters.OffsetConverter);
            serializer.Converters.Add(new NodaDateTimeZoneConverter(provider));
            serializer.Converters.Add(NodaConverters.DurationConverter);
            serializer.Converters.Add(NodaConverters.RoundtripPeriodConverter);

            // return to allow fluent chaining if desired
            return serializer;
        }
    }
}
