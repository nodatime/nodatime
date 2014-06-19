using System;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// A single entry in a Mercurial log.
    /// </summary>
    public sealed class MercurialLogEntry
    {
        public string AuthorEmail { get; private set; }
        public string AuthorName { get; private set; }
        public string Message { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public string Hash { get; private set; }
        public int Revision { get; private set; }

        private MercurialLogEntry()
        {
        }

        internal static MercurialLogEntry FromXElement(XElement element)
        {
            return new MercurialLogEntry
            {
                AuthorEmail = (string) element.Element("author").Attribute("email"),
                AuthorName = (string) element.Element("author"),
                Date = (DateTimeOffset) element.Element("date"),
                Message = (string) element.Element("msg"),
                Hash = (string) element.Attribute("node"),
                Revision = (int) element.Attribute("revision")
            };
        }
    }
}