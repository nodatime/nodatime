// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.IO;

namespace NodaTime.Web.Models
{
    public class MarkdownResource
    {
        public string Name { get; }
        public string ContentType { get; }
        private readonly byte[] content;

        public MarkdownResource(string name, byte[] content, string contentType)
        {
            Name = name;
            this.content = content;
            this.ContentType = contentType;
        }

        public Stream GetContent() => new MemoryStream(content, false);
    }
}
