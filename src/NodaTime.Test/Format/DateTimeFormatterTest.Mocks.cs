#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;

using NodaTime.Calendars;
using NodaTime.Format;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterTest
    {
        public class DateTimePrinterMock : IDateTimePrinter
        {
            public int EstimatedPrintedLength
            {
                get { throw new NotImplementedException(); }
            }

            public CalendarSystem Calendar;
            public IDateTimeZone Zone;
            public TextWriter DtWriter;
            public IFormatProvider DtProvider;
            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                DtWriter = writer;
                DtProvider = provider;

                this.Calendar = calendarSystem;
                this.Zone = dateTimeZone;
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }
        }

        public class DateTimeParserMock : IDateTimeParser
        {

            public int EstimatedParsedLength
            {
                get { throw new NotImplementedException(); }
            }

            public DateTimeParserBucket Bucket;
            public string Text;
            public int Position;
            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                Bucket = bucket;
                Text = text;
                Position = position;
                return text.Length;
            }
        }

    }
}
