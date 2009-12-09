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
        private MemoryStream stream;
        private DateTimeZoneWriter writer;
        private DateTimeZoneReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DtzIoHelper"/> class.
        /// </summary>
        public DtzIoHelper()
        {
            this.stream = new MemoryStream();
            this.writer = new DateTimeZoneWriter(this.stream);
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        public DateTimeZoneWriter Writer { get { return this.writer; } }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        public DateTimeZoneReader Reader
        {
            get
            {
                if (this.writer != null)
                {
                    this.writer = null;
                }
                if (this.reader == null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    this.reader = new DateTimeZoneReader(this.stream);
                }
                return this.reader;
            }
        }

        public ZoneRecurrence WriteRead(ZoneRecurrence value)
        {
            value.Write(Writer);
            return ZoneRecurrence.Read(Reader);
        }

        public ZoneYearOffset WriteRead(ZoneYearOffset value)
        {
            value.Write(Writer);
            return ZoneYearOffset.Read(Reader);
        }
    }
}
