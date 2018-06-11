// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using System;

namespace NodaTime.Web.Blazor.ViewModels
{
    abstract class ConversionValueBase
    {
        internal string PatternText { get; set; }
        internal string Value { get; set; }
        internal string Error { get; set; }
        internal abstract void Format(Instant currentValue);
        internal abstract Instant? Parse();
        internal abstract string Type { get; }
    }

    class ConversionValue<T> : ConversionValueBase
    {
        internal Func<Instant, T> selector;
        internal Func<T, Instant> reverseSelector;
        internal Func<string, IPattern<T>> patternProvider;

        internal override string Type => typeof(T).Name;

        internal ConversionValue(string patternText, Func<Instant, T> selector, Func<T, Instant> reverseSelector,
            Func<string, IPattern<T>> patternProvider)
        {
            PatternText = patternText;
            this.selector = selector;
            this.reverseSelector = reverseSelector;
            this.patternProvider = patternProvider;
        }

        internal override void Format(Instant currentValue)
        {
            Error = null;
            try
            {
                var pattern = patternProvider(PatternText);
                Value = pattern.Format(selector(currentValue));
            }
            catch (Exception e)
            {
                Error = $"{e.GetType()}: {e.Message}";
            }
        }

        internal override Instant? Parse()
        {
            Error = null;
            try
            {
                var pattern = patternProvider(PatternText);
                T parsed = pattern.Parse(Value).Value;
                Instant instant = reverseSelector(parsed);
                return instant;
            }
            catch (Exception e)
            {
                Error = $"{e.GetType()}: {e.Message}";
                return null;
            }
        }
    }
}
