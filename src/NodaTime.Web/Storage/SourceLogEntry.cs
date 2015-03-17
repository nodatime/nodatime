using System;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// A single entry in a source control log.
    /// </summary>
    public sealed class SourceLogEntry
    {
        public string AuthorEmail { get; private set; }
        public string AuthorName { get; private set; }
        public string Message { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public string Hash { get; private set; }
        private SourceLogEntry()
        {
        }

        public sealed class Builder
        {
            public string AuthorEmail { get; set; }
            public string AuthorName { get; set; }
            public string Message { get; set; }
            public DateTimeOffset Date { get; set; }
            public string Hash { get; set; }

            public SourceLogEntry Build() => new SourceLogEntry
                {
                    AuthorEmail = AuthorEmail,
                    AuthorName = AuthorName,
                    Message = Message,
                    Date = Date,
                    Hash = Hash
                };

            public void Reset()
            {
                AuthorEmail = null;
                AuthorName = null;
                Message = null;
                Date = default(DateTimeOffset);
                Hash = null;
            }
        }
    }
}