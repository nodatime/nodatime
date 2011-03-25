using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace NodaTime.Format
{
    [Flags]
    public enum DateTimeParseStyles
    {
        None = DateTimeStyles.None,
        AllowInnerWhite = DateTimeStyles.AllowInnerWhite,
        AllowLeadingWhite = DateTimeStyles.AllowLeadingWhite,
        AllowTrailingWhite = DateTimeStyles.AllowTrailingWhite,
        AllowWhiteSpaces = DateTimeStyles.AllowWhiteSpaces,
    }
}
