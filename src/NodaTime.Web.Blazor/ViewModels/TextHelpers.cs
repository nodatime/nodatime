// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;

namespace NodaTime.Web.Blazor.ViewModels
{
    internal static class TextHelpers
    {
        internal static LocalDate? ParseDate(string text, out string error)
        {
            var parseResult = LocalDatePattern.Iso.Parse(text ?? "");
            if (parseResult.Success)
            {
                error = null;
                return parseResult.Value;
            }
            else
            {
                var exception = parseResult.Exception;
                error = $"{exception.GetType().Name}: {exception.Message}";
                return null;
            }
        }

        internal static Period ParsePeriod(string text, out string error)
        {
            var parseResult = PeriodPattern.Roundtrip.Parse(text ?? "");
            if (parseResult.Success)
            {
                error = null;
                return parseResult.Value;
            }
            else
            {
                var exception = parseResult.Exception;
                error = $"{exception.GetType().Name}: {exception.Message}";
                return null;
            }
        }
    }
}
