using System;
using System.Collections.Generic;
using NodaTime.Text;

namespace NodaTime.Serialization.ModelBinding
{
    public class NodaModelBinderResolver
    {
        private readonly IDateTimeZoneProvider _provider;
        private readonly Dictionary<Type, object> _dictionary;

        private bool _useIsoIntervalModelBinder;

        public bool UseIsoIntervalModelBinder
        {
            get
            {
                return _useIsoIntervalModelBinder;
            }
            set
            {
                _useIsoIntervalModelBinder = value;
                _dictionary.Remove(typeof(Interval));
            }
        }

        public NodaModelBinderResolver(IDateTimeZoneProvider provider)
        {
            _provider = provider;
            _dictionary = new Dictionary<Type, object>();
        }

        public object GetCachedModelBinder(Type type)
        {
            if (_dictionary.ContainsKey(type))
            {
                return _dictionary[type];
            }

            var modelBinder = GetNewModelBinder(type);
            if (modelBinder != null)
            {
                _dictionary[type] = modelBinder;
            }

            return modelBinder;
        }

        public object GetNewModelBinder(Type type)
        {
            if (type == typeof(Instant))
            {
                return new NodaModelBinder<Instant>(InstantPattern.ExtendedIsoPattern);
            }

            if (type == typeof(Interval))
            {
                if (UseIsoIntervalModelBinder)
                {
                    return new NodaIsoIntervalModelBinder();
                }
                return new NodaIntervalModelBinder();
            }

            if (type == typeof(LocalDate))
            {
                return new NodaModelBinder<LocalDate>(LocalDatePattern.IsoPattern);
            }

            if (type == typeof(LocalDateTime))
            {
                return new NodaModelBinder<LocalDateTime>(LocalDateTimePattern.ExtendedIsoPattern);
            }

            if (type == typeof(LocalTime))
            {
                return new NodaModelBinder<LocalTime>(LocalTimePattern.ExtendedIsoPattern);
            }

            if (type == typeof(Offset))
            {
                return new NodaModelBinder<Offset>(OffsetPattern.GeneralInvariantPattern);
            }

            if (type == typeof(DateTimeZone))
            {
                return new NodaDateTimeZoneModelBinder(_provider);
            }

            if (type == typeof(Duration))
            {
                return new NodaModelBinder<Duration>(DurationPattern.CreateWithInvariantCulture(DurationPattern.RoundtripPattern.PatternText));
            }

            if (type == typeof(Period))
            {
                return new NodaModelBinder<Period>(PeriodPattern.RoundtripPattern);
            }

            if (type == typeof(OffsetDateTime))
            {
                return new NodaModelBinder<OffsetDateTime>(OffsetDateTimePattern.ExtendedIsoPattern);
            }

            if (type == typeof(ZonedDateTime))
            {
                return new NodaModelBinder<ZonedDateTime>(ZonedDateTimePattern.CreateWithInvariantCulture(ZonedDateTimePattern.ExtendedFormatOnlyIsoPattern.PatternText, _provider));
            }

            return null;
        }
    }
}