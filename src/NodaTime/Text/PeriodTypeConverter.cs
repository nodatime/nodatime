using System;
using System.ComponentModel;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides <see cref="string"/> for <see cref="T:PeriodPattern.Roundtrip"/>.
    /// </summary>
    public class PeriodTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string periodString)
            {
                var parseResult = PeriodPattern.Roundtrip.Parse(periodString);

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
                Period period = (Period) value;
                return PeriodPattern.Roundtrip.Format(period);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}