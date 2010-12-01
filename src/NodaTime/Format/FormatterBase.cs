using System;
using System.Globalization;

namespace NodaTime.Format
{
    public abstract class FormatterBase<T> : IFormatter<T>
    {
        public string Format(T value)
        {
            return Format(value, CultureInfo.CurrentCulture);
        }

        public abstract string Format(T value, IFormatProvider formatProvider);
    }
}
