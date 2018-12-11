using System;
using System.ComponentModel;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides <see cref="string"/> for <see cref="T:LocalTimePattern.ExtendedIso"/>.
    /// </summary>
    public class LocalTimeTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string localTimeString)
            {
                var parseResult = LocalTimePattern.ExtendedIso.Parse(localTimeString);

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
                LocalTime localTime = (LocalTime) value;
                return LocalTimePattern.ExtendedIso.Format(localTime);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}