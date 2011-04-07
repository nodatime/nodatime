using System.Collections.Generic;
using System.Globalization;
using NodaTime.Format;

namespace NodaTime.Test.Format
{
    public interface IFormattingData
    {
        DateTimeParseStyles Styles { get; set; }
        string Name { get; set; }
        ParseFailureKind Kind { get; set; }
        string ArgumentName { get; set; }
        List<object> Parameters { get; set; }
        CultureInfo ThreadCulture { get; set; }
        CultureInfo ThreadUiCulture { get; set; }
        CultureInfo C { get; set; }
    }
}