using System;
using System.ComponentModel;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Provides <see cref="string"/> for <see cref="T:AnnualDatePattern.Iso"/>.
    /// </summary>
    internal sealed class AnnualDateTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string annualDateString)
            {
                var parseResult = AnnualDatePattern.Iso.Parse(annualDateString);

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
                AnnualDate annualDate = (AnnualDate) value;
                return AnnualDatePattern.Iso.Format(annualDate);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}