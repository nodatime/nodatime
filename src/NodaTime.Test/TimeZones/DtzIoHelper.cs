#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using System.IO;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Wrapper around a DateTimeZoneWriter/DateTimeZoneReader pair that reads whatever is
    /// written to it.
    /// </summary>
    public class DtzIoHelper
    {
        private DateTimeZoneReader reader;
        private readonly MemoryStream stream;
        private DateTimeZoneWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DtzIoHelper"/> class.
        /// </summary>
        public DtzIoHelper()
        {
            stream = new MemoryStream();
            writer = new DateTimeZoneWriter(stream);
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        internal DateTimeZoneWriter Writer { get { return writer; } }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        internal DateTimeZoneReader Reader
        {
            get
            {
                if (writer != null)
                {
                    writer = null;
                }
                if (reader == null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    reader = new DateTimeZoneReader(stream);
                }
                return reader;
            }
        }

        public DateTimeZone WriteRead(DateTimeZone timeZone)
        {
            Writer.WriteTimeZone(timeZone);
            return Reader.ReadTimeZone(timeZone.Id);
        }

        internal ZoneRecurrence WriteRead(ZoneRecurrence value)
        {
            value.Write(Writer);
            return ZoneRecurrence.Read(Reader);
        }

        internal ZoneYearOffset WriteRead(ZoneYearOffset value)
        {
            value.Write(Writer);
            return ZoneYearOffset.Read(Reader);
        }
    }
}