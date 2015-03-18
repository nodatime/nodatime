using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    public static class MercurialLog
    {
        public static ImmutableList<SourceLogEntry> Load(string uri)
        {
            return XDocument.Load(uri)
                            .Descendants("logentry")
                            .Select(LogFromXElement)
                            .OrderByDescending(x => x.Date)
                            .ToImmutableList();
        }

        internal static SourceLogEntry LogFromXElement(XElement element)
        {
            return new SourceLogEntry.Builder {
                AuthorEmail = (string)element.Element("author").Attribute("email"),
                AuthorName = (string)element.Element("author"),
                Date = (DateTimeOffset)element.Element("date"),
                Message = (string)element.Element("msg"),
                Hash = (string)element.Attribute("node")
            }.Build();
        }
    }
}