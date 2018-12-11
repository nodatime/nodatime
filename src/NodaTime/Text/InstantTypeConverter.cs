using System;
using System.ComponentModel;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides <see cref="string"/> for <see cref="T:InstantPattern.ExtendedIso"/>.
    /// </summary>
    public class InstantTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string instantString)
            {
                var parseResult = InstantPattern.ExtendedIso.Parse(instantString);

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
                Instant instant = (Instant) value;
                return InstantPattern.ExtendedIso.Format(instant);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}