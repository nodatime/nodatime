using System;
using System.ComponentModel;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides <see cref="string"/> for <see cref="T:LocalDateTimePattern.ExtendedIso"/>.
    /// </summary>
    public class LocalDateTimeTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string localDateTimeString)
            {
                var parseResult = LocalDateTimePattern.ExtendedIso.Parse(localDateTimeString);

                if (parseResult.Success)
                {
                    return parseResult.Value;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                LocalDateTime localDateTime = (LocalDateTime) value;
                return LocalDateTimePattern.ExtendedIso.Format(localDateTime);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}